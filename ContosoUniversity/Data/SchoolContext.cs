using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

/*
 * Контекст базы данных — это основной класс, который координирует функциональные возможности EF Core для определенной модели данных. 
 * Контекст наследуется от Microsoft.EntityFrameworkCore.DbContext. 
 * === Контекст указывает сущности, которые включаются в модель данных. ===
 */

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        //Код ниже создает свойство DbSet<TEntity> для каждого набора сущностей, которые включаются в модель данных:
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        /* Так как набор сущностей содержит несколько сущностей, свойства DBSet должны иметь имена во множественном числе. 
         * Так как средство формирования шаблонов создало DBSet Student, 
         * в этом шаге его имя меняется на имя во множественном числе: Students.
         */
        //Чтобы код Razor Pages соответствовал новому имени DBSet, измените _context.Student на _context.Students в рамках всего

        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");

            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Instructor>().ToTable("Instructor");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");

            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseID, c.InstructorID }); //код добавляет новые сущности и настраивает составной первичный ключ сущности CourseAssignment.
        }
    }
}