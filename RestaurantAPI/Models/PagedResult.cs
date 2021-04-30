using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalPages { get; set; }
        public int ItemFrom { get; set; }
        public int ItemTo { get; set; }
        public int TotalItemsCount { get; set; }

        public PagedResult(List<T> items,int totalItemsCount, int pageSize, int pageNumber)
        {
            Items = items;
            TotalItemsCount = totalItemsCount;
            TotalPages = (int)Math.Ceiling(TotalItemsCount / (double)pageSize);
            ItemFrom = pageSize * (pageNumber - 1) + 1;
            ItemTo = ItemFrom + pageSize - 1;
        }
    }
}
