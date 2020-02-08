using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversityMVC
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

        /* метод CreateAsync принимает размер и номер страницы и вызывает соответствующие методы Skip и Take объекта IQueryable. 
         * Метод ToListAsync объекта IQueryable при вызове возвратит список, содержащий только запрошенную страницу. 
         * Для включения и отключения кнопок перелистывания страниц Previous и Next можно использовать свойства HasPreviousPage и HasNextPage.
         * 
            Для создания объекта PaginatedList<T> вместо конструктора используется метод CreateAsync, поскольку конструкторы не могут выполнять асинхронный код.
         */
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
