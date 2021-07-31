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

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _categoryService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _categoryService.Update(id, request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> Delete(int categoryId)
        {
            var result = await _categoryService.Delete(categoryId);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetById(int categoryId)
        {
            var result = await _categoryService.GetById(categoryId);
            if (!result.IsSuccessed)
                return NotFound("Not found");
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAll();
            if (!result.IsSuccessed)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] CategoryPagingRequest request)
        {
            var result = await _categoryService.GetAllPaging(request);
            if (!result.IsSuccessed)
                return BadRequest(result);
            return Ok(result);
        }
    }
}