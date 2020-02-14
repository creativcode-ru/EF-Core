using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversityMVC.Data;
using ContosoUniversityMVC.Models;

using ContosoUniversityMVC.Models.SchoolViewModels;

namespace ContosoUniversityMVC.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructors
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Instructors.ToListAsync());
        //}

        /*  Безотложная загрузка
         public async Task<IActionResult> Index(int? id, int? courseID)
         {// Безотложная загрузка связанных данных и размещения их в модели представления. 
         // https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-mvc/read-related-data?view=aspnetcore-3.1#learn-how-to-load-related-data

             var viewModel = new InstructorIndexData(); //надо заполнить 3 списка: Instructors, Courses, Enrollments
             viewModel.Instructors = await _context.Instructors
                   .Include(i => i.OfficeAssignment)
                   .Include(i => i.CourseAssignments)
                     .ThenInclude(i => i.Course)
                         .ThenInclude(i => i.Enrollments)
                             .ThenInclude(i => i.Student)
                   .Include(i => i.CourseAssignments)
                     .ThenInclude(i => i.Course)
                         .ThenInclude(i => i.Department)
                   .AsNoTracking()
                   .OrderBy(i => i.LastName)
                   .ToListAsync();
               //В коде задается безотложная загрузка для свойств навигации Instructor.OfficeAssignment и Instructor.CourseAssignments. 
               //Вместе со свойством CourseAssignments загружается свойство Course, 
               //с которым загружаются свойства Enrollments и Department, 
               //а с каждой сущностью Enrollment загружается свойство Student.

               //В коде повторяются CourseAssignments и Course, так как требуется получить два свойства из Course. 
               //см. https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-mvc/read-related-data?view=aspnetcore-3.1#create-an-instructors-page

             if (id != null)
             {
                 ViewData["InstructorID"] = id.Value;
                 Instructor instructor = viewModel.Instructors.Where(
                     i => i.ID == id.Value).Single();
                 //Метод Single преобразует коллекцию в отдельную сущность Instructor, что позволяет получить доступ к ее свойству CourseAssignments. 
                 viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course);
             }

             if (courseID != null)
             {
                 ViewData["CourseID"] = courseID.Value;
                 viewModel.Enrollments = viewModel.Courses.Where(
                     x => x.CourseID == courseID).Single().Enrollments;
             }

             return View(viewModel);
         }
         */

        //Явная загрузка
        public async Task<IActionResult> Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = await _context.Instructors
                  .Include(i => i.OfficeAssignment)
                  .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                  .OrderBy(i => i.LastName)
                  .ToListAsync();

            if (id != null)
            {
                ViewData["InstructorID"] = id.Value;
                Instructor instructor = viewModel.Instructors.Where(
                    i => i.ID == id.Value).Single();
                viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course);
            }

            if (courseID != null)
            {
                ViewData["CourseID"] = courseID.Value;
                var selectedCourse = viewModel.Courses.Where(x => x.CourseID == courseID).Single();
                await _context.Entry(selectedCourse).Collection(x => x.Enrollments).LoadAsync();
                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(x => x.Student).LoadAsync();
                }
                viewModel.Enrollments = selectedCourse.Enrollments;
            }

            return View(viewModel);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        /* Шаблонные методы
         
        // GET: Instructors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstMidName,HireDate")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }
        */

        //Этот код аналогичен коду для методов Edit, за исключением того, что изначально никакие курсы не выбраны. 
        public IActionResult Create()
        {
            var instructor = new Instructor();
            instructor.CourseAssignments = new List<CourseAssignment>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstMidName,HireDate,LastName,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            /* Метод HttpPost Create добавляет каждый выбранный курс в свойство навигации CourseAssignments до того, 
             * как выполнить поиск ошибок проверки и добавить нового преподавателя в базу данных. 
             * Курсы добавляются даже при наличии ошибок модели, поэтому когда имеются такие ошибки 
             * (например, пользователь ввел недопустимую дату) и страница отображается повторно с сообщением об ошибке, 
             * все выбранные курсы восстанавливаются автоматически.
             */
            if (selectedCourses != null)
            {
                instructor.CourseAssignments = new List<CourseAssignment>();
                //для добавления курсов в свойство навигации CourseAssignments нужно инициализировать как пустую коллекцию.
                // это можно сделать и в модели https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-mvc/update-related-data?view=aspnetcore-3.1#add-office-location-and-courses-to-create-page 
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignment { InstructorID = instructor.ID, CourseID = int.Parse(course) };
                    instructor.CourseAssignments.Add(courseToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var instructor = await _context.Instructors.FindAsync(id);
            var instructor = await _context.Instructors
                                    .Include(i => i.OfficeAssignment) //загружает свойство навигации OfficeAssignment
                                    .Include(i => i.CourseAssignments).ThenInclude(i => i.Course) //добавляет безотложную загрузку для свойства навигации Courses
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }
           
            PopulateAssignedCourseData(instructor); //для предоставления сведений массиву флажков с помощью класса модели представления AssignedCourseData.
            return View(instructor);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            //считывает все сущности Course, чтобы загрузить список курсов, используя класс модели представления. 
            var allCourses = _context.Courses;
            var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();

            foreach (var course in allCourses)
            {
                /* Для каждого курса код проверяет, существует ли этот курс в свойстве навигации Courses преподавателя. 
                 * Чтобы создать эффективную подстановку при проверке того, назначен ли курс преподавателю, назначаемые курсы помещаются в коллекцию HashSet.
                 * У курсов, назначенных преподавателю, для свойства Assigned задается значение true. 
                 * Представление будет использовать это свойство, чтобы определить, какие флажки нужно отображать как выбранные.
                 */
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
            ViewData["Courses"] = viewModel; // Наконец, список передается в представление в ViewData.
        }

        /* Шаблонный метод
        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName,HireDate")] Instructor instructor)
        {
            if (id != instructor.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }
        */

        //[HttpPost, ActionName("Edit")]
        //public async Task<IActionResult> EditPost(int? id) //сигнатура метода изменена на EditPost, чтобы не совпадала с методом GET Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedCourses) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                .FirstOrDefaultAsync(s => s.ID == id);
            /* Получает текущую сущность Instructor из базы данных, используя безотложную загрузку для свойства навигации OfficeAssignment. 
             * Это аналогично тому, что сделали в методе HttpGet Edit.
             */

            if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "",
                i => i.FirstMidName, i => i.LastName, i => i.HireDate, i => i.OfficeAssignment))
            {
                /* Обновляет извлеченную сущность Instructor, используя значения из связывателя модели. 
                 * Перегрузка TryUpdateModel позволяет добавить включаемые свойства в список разрешений. 
                 * Это защищает от чрезмерной передачи данных, 
                 */
                if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                    /* Если расположение кабинета отсутствует, задает для свойства Instructor.OfficeAssignment значение null, 
                     * что приведет к удалению связанной строки в таблице OfficeAssignment.
                     */
                }
                UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                try
                {
                    await _context.SaveChangesAsync(); //Сохраняет изменения в базу данных.
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }

            UpdateInstructorCourses(selectedCourses, instructorToUpdate);
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        /*
         * Так как представление не содержит коллекцию сущностей Course, 
         * связыватель модели не может автоматически обновить свойство навигации CourseAssignments. 
         * Вместо использования связывателя модели для обновления свойства навигации CourseAssignments вы делаете это в новом методе UpdateInstructorCourses.
         * Поэтому нужно исключить свойство CourseAssignments из привязки модели.
         * Это не требует внесения никаких изменений в код, вызывающем TryUpdateModel, так как вы используете перегрузку на базе списка разрешений, 
         * а CourseAssignments отсутствует в списке включений.
         */
        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
                /* Если никакие флажки не выбраны, код в UpdateInstructorCourses инициализирует свойство навигации CourseAssignments 
                 * с использованием пустой коллекции.
                 */
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.CourseAssignments.Select(c => c.Course.CourseID));
            /* После этого код в цикле проходит по всем курсам в базе данных и сравнивает каждый из них с теми, 
             * которые сейчас назначены преподавателю, в противоположность тем, которые были выбраны в представлении. 
             * Чтобы упростить эффективную подстановку, последние две коллекции хранятся в объектах HashSet.
             */
            foreach (var course in _context.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    /* Если флажок для курса был установлен, но курс отсутствует в свойстве навигации Instructor.CourseAssignments, 
                     * этот курс добавляется в коллекцию в свойстве навигации.
                     */
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = course.CourseID });
                    }
                }
                else
                {
                    /* Если флажок для курса не был установлен, но курс присутствует в свойстве навигации Instructor.CourseAssignments, 
                     * этот курс удаляется из свойства навигации.
                     */
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove = instructorToUpdate.CourseAssignments.FirstOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }
            }
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var instructor = await _context.Instructors.FindAsync(id);

            Instructor instructor = await _context.Instructors
                .Include(i => i.CourseAssignments)
                .SingleAsync(i => i.ID == id);
            /* Выполняет безотложную загрузку для свойства навигации CourseAssignments. 
             * Вам нужно включить его, иначе EF не будет знать о связанных сущностях CourseAssignment и не удалит их. 
             * Чтобы избежать необходимости считывать их, можно настроить каскадное удаление в базе данных.
             */

            var departments = await _context.Departments
                .Where(d => d.InstructorID == id)
                .ToListAsync();
            departments.ForEach(d => d.InstructorID = null);
            /* Если преподаватель, которого требуется удалить, назначен в качестве администратора любой из кафедр, 
             * удаляется назначение преподавателя из таких кафедр.
             */

            _context.Instructors.Remove(instructor);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}

