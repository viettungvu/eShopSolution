using eShopSolution.Data.Entites;
using eShopSolution.Ultilities.Exceptions;
using eShopSolution.ViewModels.Common;
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

        public async Task<string> Authenticate(LoginRequest request)
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
                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
            }
            return null;
        }

        public async Task<bool> ChangePassword(string username, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
                if (!result.Succeeded)
                    return false;
                return true;
            }
            return false;
        }

        public async Task<bool> Delete(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new EShopException($"Không tìm thấy tài khoản {username}");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return false;
            return true;
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null)
            {
                throw new EShopException($"Tài khoản {request.Username} đã tồn tại");
            }
            var newUser = new AppUser()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DoB = request.DoB,
                Email = request.Email,
                PhoneNumber = request.Phone,
                UserName = request.Username,
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
                return false;
            return true;
        }

        public async Task<bool> Update(string username, UserUpdateRequest request)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                throw new EShopException($"Không tìm thấy tài khoản {username}");
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.DoB = request.Dob;
            user.PhoneNumber = request.Phone;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return true;
            return false;
        }

        public async Task<PagedResult<UserVm>> GetUserPaging(UserPagingRequest request)
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
                    ID = x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    Username = x.UserName,
                    Dob = x.DoB,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                }).ToListAsync();
            //4. Select
            var pagedResult = new PagedResult<UserVm>()
            {
                TotalRecord = totalRow,
                ListItems = await data
            };
            return pagedResult;
        }
    }
}