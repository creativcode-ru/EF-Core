using System;
using System.Collections.Generic;
using System.Linq;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; //**

namespace ContosoUniversityMVC.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //Атрибут DatabaseGenerated=none позволяет приложению указать первичный ключ, а не использовать созданный базой данных.
        [Display(Name = "Номер")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        
        [Range(0, 5)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }

        public Department Department { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        /* Свойство Enrollments является свойством навигации. 
         * Сущность Course может быть связана с любым числом сущностей Enrollment.
         */

        public ICollection<CourseAssignment> CourseAssignments { get; set; }
    }
}
