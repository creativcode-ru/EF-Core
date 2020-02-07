# EF-Core
Доступ к базе данных SQL для приложений .Net Core на основе EF Core

## Начало работы с EF Core
[Создание консольного приложения EFGetStarted](https://docs.microsoft.com/ru-ru/ef/core/get-started/?tabs=visual-studio)

При копировании команд для создания базы данных, не забудьте нажать Enter, чтобы выполнить последнюю команду Update-Database

Посмотреть содержимое БД SQLite можно с помощью [Браузер БД SQLite](https://sqlitebrowser.org/)

## EF Core с Razor Pages
[Учебник Razor Pages с Entity Framework Core в ASP.NET Core](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/intro?view=aspnetcore-3.1&tabs=visual-studio)

База данных создается в стандартном каталоге C:/Users/{user} . Для доступа к БД (просмотр, удаление) можно использовать SQL Server Management Studio (SSMS) - при подключении выбрать (localdb)\MSSQLLocalDB

## EF Core с MVC
[Учебник по работе с ASP.NET Core 2.0 MVC и EF Core](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-mvc/?view=aspnetcore-3.1)

В проекте используется Net Core 3.1, что то приходится подправить, на основе руководства по EF Core с Razor Pages.  
Классы модели данных в точности такие же, как и для Razor Pages.

При добавлении класса SchoolContext требуется добавить в приложение несколько пакетов с помощью NuGet:
ссылку на общий пакет Microsoft.EntityFrameworkCore -- не добавлять.

Добавить те же пакеты, что и в приложении `EF Core с Razor Pages`

<p align="center">
  <img src="ContosoUniversityMVC/wwwroot/img/prtsc/addNuget.png" width="400" alt="Установка пакетов Nuget для EF Core 3.1 с MVC">
</p>

### [Реализация функциональности CRUD](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-mvc/crud?view=aspnetcore-3.1)
● Настройка страницы сведений: представление Details для отображения списка Enrollments;  
● Обновление страницы Create: настройка обработчика сбоев, защита от чрезмерной передачи данных;  
● Обновление страницы редактирования: 
