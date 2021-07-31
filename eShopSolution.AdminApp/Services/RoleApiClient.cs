using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public class RoleApiClient : BaseApiClient, IRoleApiClient
    {
        public RoleApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
            : base(httpClientFactory, httpContextAccessor)
        {
        }

        public async Task<ApiResult<List<RoleVm>>> GetAll()
        {
            return await OnGetAsync<ApiResult<List<RoleVm>>>("roles");
        }

        public async Task<ApiResult<PagedResult<RoleVm>>> GetAllPaging(RolePagingRequest request)
        {
            return await OnGetAsync<ApiResult<PagedResult<RoleVm>>>($"roles/paging?pageIndex={request.PageIndex}&pageSize={request.PageSize}&keyword={request.RoleName}");
        }

        public async Task<ApiResult<bool>> CreateRole(RoleCreateRequest request)
        {
            return await OnGetAsync<ApiResult<bool>>("roles/create");
        }

        public Task<ApiResult<bool>> DeleteRole(Guid id)
        {
            return OnDeleteAsync($"roles/{id}");
        }

        public Task<ApiResult<bool>> UpdateRole(RoleUpdateRequest request)
        {
            return OnPutAsync("roles/edit", request);
        }

        public async Task<ApiResult<RoleVm>> GetByName(string roleName)
        {
            return await OnGetAsync<ApiResult<RoleVm>>("roles/create");
        }
    }
}