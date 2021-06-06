using eShopSolution.Application.Catalog.Products;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("new")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var productId = await _productService.Create(request);
            var product = await _productService.GetById(productId, request.LanguageId);
            if (productId == 0)
            {
                return BadRequest();
            }
            return Created(nameof(GetById), product);
        }/*DONE*/

        [HttpPatch("{productId}")]
        public async Task<IActionResult> Update(int productId, [FromForm] ProductUpdateRequest request)
        {
            var result = await _productService.Update(productId, request);
            if (result == 0)
                return BadRequest();
            return Ok("Successful");
        }/*DONE*/

        [HttpPatch("{productId}/{newPrice:decimal}")]
        public async Task<IActionResult> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _productService.UpdatePrice(productId, newPrice);
            if (product == 0)
                return BadRequest("Update price failed");
            return Ok("Successful");
        }/*DONE*/

        [HttpPatch("stock/{productId}/{addedStock:int}")]
        public async Task<IActionResult> UpdateStock(int productId, int addedStock)
        {
            var product = await _productService.UpdateStock(productId, addedStock);
            if (product == 0)
                return BadRequest("Update stock failed");
            return Ok("Successful");
        }/*DONE*/

        [HttpPatch("view")]
        public async Task<IActionResult> UpdateViewCount(int productId)
        {
            var result = await _productService.UpdateViewCount(productId);
            if (!result)
                return BadRequest("Update view count failed");
            return Ok("Successful");
        }/*DONE*/

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var result = await _productService.Delete(productId);
            if (result == 0)
                return BadRequest("Can not delete");
            return Ok(result);
        }/*DONE*/

        [HttpGet("{languageId}")]
        public async Task<IActionResult> GetAllPaging(string languageId, [FromQuery] ProductPagingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var product = await _productService.GetAllPaging(languageId, request);
            return Ok(product);
        }/*DONE*/

        //[HttpGet("list")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var products = await _productService.GetAll();
        //    return Ok(products);
        //}/*DONE*/

        [HttpGet("all/{languageId}")]
        public async Task<IActionResult> GetByCategoryId(string languageId, [FromQuery] ProductPagingRequest request)
        {
            var product = await _productService.GetByCategoryId(languageId, request);
            return Ok(product);
        }/*DONE*/

        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await _productService.GetById(productId, languageId);
            if (product == null)
            {
                return NotFound($"Not found product {productId}");
            }
            return Ok(product);
        }/*DONE*/

        [HttpPost("{productId}/image")]
        public async Task<IActionResult> AddImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            var imageId = await _productService.AddImage(productId, request);
            var image = await _productService.GetImageById(imageId);
            if (imageId == 0)
                return BadRequest("Failed");
            return Created(nameof(GetImageById), image);
        }

        [HttpDelete("{productId}/image/{imageId}")]
        public async Task<IActionResult> DeleteImage(int productId, int imageId)
        {
            var result = await _productService.RemoveImage(productId, imageId);
            if (!result)
                return BadRequest("Failed");
            return Ok("Deleted successful");
        }

        [HttpPut("{productId}/image/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            var result = await _productService.UpdateImage(imageId, request);
            if (!result)
                return BadRequest();
            return Ok();
        }

        [HttpGet("{productId}/image/{imageId}")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            var image = await _productService.GetImageById(imageId);
            if (image == null)
                return NotFound();
            return Ok(image);
        }
    }
}