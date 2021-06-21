using eShopSolution.ViewModels.Common;
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

        Task<ApiResult<UserVm>> GetByUsername(string username);

        Task<ApiResult<PagedResult<UserVm>>> GetUserPaging(UserPagingRequest request);

        Task<ApiResult<bool>> Delete(string username);

        Task<ApiResult<bool>> Update(string username, UserUpdateRequest request);
    }
}