using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class CategoryApiClient : BaseApiClient, ICategoryApiClient
    {
        public CategoryApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor)
        {
        }

        public async Task<ApiResult<bool>> Create(CategoryCreateRequest request)
        {
            return await OnPostAsync("categories/", request);
        }

        public async Task<ApiResult<bool>> Delete(int categoryId)
        {
            return await OnDeleteAsync($"categories/{categoryId}");
        }

        public async Task<ApiResult<List<CategoryVm>>> GetAll()
        {
            return await OnGetAsync<ApiResult<List<CategoryVm>>>("categories/");
        }

        public async Task<ApiResult<CategoryVm>> GetById(int categoryId)
        {
            return await OnGetAsync<ApiResult<CategoryVm>>($"categories/{categoryId}");
        }

        public async Task<ApiResult<bool>> Update(CategoryUpdateRequest request)
        {
            return await OnPutAsync<CategoryUpdateRequest>($"categories/{request.Id}", request);
        }

        public async Task<ApiResult<PagedResult<CategoryVm>>> GetAllPaging(CategoryPagingRequest request)
        {
            return await OnGetAsync<ApiResult<PagedResult<CategoryVm>>>($"categories/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
        }
    }
}