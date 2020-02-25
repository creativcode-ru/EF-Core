[← EF Core](/README.md)  

# Database First EF Core Razor Pages

В данном примере рассматривается использование расширение Visual Studio для создания классов модели из существующнй БД.  
Вы можете попробывать и стандартный подход - [Реконструирование](https://docs.microsoft.com/ru-ru/ef/core/managing-schemas/scaffolding) спомощью команды Диспечера пакетов (PMC) `Scaffold-DbContext`, не забудьте при этом указать также ключ `-DataAnnotations`, для формирования анатаций к данным. Подробнее можно посмотреть в видео [Working with an Existing Database [2 of 5]](https://channel9.msdn.com/Series/Entity-Framework-Core-101/Working-with-an-Existing-Database).  
В моём случае, это не сработало - возникала ошибка при подключении к БД. Предлагаемый ниже способ дал требуемый результат.

## Подготовка
* Установите расширение <a href="https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools">EF Core Power Tools <img src="https://erikej.gallerycdn.vsassets.io/extensions/erikej/efcorepowertools/2.4.0/1581168364918/Microsoft.VisualStudio.Services.Icons.Default" width="32" alt=""></a>  
* Установите компонент **редактор DGML**. Для этого запустите стандартный установщик Visual Stiudio, перейдите на вкладку **Отдельные компоненты**, и в строке поиска наберите `dgml`

<p align="center">
     <img src="/Images/dgml.jpg" width="400" alt="">  
</p>

## Создание проекта
* Создание копии БД.  
Для этого будем использовать [ранее созданныей проект ConsotoUniversity](EF-Core-Razor-Pages.md). В нем, в файле _appsettings.json_ в строке подключение изменим имя базы с на ConsotoDbFirst:
```
  "ConnectionStrings": {
    "SchoolContext": "Server=(localdb)\\mssqllocaldb;Database=ConsotoDbFirst;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
```
и выполним в консоле диспечера пакетов (PMC) команду `Update-Database`. В созданнной БД удалим таблицу отслеживания миграций `__EFMigrationsHistory`. 

⚗ ещё готовится:)
