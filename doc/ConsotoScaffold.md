[← EF Core](/README.md)  

# Database First EF Core Razor Pages

В данном примере рассматривается использование расширение Visual Studio для создания классов модели из существующнй БД.  
Вы можете попробывать и стандартный подход - [Реконструирование](https://docs.microsoft.com/ru-ru/ef/core/managing-schemas/scaffolding) спомощью команды Диспечера пакетов (PMC) `Scaffold-DbContext`, не забудьте при этом указать также ключ `-DataAnnotations`, для формирования анатаций к данным. Подробнее можно посмотреть в видео [Working with an Existing Database [2 of 5]](https://channel9.msdn.com/Series/Entity-Framework-Core-101/Working-with-an-Existing-Database).  
В моём случае, это не сработало - возникала ошибка при подключении к БД. Предлагаемый ниже способ дал требуемый результат.

## Подготовка

⚗ ещё готовится:)
