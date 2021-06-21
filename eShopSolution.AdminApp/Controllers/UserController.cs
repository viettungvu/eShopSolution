using eShopSolution.AdminApp.Services;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;

        public UserController(IUserApiClient userApiClient, IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var request = new UserPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Keyword = keyword
            };
            var data = await _userApiClient.GetUserPaging(request);
            if (data.IsSuccessed)
                return View(data.ResultObject);
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            if (_userApiClient.GetByUsername(request.Username) != null)
            {
                ModelState.AddModelError("Username", $"User {request.Username} already registered");
                return View(request);
            }
            if (!ModelState.IsValid)
                return View(ModelState);
            var result = await _userApiClient.Create(request);
            if (result.IsSuccessed)
                return RedirectToAction("Index", "User");
            return View(request);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string username)
        {
            var result = await _userApiClient.Delete(username);
            if (result.IsSuccessed)
                return RedirectToAction("Index", "User");
            ModelState.AddModelError("", result.Message);
            return View(ModelState);
        }

        [HttpGet]
        public IActionResult Edit(string username)
        {
            var user = _userApiClient.GetByUsername(username);
            return View(user);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(string username, UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(ModelState);
            var result = await _userApiClient.Update(username, request);
            if (result.IsSuccessed)
                return RedirectToAction("Index", "User");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var result = await _userApiClient.GetByUsername(username);
            return View(result.ResultObject);
        }
    }
}