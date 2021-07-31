using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Categories
{
    public interface ICategoryService
    {
        Task<ApiResult<bool>> Create(CategoryCreateRequest request);

        Task<ApiResult<bool>> Update(int categoryId, CategoryUpdateRequest request);

        Task<ApiResult<bool>> Delete(int categoryId);

        Task<ApiResult<List<CategoryVm>>> GetAll();

        Task<ApiResult<PagedResult<CategoryVm>>> GetAllPaging(CategoryPagingRequest request);

        Task<ApiResult<CategoryVm>> GetById(int categoryId);
    }
}