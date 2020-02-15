using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversityMVC.Data;
using ContosoUniversityMVC.Models;

namespace ContosoUniversityMVC.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly SchoolContext _context;

        public DepartmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Departments.Include(d => d.Administrator);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,Name,Budget,StartDate,InstructorID,RowVersion")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                             .Include(i => i.Administrator)
                             .AsNoTracking()
                             .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        /* Шаблонный метод
        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,Name,Budget,StartDate,InstructorID,RowVersion")] Department department)
        {
            if (id != department.DepartmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentToUpdate = await _context.Departments.Include(i => i.Administrator).FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                await TryUpdateModelAsync(deletedDepartment);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }

            _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            /* Представление сохраняет исходное значение RowVersion в скрытом поле, а данный метод получает это значение в параметре rowVersion. 
             * Перед вызовом SaveChanges нужно поместить это исходное значение свойства RowVersion в коллекцию OriginalValues для сущности.
             */

            if (await TryUpdateModelAsync<Department>(
                departmentToUpdate,
                "",
                s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorID))
            {
                try
                {

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                { //конфликт параллельного редактирования, данные нельзя сохранять

                    var exceptionEntry = ex.Entries.Single();
                    /* Код в блоке catch для этого исключения возвращает затронутую сущность Department, 
                     * имеющую обновленные значения из свойства Entries в объекте исключения.
                     * Коллекция Entries будет иметь всего один объект EntityEntry. 
                     * Вы можете использовать его для получения новых значений, введенных пользователем, и текущих значений в базе данных.
                     */
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    { //кафедра была удалена другим пользователем.
                        ModelState.AddModelError(string.Empty,
                            "Нет возможнгсти сохранить данные. Кафедра удалена другим пользователем.");
                    }
                    else
                    { //одно или несколько полей были изменены другим пользователем
                        var databaseValues = (Department)databaseEntry.ToObject();

                        /* Код добавляет настраиваемое сообщение об ошибке для каждого столбца, 
                         * для которого значения базы данных отличаются от введенных пользователем на странице "Edit"
                         */
                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {databaseValues.Name}");
                        }
                        if (databaseValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Budget", $"Current value: {databaseValues.Budget:c}");
                        }
                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"Current value: {databaseValues.StartDate:d}");
                        }
                        if (databaseValues.InstructorID != clientValues.InstructorID)
                        {
                            Instructor databaseInstructor = await _context.Instructors.FirstOrDefaultAsync(i => i.ID == databaseValues.InstructorID);
                            ModelState.AddModelError("InstructorID", $"Current value: {databaseInstructor?.FullName}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");

                        /* код задает для RowVersion объекта departmentToUpdate новое значение, полученное из базы данных. 
                         * Это новое значение RowVersion будет сохранено в скрытом поле при повторном отображении страницы "Edit" (Редактирование). 
                         * Когда пользователь в следующий раз нажимает кнопку Save (Сохранить), перехватываются только те ошибки параллелизма, 
                         * которые возникли с момента повторного отображения страницы "Edit" (Редактирование).
                         */
                        departmentToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                        /* Оператор ModelState.Remove является обязательным, так как ModelState имеет старое значение RowVersion. 
                         * На представлении значение ModelState для поля имеет приоритет над значениями свойств модели, если они присутствуют вместе.
                         */
                    }
                }
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", departmentToUpdate.InstructorID);
            return View(departmentToUpdate);
        }

        /* Шаблонный метод
       // GET: Departments/Delete/5
       public async Task<IActionResult> Delete(int? id)
       {
           if (id == null)
           {
               return NotFound();
           }

           var department = await _context.Departments
               .Include(d => d.Administrator)
               .FirstOrDefaultAsync(m => m.DepartmentID == id);
           if (department == null)
           {
               return NotFound();
           }

           return View(department);
       }


       // POST: Departments/Delete/5
       [HttpPost, ActionName("Delete")]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> DeleteConfirmed(int id)
       {
           var department = await _context.Departments.FindAsync(id);
           _context.Departments.Remove(department);
           await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
       }
       */

        /* Этот метод принимает необязательный параметр, который указывает, отображается ли страница повторно после ошибки параллелизма. 
         * Если этот флаг имеет значение true, а указанная кафедра больше не существует, она была удалена другим пользователем. 
         * В этом случае код выполняет перенаправление на страницу индекса. 
         * Если этот флаг имеет значение true, а кафедра существует, она была изменена другим пользователем. 
         * В этом случае код отправляет сообщение об ошибке в представление, используя ViewData.
         */
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(department);
        }

        /* В шаблонном коде, который заменили, этот метод принимал только идентификатор записи.
         * Этот параметр заменен на экземпляр Department, созданный связывателем модели. 
         * Это предоставляет EF доступ к значению свойства RowVersion в дополнение к ключу записи.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department)
        {
            try
            {
                // Если кафедра уже удалена, метод AnyAsync возвращает значение false, а приложение просто возвращается к методу Index.
                if (await _context.Departments.AnyAsync(m => m.DepartmentID == department.DepartmentID))
                {
                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = department.DepartmentID });
                /* При перехвате ошибки параллелизма код повторно отображает страницу подтверждения удаления и предоставляет флаг, 
                 * указывающий, что нужно отобразить сообщение об ошибке параллелизма.
                 */
            }
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}
