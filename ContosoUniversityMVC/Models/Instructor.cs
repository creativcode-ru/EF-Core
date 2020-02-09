using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations; //использование атрибутов данных
using System.ComponentModel.DataAnnotations.Schema; //для переименовании колонок

namespace ContosoUniversityMVC.Models
{
    public class Instructor
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Имя")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [Column("FirstName")]
        [Display(Name = "Фамилия")]
        [StringLength(50)]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date), Display(Name = "Принят на работу"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Полное имя")]
        public string FullName
        {
            get { return LastName + ", " + FirstMidName; }
        }

        public ICollection<CourseAssignment> CourseAssignments { get; set; } //Преподаватель может проводить любое количество курсов, поэтому CourseAssignments определен как коллекция.
        public OfficeAssignment OfficeAssignment { get; set; }
        /* Бизнес-правила университета Contoso указывают, что преподаватель может иметь не более одного кабинета, 
         * поэтому свойство OfficeAssignment содержит отдельную сущность OfficeAssignment 
         * (которая может иметь значение null, если кабинет не назначен).
         */
    }
}
