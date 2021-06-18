using eShopSolution.Data.Entites;
using eShopSolution.Ultilities.Exceptions;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, string.Join(";", roles))
                    };
                    var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var token = new JwtSecurityToken(
                        issuer: _config["Tokens:Issuer"],
                        audience: _config["Tokens:Audience"],
                        expires: DateTime.Now.AddMinutes(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                        );
                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
                return null;
            }
            return null;
        }

        public async Task<bool> Delete(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return true;
            return false;
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
            if (result.Succeeded)
                return true;
            return false;
        }

        public async Task<bool> Update(string username, UserUpdateRequest request)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new EShopException($"Tài khoản {username} không tồn tại");
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DoB = request.DoB;
            user.Email = request.Email;
            user.PhoneNumber = request.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return true;
            return false;
        }
    }
}