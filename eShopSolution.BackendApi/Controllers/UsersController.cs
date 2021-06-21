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
    [Authorize]
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
            var result = await _userService.Authenticate(request);
            if (string.IsNullOrEmpty(result.ResultObject))
                return BadRequest(result.Message);
            return Ok(result.ResultObject);
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Register(request);
            if (!result.IsSuccessed)
                return BadRequest(result.Message);
            return Ok();
        }

        [HttpPatch("{username}/change-password")]
        public async Task<IActionResult> ChangePassword(string username, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.ChangePassword(username, request);
            if (!result.IsSuccessed)
                return BadRequest(result.Message);
            return Ok();
        }

        //SELETE: https:/localhost/api/user/username?
        [HttpDelete("{username}")]
        public async Task<IActionResult> Delete(string username)
        {
            var result = await _userService.Delete(username);
            if (!result.IsSuccessed)
                return BadRequest(result.Message);
            return Ok();
        }

        //https:/localhost/api/users/paging?pageIndex=1&keyword=?
        [HttpGet("paging")]
        public async Task<IActionResult> GetUserPaging([FromQuery] UserPagingRequest request)
        {
            var data = await _userService.GetUserPaging(request);
            return Ok(data.ResultObject);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var result = await _userService.GetByUsername(username);
            if (!result.IsSuccessed)
                return BadRequest(result.Message);
            return Ok();
        }

        //PUT: https:/localhost/api/user/username
        [HttpPut("{username}")]
        public async Task<IActionResult> Update(string username, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Update(username, request);
            if (!result.IsSuccessed)
                return BadRequest(result.Message);
            return Ok();
        }
    }
}