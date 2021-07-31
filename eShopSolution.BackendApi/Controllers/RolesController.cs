using eShopSolution.Application.System.Roles;
using eShopSolution.Data.Entites;
using eShopSolution.ViewModels.System.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("/api/{controller}")]
    [ApiController]
    [Authorize]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAll();
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] RolePagingRequest request)
        {
            var result = await _roleService.GetAllPaging(request);
            if (result.IsSuccessed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoleCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _roleService.CreateRole(request);
            if (result.IsSuccessed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _roleService.DeleteRole(id);
            if (result.IsSuccessed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> UpdateRole(RoleUpdateRequest request)
        {
            var result = await _roleService.UpdateRole(request);
            if (result.IsSuccessed)
                return Ok(result);
            return BadRequest(result);
        }
    }
}