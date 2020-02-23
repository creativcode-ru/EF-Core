using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;  // Add VM
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public InstructorIndexData InstructorData { get; set; }
        public int InstructorID { get; set; }
        public int CourseID { get; set; }

        public async Task OnGetAsync(int? id, int? courseID)
        {
            //Безотложная загрузка свойств навигации
            /* методы Include и ThenInclude повторяются для CourseAssignments и Course. 
             * Это необходимо для того, чтобы задать безотложную загрузку для двух свойств навигации сущности Course.
             */
            InstructorData = new InstructorIndexData();
            InstructorData.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                /* Предположим, что пользователям редко требуется просматривать зачисления на курс. 
                 * В этом случае можно оптимизировать работу, загружая данные о зачислении только при их запросе.
                 * Тогда комментируем это фрагментю и добавляем явную занрузку курсов
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Enrollments)
                            .ThenInclude(i => i.Student)*/
                //.AsNoTracking() -- для подклчения явной занрузки трекинг должен бытьвключен 
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (id != null)
            {
                /* Из свойства навигации CourseAssignments этого преподавателя 
                 * загружается свойство модели представления Courses вместе с сущностями Course.
                 */
                InstructorID = id.Value;
                Instructor instructor = InstructorData.Instructors
                    .Where(i => i.ID == id.Value).Single();
                InstructorData.Courses = instructor.CourseAssignments.Select(s => s.Course);
            }

            if (courseID != null)
            {
                CourseID = courseID.Value;
                var selectedCourse = InstructorData.Courses
                    .Where(x => x.CourseID == courseID).Single();
                //---------------- Явная загрузка курсов -------------------------
                await _context.Entry(selectedCourse).Collection(x => x.Enrollments).LoadAsync();
                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(x => x.Student).LoadAsync();
                }
                //---------------- end: Явная загрузка курсов --------------------

                InstructorData.Enrollments = selectedCourse.Enrollments;
            }
        }
    }
}