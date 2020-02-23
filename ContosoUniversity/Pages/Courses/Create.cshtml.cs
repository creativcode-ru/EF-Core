using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Courses
{
    //public class CreateModel : PageModel
    public class CreateModel : DepartmentNamePageModel //при добвлении списка преподавателей
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public CreateModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        /* Шаблон
        public IActionResult OnGet()
        {
        ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
            return Page();
        }
        */
        public IList<Course> Courses { get; set; }
        /*
        public async Task OnGetAsync()
        {
            Courses = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking() //AsNoTracking повышает производительность, так как возвращаемые сущности не отслеживаются. Отслеживать сущности не нужно, так как они не изменяются в текущем контексте.
                .ToListAsync();
        }
        */
        public IActionResult OnGet()
        {
            PopulateDepartmentsDropDownList(_context); //заполнение списка преподавателей
            return Page();
        }

        [BindProperty]
        public Course Course { get; set; }

        /*
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Courses.Add(Course);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        */

        public async Task<IActionResult> OnPostAsync()
        {
            var emptyCourse = new Course();

            //Использует TryUpdateModelAsync, чтобы предотвратить чрезмерную передачу данных.
            if (await TryUpdateModelAsync<Course>(
                 emptyCourse,
                 "course",   // Prefix for form value.
                 s => s.CourseID, s => s.DepartmentID, s => s.Title, s => s.Credits))
            {
                _context.Courses.Add(emptyCourse);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            // Select DepartmentID if TryUpdateModelAsync fails.
            PopulateDepartmentsDropDownList(_context, emptyCourse.DepartmentID);
            return Page();
        }
    }
}
