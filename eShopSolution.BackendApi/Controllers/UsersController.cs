using eShopSolution.Application.System.Roles;
using eShopSolution.Application.System.Users;
using eShopSolution.ViewModels.System.Roles;
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
        private readonly IRoleService _roleService;

        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Authenticate(request);
            if (string.IsNullOrEmpty(result.ResultObject))
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Register(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        //GET: https:/localhost/api/users/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userService.GetById(id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        //https:/localhost/api/users/paging?pageIndex=1&keyword=?
        [HttpGet("paging")]
        public async Task<IActionResult> GetUserPaging([FromQuery] UserPagingRequest request)
        {
            var data = await _userService.GetUserPaging(request);
            return Ok(data);
        }

        //PUT: https:/localhost/api/user/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.Update(id, request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        //DELETE: https:/localhost/api/user/username?
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.Delete(id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}/role-assign")]
        public async Task<IActionResult> RoleAssign(Guid id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.RoleAssign(id, request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
    }
}