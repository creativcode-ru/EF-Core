using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        /* Свойство EnrollmentID является первичным ключом. В этой сущности используется шаблон classnameID вместо ID. 
         * Для рабочей модели данных выберите один шаблон и используйте только его. 
         * В этом учебнике используются оба шаблона, чтобы проиллюстрировать их работу.
         * Использование ID без classname упрощает внесение некоторых изменений в модель данных.
         */
        public int CourseID { get; set; }
        public int StudentID { get; set; } //Свойство StudentID представляет собой внешний ключ
        /* EF Core воспринимает свойство как внешний ключ, если он имеет имя 
        * <navigation property name><primary key property name>. 
        * Например, StudentID является внешним ключом для свойства навигации Student, 
        * так как сущность Student имеет первичный ключ ID. 
        * Свойства внешнего ключа также могут называться 
        * <primary key property name>. 
        * Например, CourseID, так как сущность Course имеет первичный ключ CourseID.
        */
        [DisplayFormat(NullDisplayText = "Оценка")]
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
        /* Ключу StudentID соответствует свойство навигации Student. 
         * Сущность Enrollment связана с одной сущностью Student, поэтому свойство содержит отдельную сущность Student.
         */

       
    }
}
