using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity
{
    public class DetailsModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public DetailsModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public Student Student { get; set; }

        /* Шаблон
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Students.FirstOrDefaultAsync(m => m.ID == id);

            if (Student == null)
            {
                return NotFound();
            }
            return Page();
        }
        */

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            /* Методы Include и ThenInclude инструктируют контекст для загрузки свойства навигации Student.Enrollments, 
             * а также свойства навигации Enrollment.Course в пределах каждой регистрации. 
             * Эти методы более подробно рассматриваются в учебнике, посвященном чтению связанных данных.
             * 
               Метод AsNoTracking повышает производительность в тех сценариях, 
               где возвращаемые сущности не обновляются в текущем контексте. AsNoTracking рассматривается позднее в этом учебнике.
             */


            if (Student == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
