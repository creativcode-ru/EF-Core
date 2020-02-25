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

<p align="center">
     <img src="/Images/db-first.jpg" width="294" alt="">
</p>

* Создайте проект Razor Pages с названием `ConsotoDbFirst`  
Добавьте в проект необходимые пакеты NuGet:
```
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
Microsoft.EntityFrameworkCore.Tools
```

## Реконструирование 
Ранее мы установили расширение EF Core Power Tools, теперь пора им воспользоваться. Шёлкаем правой кнопкой мыши на проекте, и в открывшемся списке выбираем: `EF Core Power Tools` затем `Reverse Engineer`:

<p align="center">
     <img src="/Images/ef-core-power-tools.jpg" width="700" alt="">  
</p>

В маленьком окошке нам предлагается добавить соединение с БД, нажимаем кнопку `Add...`

<p align="center">
     <img src="/Images/db-connection.jpg" width="552" alt="">  
</p>

Это стандартное окно подключения к БД, выбираем нашу БД, и `OK`. Возвращаемся первоначальному окошку, и не ставим галочку `Use Ef Core 3.x`:
<p align="center">
     <img src="/Images/add-model.jpg" width="404" alt="">  
</p>

Далее появится окошко, где можно выбрать все таблицы (или некоторые), снова `OK` и открывается окно параметров создания модели из БД:

<p align="center">
     <img src="/Images/generate-model.jpg" width="407" alt="">  
</p>

Здесь задаем необходимые каталога Model и Data, ставим галочку для анотации данных. После нажатия `OK` происходит реконструирование данных.

## Отображение модели 

⚗ ещё готовится:)

<br /><br />
<p align="center">
  Практические консультации вы можете получить на наших <a  href="http://creativcode.ru/learn" target="_blank" >веб курсах в Сочи, Адлер</a>:<br /><br />
   <a  href="http://creativcode.ru/learn/webnet" target="_blank" >
  <img src="http://creativcode.ru/img/learn/net-backend.jpg" width="400" alt="">
   </a>
</p>
