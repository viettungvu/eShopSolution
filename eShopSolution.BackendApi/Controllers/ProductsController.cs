using eShopSolution.Application.Catalog.Products;
using eShopSolution.Application.Common;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IStorageService _storageService;

        public ProductsController(IProductService productService, IStorageService storageService)
        {
            _productService = productService;
            _storageService = storageService;
        }

        [HttpPost("new")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _productService.Create(request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Created(nameof(GetById), result.ResultObject);
        }

        [HttpPatch("{productId}/update")]
        public async Task<IActionResult> Update(int productId, [FromForm] ProductUpdateRequest request)
        {
            var result = await _productService.Update(productId, request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpPatch("{productId}/newPrice")]
        public async Task<IActionResult> UpdatePrice(int productId, [FromQuery] decimal newPrice)
        {
            var result = await _productService.UpdatePrice(productId, newPrice);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpPatch("{productId}/stock")]
        public async Task<IActionResult> UpdateStock(int productId, [FromQuery] int addedStock)
        {
            var result = await _productService.UpdateStock(productId, addedStock);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpPatch("{productId}/view")]
        public async Task<IActionResult> UpdateViewCount(int productId)
        {
            var result = await _productService.UpdateViewCount(productId);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var result = await _productService.Delete(productId);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpGet("{languageId}")]
        public async Task<IActionResult> GetAllPaging(string languageId, [FromQuery] ProductPagingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _productService.GetAllPaging(languageId, request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetByCategoryId(int categoryId, [FromQuery] ProductPagingRequest request)
        {
            var result = await _productService.GetByCategoryId(categoryId, request);
            if (!result.IsSuccessed)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var result = await _productService.GetById(productId, languageId);
            if (!result.IsSuccessed)
                return NotFound();
            return Ok(result);
        }

        [HttpPost("{productId}/addImage")]
        public async Task<IActionResult> AddImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            var result = await _productService.AddImage(productId, request);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpDelete("{productId}/image/{imageId}")]
        public async Task<IActionResult> DeleteImage(int productId, int imageId)
        {
            var result = await _productService.RemoveImage(productId, imageId);
            if (!result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpPut("{productId}/image/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            var result = await _productService.UpdateImage(imageId, request);
            if (result.IsSuccessed)
                return BadRequest();
            return Ok(result);
        }

        [HttpGet("image/{imageId}")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            var image = await _productService.GetImageById(imageId);
            if (!image.IsSuccessed)
                return NotFound();
            return Ok(image);
        }
    }
}