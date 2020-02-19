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
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; } //сохраняет значение для фильтрации
        public string CurrentSort { get; set; } //сохраняет праметр сортировки

        //public IList<Student> Students { get;set; } //имя сойства Student заменяется на Students
        public PaginatedList<Student> Students { get; set; }

        /* Метод OnGetAsync принимает параметр sortOrder из строки запроса в URL-адресе. 
         * URL-адрес (включая строку запроса) формируется вспомогательной функцией тегов привязки.
         * 
         * Параметр sortOrder имеет значение "Name" или "Date". 
         * После параметра sortOrder может стоять "_desc", чтобы указать порядок по убыванию. 
         * По умолчанию используется порядок сортировки по возрастанию
         */
        public async Task OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            //параметры сортировки из строки запроса
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageIndex = 1; //сбрасывает индекс страницы в значение 1 при получении новой строки поиска;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Student> studentsIQ = from s in _context.Students
                                             select s;
           
            if (!String.IsNullOrEmpty(searchString))
            {
                studentsIQ = studentsIQ.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }


            switch (sortOrder)
            {
                case "name_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    studentsIQ = studentsIQ.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    studentsIQ = studentsIQ.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    studentsIQ = studentsIQ.OrderBy(s => s.LastName);
                    break;
            }

            //Students = await studentsIQ.AsNoTracking().ToListAsync(); //здесь происходит обращение к БД
            int pageSize = 3;
            Students = await PaginatedList<Student>.CreateAsync(
                studentsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
