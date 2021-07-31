using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}