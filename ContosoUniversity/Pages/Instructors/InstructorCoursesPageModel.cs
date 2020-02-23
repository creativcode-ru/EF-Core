using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

/*
 * Базовый класс InstructorCoursesPageModel будет использоваться для моделей страниц редактирования и создания. 
 * PopulateAssignedCourseData считывает все сущности Course для заполнения списка AssignedCourseDataList. 
 * Для каждого курса код задает CourseID, название, а также сведения о назначении курсу преподавателя. 
 * Для эффективного поиска используется класс HashSet.
 *  https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/update-related-data?view=aspnetcore-3.1#create-an-instructor-page-model-base-class
 */

namespace ContosoUniversity.Pages.Instructors
{
    public class InstructorCoursesPageModel : PageModel
    {

        public List<AssignedCourseData> AssignedCourseDataList;

        public void PopulateAssignedCourseData(SchoolContext context,
                                               Instructor instructor)
        {
            var allCourses = context.Courses;
            var instructorCourses = new HashSet<int>(
                instructor.CourseAssignments.Select(c => c.CourseID));
            AssignedCourseDataList = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                AssignedCourseDataList.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
        }

        /* Так как страница Razor не содержит коллекцию сущностей Course, 
         * связыватель модели не может автоматически обновить свойство навигации CourseAssignments. 
         * Вместо использования связывателя модели для обновления свойства навигации CourseAssignments 
         * вы делаете это в новом методе UpdateInstructorCourses. 
         * Поэтому нужно исключить свойство CourseAssignments из привязки модели. 
         * Это не требует внесения никаких изменений в код, вызывающем TryUpdateModel, 
         * так как вы используете перегрузку на базе списка разрешений, а CourseAssignments отсутствует в списке включений.
         */

        public void UpdateInstructorCourses(SchoolContext context,
            string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {/* Если никакие флажки не выбраны, 
                инициализируется свойство навигации CourseAssignments с использованием пустой коллекции
                */
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            /* После этого код в цикле проходит по всем курсам в базе данных и сравнивает каждый из них с теми, 
             * которые сейчас назначены преподавателю, в противоположность тем, которые были выбраны на странице. 
             * Чтобы упростить эффективную подстановку, последние две коллекции хранятся в объектах HashSet.
             */
            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.CourseAssignments.Select(c => c.Course.CourseID));
            foreach (var course in context.Courses)
            {
                /* Если флажок для курса был установлен, но курс отсутствует в свойстве навигации Instructor.CourseAssignments, 
                 * этот курс добавляется в коллекцию в свойстве навигации.
                 */
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.CourseAssignments.Add(
                            new CourseAssignment
                            {
                                InstructorID = instructorToUpdate.ID,
                                CourseID = course.CourseID
                            });
                    }
                }
                else
                {
                    /* Если флажок для курса не был установлен, но курс присутствует в свойстве навигации Instructor.CourseAssignments, 
                     * этот курс удаляется из свойства навигации.
                     */
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove
                            = instructorToUpdate
                                .CourseAssignments
                                .SingleOrDefault(i => i.CourseID == course.CourseID);
                        context.Remove(courseToRemove);
                    }
                }
            }
        }
    }
}