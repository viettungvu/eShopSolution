using eShopSolution.Data.Entites;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Roles
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleService(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task<ApiResult<bool>> AddUserToRole(List<AppUser> appUsers)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<bool>> CreateRole(RoleCreateRequest request)
        {
            var role = await _roleManager.FindByNameAsync(request.Name);
            if (role != null)
                return new ApiErrorResult<bool>($"Role {request.Name} is exist");
            role.Name = request.Name;
            role.Description = request.Decription;
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Create role failed");
        }

        public async Task<ApiResult<bool>> DeleteRole(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                return new ApiErrorResult<bool>($"{id} không tồn tại");
            var userInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            foreach (var user in userInRole)
            {
                await _userManager.RemoveFromRoleAsync(user, role.Name);
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Xóa thất bại");
        }

        public async Task<ApiResult<List<RoleVm>>> GetAll()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleVm()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();
            if (roles == null)
                return new ApiErrorResult<List<RoleVm>>("Roles không tồn tại");
            return new ApiSuccessResult<List<RoleVm>>(roles);
        }

        public async Task<ApiResult<bool>> UpdateRole(RoleUpdateRequest request)
        {
            var role = await _roleManager.FindByNameAsync(request.Name);
            if (role == null)
                return new ApiErrorResult<bool>("Role không tồn tại");
            role.Name = request.Name;
            role.Description = request.Decription;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Update role thất bại");
        }

        public async Task<ApiResult<PagedResult<RoleVm>>> GetAllPaging(RolePagingRequest request)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(request.RoleName))
            {
                query = query.Where(x => x.Name.Contains(request.RoleName));
            }
            int totalRow = await query.CountAsync();
            var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new RoleVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToListAsync();
            //4. Select
            var pagedResult = new PagedResult<RoleVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                ListItems = await data
            };
            return new ApiSuccessResult<PagedResult<RoleVm>>(pagedResult);
        }

        public async Task<ApiResult<RoleVm>> GetByName(string roleName)
        {
            var result = await _roleManager.FindByNameAsync(roleName);
            if (result == null)
                return new ApiErrorResult<RoleVm>("Not found");

            var role = new RoleVm()
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description,
            };
            return new ApiSuccessResult<RoleVm>(role);
        }
    }
}