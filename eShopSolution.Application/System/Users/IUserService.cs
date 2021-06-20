using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Users;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Users
{
    public interface IUserService
    {
        Task<string> Authenticate(LoginRequest request);

        Task<bool> Register(RegisterRequest request);

        Task<bool> ChangePassword(string username, ChangePasswordRequest request);

        Task<bool> Update(string username, UserUpdateRequest request);

        Task<bool> Delete(string username);

        Task<PagedResult<UserVm>> GetUserPaging(UserPagingRequest request);
    }
}