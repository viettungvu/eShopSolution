using System.ComponentModel.DataAnnotations;

namespace eShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryCreateRequest
    {
        public string Name { set; get; }
        public string LanguageId { set; get; }

        [Display(Name = "SEO Description")]
        public string SeoDescription { set; get; }

        [Display(Name = "SEO Title")]
        public string SeoTitle { set; get; }

        [Display(Name = "SEO Alias")]
        public string SeoAlias { set; get; }

        [Display(Name = "Display order")]
        public int SortOrder { get; set; }

        [Display(Name = "Show on home")]
        public bool IsShowOnHome { get; set; }

        public bool Status { get; set; }
    }
}