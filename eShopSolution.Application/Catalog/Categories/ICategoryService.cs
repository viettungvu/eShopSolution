using eShopSolution.ViewModels.Catalog.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Categories
{
    public interface ICategoryService
    {
        Task<int> Create(CategoryCreateRequest request);
        Task<bool> Update(int categoryId, CategoryUpdateRequest request);
        Task<bool> Delete(int categoryId);
        Task<List<CategoryViewModel>> GetAll();
        Task<CategoryViewModel> GetById(int categoryId, string languageId);
    }
}
