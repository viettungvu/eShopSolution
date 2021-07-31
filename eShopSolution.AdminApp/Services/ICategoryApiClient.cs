using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public interface ICategoryApiClient
    {
        Task<ApiResult<bool>> Create(CategoryCreateRequest request);

        Task<ApiResult<bool>> Update(CategoryUpdateRequest request);

        Task<ApiResult<bool>> Delete(int categoryId);

        Task<ApiResult<List<CategoryVm>>> GetAll();

        Task<ApiResult<CategoryVm>> GetById(int categoryId);

        Task<ApiResult<PagedResult<CategoryVm>>> GetAllPaging(CategoryPagingRequest request);
    }
}