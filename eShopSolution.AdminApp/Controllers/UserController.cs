using eShopSolution.AdminApp.Services;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.System.Roles;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly IRoleApiClient _roleApiClient;

        public UserController(IUserApiClient userApiClient, IConfiguration configuration, IRoleApiClient roleApiClient)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _roleApiClient = roleApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 5)
        {
            var request = new UserPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Keyword = keyword
            };
            var data = await _userApiClient.GetUserPaging(request);
            ViewBag.Keyword = keyword;
            if (data.IsSuccessed)
                return View(data.ResultObject);
            return View("404");
        }

        [HttpGet()]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost()]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _userApiClient.Create(request);
            if (result.IsSuccessed)
            {
                SetAlert("success", "Added successful");
                return RedirectToAction("Index", "User");
            }
            SetAlert("error", result.Message);
            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpGet()]
        public IActionResult Delete(Guid id)
        {
            var deleteRequest = new UserDeleteRequest()
            {
                Id = id
            };
            return View(deleteRequest);
        }

        [HttpPost()]
        public async Task<IActionResult> Delete(UserDeleteRequest request)
        {
            var result = await _userApiClient.Delete(request.Id);
            if (result.IsSuccessed)
            {
                SetAlert("success", "Deleted successful");
                return RedirectToAction("Index", "User");
            }
            SetAlert("error", result.Message);
            ModelState.AddModelError("", result.Message);
            return View();
        }

        [HttpGet()]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userApiClient.GetById(id);
            var updateRequest = new UserUpdateRequest()
            {
                Id = id,
                FirstName = user.ResultObject.Firstname,
                LastName = user.ResultObject.Lastname,
                DoB = user.ResultObject.Dob,
                Phone = user.ResultObject.PhoneNumber,
                Email = user.ResultObject.Email,
            };
            return View(updateRequest);
        }

        [HttpPost()]
        public async Task<IActionResult> Edit(UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _userApiClient.Update(request.Id, request);
            if (result.IsSuccessed)
            {
                SetAlert("success", "Update successful");
                return RedirectToAction("Index", "User");
            }
            SetAlert("error", result.Message);
            ModelState.AddModelError("", result.Message);
            return View(request);
        }

        [HttpGet()]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _userApiClient.GetById(id);
            if (result.IsSuccessed)
                return View(result.ResultObject);
            return View();
        }

        [HttpGet()]
        public async Task<IActionResult> RoleAssign(Guid id)
        {
            var roleAssignRequest = await GetRoleAssignRequest(id);
            return View(roleAssignRequest);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return View();
            var result = await _userApiClient.RoleAssign(request.Id, request);
            if (result.IsSuccessed)
            {
                SetAlert("success", "Role assigned successful");
                return RedirectToAction("RoleAssign", "User");
            }
            SetAlert("error", result.Message);
            var roleAssignRequest = GetRoleAssignRequest(request.Id);
            ModelState.AddModelError("", result.Message);
            return View(roleAssignRequest);
        }

        private async Task<RoleAssignRequest> GetRoleAssignRequest(Guid id)
        {
            var user = await _userApiClient.GetById(id);
            var roles = await _roleApiClient.GetAll();
            var roleAssignRequest = new RoleAssignRequest();
            foreach (var role in roles.ResultObject)
            {
                roleAssignRequest.Id = id;
                roleAssignRequest.Roles.Add(new SelectItem()
                {
                    Id = role.Id.ToString(),
                    Name = role.Name,
                    IsSelected = user.ResultObject.Roles.Contains(role.Name)
                });
            }
            return roleAssignRequest;
        }
    }
}