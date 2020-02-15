using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversityMVC.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")] //SQL тип данных
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }

        public int? InstructorID { get; set; } //свойство допускает значение NULL

        [Timestamp] //Атрибут Timestamp указывает, что этот столбец будет включен в предложение Where команд Update и Delete
        public byte[] RowVersion { get; set; } //столбец отслеживния для устранения кофликтов паралельного редактирования

        public Instructor Administrator { get; set; } //свойство навигации
        public ICollection<Course> Courses { get; set; } //Кафедра может иметь несколько курсов
    }
}

/*
 * По соглашению Entity Framework разрешает каскадное удаление для внешних ключей, не допускающих значение null, 
 * и связей многие ко многим. Это может привести к циклическим правилам каскадного удаления, 
 * которые вызывают исключение при попытке добавить миграцию. Например, 
 * если вы не определили свойство Department.InstructorID как допускающее значение NULL, 
 * EF настраивает правило каскадного удаления для удаления кафедры при удалении преподавателя, что вам не нужно. 
 * Если бизнес-правила требуют, чтобы свойство InstructorID не допускало значение null, 
 * используйте следующий оператор текучего API, чтобы отключить каскадное удаление для этой связи:


modelBuilder.Entity<Department>()
   .HasOne(d => d.Administrator)
   .WithMany()
   .OnDelete(DeleteBehavior.Restrict) //***
 */
