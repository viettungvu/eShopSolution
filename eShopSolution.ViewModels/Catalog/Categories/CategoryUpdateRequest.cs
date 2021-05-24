using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryUpdateRequest
    {
        public string Name { set; get; }
        public string SeoDescription { set; get; }
        public string SeoTitle { set; get; }
        public string SeoAlias { set; get; }
        public int SortOrder { get; set; }
        public bool IsShowOnHome { get; set; }
        public bool Status { get; set; }
        public string LanguageId { set; get; }
    }
}
