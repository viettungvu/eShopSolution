using eShopSolution.AdminApp.Services;
using eShopSolution.ViewModels.Catalog.Categories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryApiClient _categoryApiClient;

        public CategoryController(ICategoryApiClient categoryApiClient)
        {
            _categoryApiClient = categoryApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 5)
        {
            var request = new CategoryPagingRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Keyword = keyword
            };
            var data = await _categoryApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;
            if (data.IsSuccessed)
                return View(data.ResultObject);
            return View("404");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var request = new CategoryCreateRequest();
            return PartialView("_Create", request);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", request);
            var result = await _categoryApiClient.Create(request);
            if (!result.IsSuccessed)
            {
                ModelState.AddModelError("", result.Message);
                SetAlert("danger", result.Message);
                return PartialView("_Create", request);
            }
            SetAlert("success", "Tạo danh mục thành công");
            return RedirectToAction("index", "category");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _categoryApiClient.GetById(id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return PartialView("_Details", result.ResultObject);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var request = new CategoryDeleteRequest() { Id = id, };
            return PartialView("_Delete", request);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CategoryDeleteRequest request)
        {
            var result = await _categoryApiClient.Delete(request.Id);
            if (!result.IsSuccessed)
                return BadRequest(result);
            SetAlert("success", "Xóa danh mục thành công");
            return RedirectToAction("index", "category");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryApiClient.GetById(id);
            var data = category.ResultObject;
            var request = new CategoryUpdateRequest()
            {
                Id = data.Id,
                Name = data.Name,
                SeoAlias = data.SeoAlias,
                SeoDescription = data.SeoDescription,
                SeoTitle = data.SeoTitle,
                LanguageId = data.LanguageId,
                IsShowOnHome = data.IsShowOnHome,
                SortOrder = data.SortOrder,
                Status = data.Status
            };
            return PartialView("_Edit", request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryUpdateRequest request)
        {
            var result = await _categoryApiClient.Update(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            SetAlert("success", "Cập nhật danh mục thành công");
            return RedirectToAction("index", "category");
        }
    }
}