using eShopSolution.AdminApp.Services;
using eShopSolution.ViewModels.System.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly IRoleApiClient _roleApiClient;

        public RoleController(IUserApiClient userApiClient, IConfiguration configuration, IRoleApiClient roleApiClient)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _roleApiClient = roleApiClient;
        }

        public async Task<IActionResult> Index(string roleName, int pageIndex = 1, int pageSize = 5)
        {
            var rolePaging = new RolePagingRequest()
            {
                RoleName = roleName,
                PageIndex = pageIndex,
                PageSize = pageSize,
            };
            var result = await _roleApiClient.GetAllPaging(rolePaging);
            if (result.IsSuccessed)
                return View(result.ResultObject);
            return View("404");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _roleApiClient.CreateRole(request);
            if (result.IsSuccessed)
            {
                SetAlert("success", "Added successful");
                return RedirectToAction("Index", "User");
            }
            SetAlert("error", result.Message);
            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var deleteRequest = new RoleDeleteRequest()
            {
                Id = id,
            };
            return View(deleteRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RoleDeleteRequest request)
        {
            var result = await _roleApiClient.DeleteRole(request.Id);
            if (result.IsSuccessed)
            {
                SetAlert("success", "Deleted successful");
                return RedirectToAction("Index", "User");
            }
            SetAlert("error", result.Message);
            ModelState.AddModelError("", result.Message);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string roleName)
        {
            var role = await _roleApiClient.GetByName(roleName);
            var updateRequest = new RoleUpdateRequest()
            {
                Name = role.ResultObject.Name,
                Decription = role.ResultObject.Description,
            };
            return View(updateRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _roleApiClient.UpdateRole(request);
            if (result.IsSuccessed)
            {
                SetAlert("success", "Update successful");
                return RedirectToAction("Index", "User");
            }
            SetAlert("error", result.Message);
            ModelState.AddModelError("", result.Message);

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string roleName)
        {
            var result = await _roleApiClient.GetByName(roleName);
            if (result.IsSuccessed)
                return View(result.ResultObject);
            return View();
        }
    }
}