using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversityMVC.Data;
using ContosoUniversityMVC.Models;


// Реализация функциональности CRUD https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-mvc/crud?view=aspnetcore-3.1
namespace ContosoUniversityMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students
            //    .FirstOrDefaultAsync(m => m.ID == id);

            var student = await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .AsNoTracking() //Отключение отслеживания запросов
            .FirstOrDefaultAsync(m => m.ID == id);
            /* Методы Include и ThenInclude инструктируют контекст для загрузки свойства навигации Student.Enrollments, 
             * а также свойства навигации Enrollment.Course в пределах каждой регистрации.
             * 
             * Метод AsNoTracking повышает производительность в тех сценариях, где возвращаемые сущности не обновляются в текущем контексте. 
             * AsNoTracking рассматривается позднее в этом учебнике.
             */

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,EnrollmentDate")] Student student)
        {// ID удаляется из Bind, поскольку генерируется БД
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            /* Исключения DbUpdateException иногда связаны с внешними факторами, а не с ошибкой при программировании приложения, 
             * поэтому рекомендуется попробовать повторить выполненные действия снова. 
             * В этом примере такое поведение не реализовано, однако в рабочем приложении, как правило, исключения заносятся в журнал.
             */

            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students.FindAsync(id);
            var student = await _context.Students.FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            if (id != student.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                /*
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
                */

                //предотвращение чрезмерной отправки данных
                //Подход с предварительным считываением данных. (код может усложняться для обработки конфликтов параллелизма)
                var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
                if (await TryUpdateModelAsync<Student>(
                    studentToUpdate,
                    "",
                    s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
                //Чтобы предотвратить чрезмерную передачу данных, рекомендуется добавить поля, 
                //которые требуется обновлять на странице Edit, в список разрешенных в параметрах TryUpdateModel. 
                //(Пустая строка перед списком полей в списке параметров предназначена для префикса, который используется с именами полей формы.) 
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index)); //>>>
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
                return View(studentToUpdate);

            }
            return View(student);
        }
        /* Без предварительного считывания данных:
         // В качестве альтернативы можно присоединить сущность, созданную связывателем модели, к контексту EF и пометить ее как измененную. 
         public async Task<IActionResult> Edit(int id, [Bind("ID,EnrollmentDate,FirstMidName,LastName")] Student student)
            {
                if (id != student.ID)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(student);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
            }
                return View(student);
            }

           // Этот подход можно использовать в тех случаях, когда пользовательский интерфейс веб-страницы включает все поля сущности 
           // и может обновлять любые из них.
         */

        /* Для Razor Pages:
         * if (await TryUpdateModelAsync<Student>(
        studentToUpdate,
        "student",
        s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
    {
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
         */



        // GET: Students/Delete/5
        /*
         * Этот код принимает необязательный параметр, который указывает, был ли метод вызван после сбоя при сохранении изменений. 
         * Если перед вызовом метода HttpGet Delete не произошел сбой, этот параметр будет иметь значение false. 
         * Если он вызывается методом HttpPost Delete в ответ на ошибку при обновлении базы данных, этот параметр будет иметь значение true, 
         * а в представление передается сообщение об ошибке.
         */
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        //Подход с предварительным чтением для метода HttpPost Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                /* Этот код извлекает выбранную сущность и вызывает метод Remove, чтобы присвоить ей состояние Deleted. 
                 * При вызове метода SaveChanges создается инструкция SQL DELETE.
                 */
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
        /* Подход с созданием и присоединением для метода HttpPost Delete
          
        //Если требуется обеспечить максимальную производительность крупного приложения, можно избежать создания ненужных запросов SQL.
        //Для этого можно создать экземпляр сущности Student, используя только значение первичного ключа, 
        //и затем присвоить этой сущности состояние Deleted.Это все, что платформе Entity Framework необходимо для удаления сущности.

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                try
                {
                    Student studentToDelete = new Student() { ID = id };
                    _context.Entry(studentToDelete).State = EntityState.Deleted;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
                }
            }

            Если также требуется удалить связанные с сущностью данные, убедитесь, что в базе данных настроено каскадное удаление. 
            При таком подходе к удалению сущности платформе EF может быть неизвестно о наличии связанных сущностей, которые требуется удалить.
        */


        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}

/*
 Когда контекст базы данных извлекает строки таблицы и создает представляющие их объекты сущностей, 
 по умолчанию отслеживается состояние синхронизации сущностей в памяти с содержимым базы данных. 
 При обновлении сущности данные в памяти выступают в роли кэша. 
 В веб-приложении такое кэширование часто не нужно, поскольку экземпляры контекста, как правило, существуют недолго 
 (для каждого запроса создается и ликвидируется собственный экземпляр), и контекст, считывающий сущность, 
 как правило, ликвидируется до того, как сущность будет использована снова.

 Чтобы отключить отслеживание объектов сущностей в памяти, вызовите метод AsNoTracking.
*/
