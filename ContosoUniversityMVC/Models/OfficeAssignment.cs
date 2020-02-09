
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversityMVC.Models
{
    public class OfficeAssignment
    {
        [Key] //назначает ключ БД
        public int InstructorID { get; set; }
        /* Между сущностями Instructor и OfficeAssignment действует связь один к нулю или к одному. 
         * Назначение кабинета существует только в связи с преподавателем, которому оно назначено, 
         * поэтому его первичный ключ также является внешним ключом для сущности Instructor. 
         * Однако Entity Framework не распознает InstructorID в качестве первичного ключа этой сущности автоматически, 
         * так как ее имя не соответствует соглашению об именовании ID или classnameID. 
         * Таким образом, атрибут Key используется для определения ее в качестве ключа:
         */

        [StringLength(50)]
        [Display(Name = "Расположение офиса")]
        public string Location { get; set; }

        public Instructor Instructor { get; set; }
    }
}
