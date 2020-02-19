using ContosoUniversity.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ContosoUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host); //создание базы, если её нет

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<SchoolContext>();

                    //context.Database.EnsureCreated(); //Метод EnsureCreated не выполняет никаких действий, если база данных для контекста существует. Если база данных не существует, она создается вместе со схемой. 
                    /*
                     * EnsureCreated обеспечивает описанный ниже рабочий процесс для обработки изменений модели данных.
                            База данных удаляется. Все существующие данные теряются.
                            Модель данных изменяется. Например, добавляется поле EmailAddress.
                            Запустите приложение.
                            Метод EnsureCreated создает базу данных с новой схемой.

                    Этот рабочий процесс хорошо подходит для ранних стадий разработки, когда схема часто меняется, если данные сохранять не требуется. 
                    Однако если данные, введенные в базу данных, необходимо сохранять, ситуация будет иной. В таком случае используйте перенос.
                    https://docs.microsoft.com/ru-ru/aspnet/core/data/ef-rp/intro?view=aspnetcore-3.1&tabs=visual-studio

                    Созданную методом EnsureCreated базу данных нельзя обновить, используя миграции.

                     */

                    DbInitializer.Initialize(context); //содержит метод context.Database.EnsureCreated()
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ошибка при создании Базы данных.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
