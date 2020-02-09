using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; //использование атрибутов данных
using System.ComponentModel.DataAnnotations.Schema; //для переименовании колонок

namespace ContosoUniversityMVC.Models
{
    public class Student
    {
        public int ID { get; set; } //По умолчанию платформа EF Core интерпретирует в качестве первичного ключа свойство ID или classnameID.
        [Required]
        [StringLength(50)] //минимальное значение не влияет на схему данных, но обеспечивает проверку на клиенте и сервере. Для максимального значения изменится схема данных при миграции
        [Display(Name = "Имя (Отчетво)")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Фамилия")]
        [RegularExpression(@"^[A-ZА-Я]+[a-zA-Zа-яА-Я""'\s-]*$")]
        [Column("FirstName")] //БД будет содержать именно это имя: данные будут браться из столбца FirstName таблицы Student или обновляться в нем
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)] //Поддержка функций HTML5 в браузере
        [Display(Name = "Дата поступления")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Полное имя")]
        public string FullName
        {
            get //поскольку только метод GET, то и в БД не будет создан столбец FullName. 
            {
                return LastName + ", " + FirstMidName;
            }
        }

        public ICollection<Enrollment> Enrollments { get; set; } //Это связь
        //Можно использовать и другие типы коллекций, например List<Enrollment> или HashSet<Enrollment>. 
        //Если используется ICollection<Enrollment>, платформа EF Core по умолчанию создает коллекцию HashSet<Enrollment>.
    }
}
