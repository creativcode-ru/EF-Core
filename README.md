# EF-Core
[<img align="right" width="96px" src="http://creativcode.ru/img/app/logo-page.png" />](http://creativcode.ru/)
Доступ к базе данных SQL для приложений .Net Core на основе EF Core  
Рещение содержит ряд учебных проектов с кодом. Проекты созданы на основе документации, на которую приводятся сслки. Вы можете самостоятельно следовать документации, сверяясь, при необходимости с кодом нашего проекта.

## Начало работы с EF Core
[Создание консольного приложения EFGetStarted](https://docs.microsoft.com/ru-ru/ef/core/get-started/?tabs=visual-studio)

При копировании команд для создания базы данных, не забудьте нажать Enter, чтобы выполнить последнюю команду Update-Database  
Посмотреть содержимое БД SQLite можно с помощью [Браузер БД SQLite](https://sqlitebrowser.org/)

Учебный проект `EFGetStarted`

## EF Core с Razor Pages
[Учебник Razor Pages с Entity Framework Core в ASP.NET Core](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/intro?view=aspnetcore-3.1&tabs=visual-studio)

База данных создается в стандартном каталоге `C:/Users/{user}` . Для доступа к БД (просмотр, удаление) можно использовать SQL Server Management Studio (SSMS) - при подключении выбрать (localdb)\MSSQLLocalDB

Учебный проект `ContosoUniversity`

## [EF Core с MVC →](doc-EF-Core-MVC.md)
Учебный проект `ContosoUniversityMVC` по работе с ASP.NET Core MVC и EF Core 3.1  
* Реализация наследования в модели данных  
* Выполнение прямых SQL-запросов  
* Использование динамических запросов LINQ для упрощения кода  

## [EF Core Database First MVC →](doc-EF-Core-Scaffold.md)

Подключение существующей базы данным к приложения MVC c EF Core


<br /><br />
<p align="center">
  Практические консультации вы можете получить на наших <a  href="http://creativcode.ru/learn" target="_blank" >веб курсах в Сочи, Адлер</a>:<br /><br />
   <a  href="http://creativcode.ru/learn/webnet" target="_blank" >
  <img src="http://creativcode.ru/img/learn/net-learn.jpg" width="400" alt="">
   </a>
</p>
