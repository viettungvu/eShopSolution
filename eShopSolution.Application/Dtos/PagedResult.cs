using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Application.Catalog.Dtos
{
    public class PagedResult<T>
    {
        public List<T> ListItems { get; set; }
        public int TotalRecord { get; set; }
    }
}
