using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public interface IRoleApiClient
    {
        Task<ApiResult<List<RoleVm>>> GetAll();

        Task<ApiResult<bool>> CreateRole(RoleCreateRequest request);

        Task<ApiResult<bool>> DeleteRole(Guid id);

        Task<ApiResult<bool>> UpdateRole(RoleUpdateRequest request);

        Task<ApiResult<PagedResult<RoleVm>>> GetAllPaging(RolePagingRequest request);

        Task<ApiResult<RoleVm>> GetByName(string roleName);
    }
}