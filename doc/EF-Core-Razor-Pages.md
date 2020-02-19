# Razor Pages с EF Core

## [Создание проекта веб-приложения](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/intro?view=aspnetcore-3.1&tabs=visual-studio)
<p align="center">
   <a  href="https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/intro?view=aspnetcore-3.1&tabs=visual-studio" target="_blank" >
  <img src="https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/intro/_static/data-model-diagram.png?view=aspnetcore-3.1" width="400" alt="">
   </a>
</p>

* Модель данных. Создание базы данных. Заполнение базы данных.
Метод `EnsureCreated` создает пустую базу данных, поэтому он размещается в методе инициализации БД `DbInitializer.Initialize(context)`.  

* [EF Core CRUD](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/crud?view=aspnetcore-3.1)  
CREATE: обработка уязвимости [Чрезмерная передача данных](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/crud?view=aspnetcore-3.1#overposting), с помощью метода `TryUpdateModel` или модели представления (ViewModel) и метода ` entry.CurrentValues.SetValues(StudentVM);`  
EDIT: Если включать связанные данные не требуется, более эффективным будет метод `FindAsync`. [Состояния сущностей](https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/crud?view=aspnetcore-3.1#entity-states).  
DELETE: Обработка сбоя - операция удаления может завершиться сбоем из-за временных проблем с сетью.