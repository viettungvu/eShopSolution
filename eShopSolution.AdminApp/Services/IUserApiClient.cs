using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using eShopSolution.ViewModels.System.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public interface IUserApiClient
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);

        Task<ApiResult<bool>> Create(RegisterRequest request);

        Task<ApiResult<PagedResult<UserVm>>> GetUserPaging(UserPagingRequest request);

        Task<ApiResult<bool>> Delete(Guid id);

        Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request);

        Task<ApiResult<UserVm>> GetById(Guid id);

        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);
    }
}