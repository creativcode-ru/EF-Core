using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Имя не более 50 символов")]
        [Column("FirstName")] //изменяем название колоки в БД
        [Display(Name = "Имя")] //отображаемый на страниц заголовок
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)] //требуется только дата
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] //*** 
        [Display(Name = "Дата поступления")]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Полное имя")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}