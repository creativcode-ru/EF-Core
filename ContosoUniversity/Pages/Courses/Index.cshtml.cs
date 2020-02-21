using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        
        public IList<Course> Courses { get;set; }
        public async Task OnGetAsync()
        {
            Courses = await _context.Courses
                .Include(c => c.Department).ToListAsync();


        }
        /* ШАБЛОН*/


        /*
    public IList<CourseViewModel> CourseVM { get; set; }


    public async Task OnGetAsync()
    {
        CourseVM = await _context.Courses
                .Select(p => new CourseViewModel
                {
                    CourseID = p.CourseID,
                    Title = p.Title,
                    Credits = p.Credits,
                    DepartmentName = p.Department.Name
                }).ToListAsync();
    }

    public class CourseViewModel
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public string DepartmentName { get; set; }
    }
    */

    }
}
