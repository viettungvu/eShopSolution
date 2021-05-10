using eShopSolution.ViewModels.Catalog.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Products.Public
{
    public class PublicProductPagingRequest:PagingRequestBase
    {
        public int? CategoryId { get; set; }
    }
}
