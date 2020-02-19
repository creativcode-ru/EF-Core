using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }


        
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        // Для включения и отключения кнопок перелистывания страниц Previous (Назад) и Next (Далее) используются свойства HasPreviousPage и HasNextPage.
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        //метод CreateAsync принимает размер и номер страницы и вызывает соответствующие методы Skip и Take объекта IQueryable. 
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync(); //Метод ToListAsync объекта IQueryable при вызове возвращает список, содержащий только запрошенную страницу.
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        /* Метод CreateAsync используется для создания PaginatedList<T>. 
         * Конструктор не позволяет создать объект PaginatedList<T>, так как конструкторы не могут выполнять асинхронный код.
         */
    }
}