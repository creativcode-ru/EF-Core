using System;
using System.Collections.Generic;
using System.Linq;

using System.ComponentModel.DataAnnotations.Schema; //**

namespace ContosoUniversityMVC.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //Атрибут DatabaseGenerated=none позволяет приложению указать первичный ключ, а не использовать созданный базой данных.
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        /* Свойство Enrollments является свойством навигации. 
         * Сущность Course может быть связана с любым числом сущностей Enrollment.
         */
    }
}
