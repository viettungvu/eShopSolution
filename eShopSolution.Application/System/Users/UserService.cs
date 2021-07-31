using eShopSolution.Data.Entites;
using eShopSolution.Ultilities.Exceptions;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _config;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, request.IsRemember, true);
                if (signInResult.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>() {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, string.Join(";", roles))
                    };
                    var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var token = new JwtSecurityToken(
                        issuer: _config["Tokens:Issuer"],
                        audience: _config["Tokens:Audience"],
                        expires: DateTime.Now.ToLocalTime().AddHours(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                        );
                    return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            return new ApiErrorResult<string>("User does not exist");
        }

        public async Task<ApiResult<bool>> Register(RegisterRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == request.Username))
                return new ApiErrorResult<bool>($"User {request.Username} already registered");
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email))
                return new ApiErrorResult<bool>($"Email already registered");
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null)
                return new ApiErrorResult<bool>($"User {request.Username} already registered");
            user = new AppUser()
            {
                FirstName = request.FirstName.Trim().ToUpper(),
                LastName = request.LastName.Trim().ToUpper(),
                DoB = request.DoB,
                Email = request.Email,
                PhoneNumber = request.Phone,
                UserName = request.Username.Trim().ToLower(),
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Register failed");
        }

        public async Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != request.Id))
                return new ApiErrorResult<bool>("Email đã tồn tại");
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.Id = request.Id;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.DoB = request.DoB;
            user.PhoneNumber = request.Phone;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Update failed");
        }

        public async Task<ApiResult<PagedResult<UserVm>>> GetUserPaging(UserPagingRequest request)
        {
            var query = _userManager.Users;
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query
                    .Where(x => x.UserName.Contains(request.Keyword) || x.FirstName.Contains(request.Keyword) || x.LastName.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserVm()
                {
                    Id = x.Id,
                    Firstname = x.FirstName,
                    Lastname = x.LastName,
                    Username = x.UserName,
                    Dob = x.DoB,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                }).ToListAsync();
            //4. Select
            var pagedResult = new PagedResult<UserVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                ListItems = await data
            };
            return new ApiSuccessResult<PagedResult<UserVm>>(pagedResult);
        }

        public async Task<ApiResult<UserVm>> GetById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return new ApiErrorResult<UserVm>("Not found");
            var roles = await _userManager.GetRolesAsync(user);
            var uservm = new UserVm()
            {
                Id = user.Id,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Dob = user.DoB,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                Roles = roles,
            };
            return new ApiSuccessResult<UserVm>(uservm);
        }

        public async Task<ApiResult<bool>> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return new ApiErrorResult<bool>("User does not exist");
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Delete failed");
        }

        public async Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return new ApiErrorResult<bool>("User không tồn tại");
            var removedRoles = request.Roles.Where(x => x.IsSelected == false).Select(x => x.Name).ToList();
            foreach (var role in removedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, role) == true)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }
            var test = await _userManager.RemoveFromRolesAsync(user, removedRoles);
            var addedRoles = request.Roles.Where(x => x.IsSelected).Select(x => x.Name).ToList();
            foreach (var role in addedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, role) == false)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
            return new ApiSuccessResult<bool>();
        }
    }
}