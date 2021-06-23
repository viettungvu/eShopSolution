using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using System;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Users
{
    public interface IUserService
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);

        Task<ApiResult<bool>> Register(RegisterRequest request);

        Task<ApiResult<bool>> ChangePassword(string username, ChangePasswordRequest request);

        Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request);

        Task<ApiResult<bool>> Delete(Guid id);

        Task<ApiResult<UserVm>> GetByUsername(string username);

        Task<ApiResult<UserVm>> GetById(Guid id);

        Task<int> Delete(string username);
    }
}