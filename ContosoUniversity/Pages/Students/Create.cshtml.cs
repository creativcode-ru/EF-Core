using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity
{
    public class CreateModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public CreateModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; }

        /* Шаблонный код OnPostAsync для страницы создания уязвим к чрезмерной передаче данных.
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Students.Add(Student);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        */
        public async Task<IActionResult> OnPostAsync()
        {
            var emptyStudent = new Student(); // новый объект Student, после чего опубликованные поля формы используются для обновления свойств этого объекта. 

            //Использует опубликованные значения формы из свойства PageContext в PageModel.
            if (await TryUpdateModelAsync<Student>(
                emptyStudent,
                "student",   //Ищет поля формы с префиксом "student". Например, Student.FirstMidName. Задается без учета регистра символов.
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate)) //Обновляет только перечисленные свойства.
            {
                /* Использует систему привязки модели для преобразования значений формы из строк в типы модели Student. 
                 * Например, EnrollmentDate следует преобразовать в тип DateTime.
                 */
                _context.Students.Add(emptyStudent);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
