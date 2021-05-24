using eShopSolution.Application.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Categories;
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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("new")]
        public async Task<IActionResult> Create([FromForm] CategoryCreateRequest request)
        {
            var categoryId = await _categoryService.Create(request);
            var category = await _categoryService.GetById(categoryId, request.LanguageId);
            if (categoryId == 0)
                return BadRequest($"Category {request.Name} is existed");
            return Created(nameof(GetById), category);
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> Update(int categoryId, [FromForm] CategoryUpdateRequest request)
        {
            var category = await _categoryService.Update(categoryId, request);
            if (!category)
                return BadRequest("Failed");
            return Ok("Successful");
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> Delete(int categoryId)
        {
            var result = await _categoryService.Delete(categoryId);
            if (!result)
                return BadRequest("Failed");
            return Ok("Deleted");
        }

        [HttpGet("{categoryId}/{languageId}")]
        public async Task<IActionResult> GetById(int categoryId, string languageId)
        {
            var categoryVm = await _categoryService.GetById(categoryId, languageId);
            if (categoryVm == null)
                return NotFound("Not found");
            return Ok(categoryVm);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categoryVm = await _categoryService.GetAll();
            if (categoryVm == null)
                return NotFound("List empty");
            return Ok(categoryVm);
        }
    }
}