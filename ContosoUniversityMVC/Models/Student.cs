using System;
using System.Collections.Generic;

namespace ContosoUniversityMVC.Models
{
    public class Student
    {
        public int ID { get; set; } //По умолчанию платформа EF Core интерпретирует в качестве первичного ключа свойство ID или classnameID.
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } //Это связь
        //Можно использовать и другие типы коллекций, например List<Enrollment> или HashSet<Enrollment>. 
        //Если используется ICollection<Enrollment>, платформа EF Core по умолчанию создает коллекцию HashSet<Enrollment>.
    }
}
