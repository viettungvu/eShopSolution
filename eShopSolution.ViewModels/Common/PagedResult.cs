using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Common
{
    public class PagedResult<T>
    {
        public List<T> ListItems { get; set; }
        public int TotalRecord { get; set; }
    }
}
