using eShopSolution.Data.Entites;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Roles
{
    public interface IRoleService
    {
        Task<ApiResult<List<RoleVm>>> GetAll();

        Task<ApiResult<bool>> CreateRole(RoleCreateRequest request);

        Task<ApiResult<bool>> DeleteRole(Guid id);

        Task<ApiResult<bool>> UpdateRole(RoleUpdateRequest request);

        Task<ApiResult<bool>> AddUserToRole(List<AppUser> appUsers);

        Task<ApiResult<PagedResult<RoleVm>>> GetAllPaging(RolePagingRequest request);

        Task<ApiResult<RoleVm>> GetByName(string roleName);
    }
}