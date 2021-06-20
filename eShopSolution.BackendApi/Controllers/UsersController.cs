using eShopSolution.Application.System.Users;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var resultToken = await _userService.Authenticate(request);
            if (string.IsNullOrEmpty(resultToken))
                return BadRequest();
            return Ok(resultToken);
        }

        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Register(request);
            if (!result)
                return BadRequest("Register failed");
            return Ok();
        }

        [HttpPatch("{username}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string username, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.ChangePassword(username, request);
            if (!result)
                return BadRequest("An error occurred while changing your password");
            return Ok();
        }

        [HttpDelete("{username}")]
        [Authorize]
        public async Task<IActionResult> Delete(string username)
        {
            var result = await _userService.Delete(username);
            if (!result)
                return BadRequest($"An error occurred while deleting user {username}");
            return Ok();
        }

        //https:/localhost/api/users/paging?pageIndex=1&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetUserPaging([FromQuery] UserPagingRequest request)
        {
            var data = await _userService.GetUserPaging(request);
            return Ok(data);
        }
    }
}