using eShopSolution.Data.Entites;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IProductService
    {
        Task<ApiResult<bool>> Create(ProductCreateRequest request);

        Task<ApiResult<bool>> Update(int productId, ProductUpdateRequest request);

        Task<ApiResult<bool>> Delete(int productId);

        Task<ApiResult<bool>> UpdatePrice(int productId, decimal newPrice);

        Task<ApiResult<bool>> UpdateStock(int productId, int stock);

        Task<ApiResult<bool>> UpdateViewCount(int productId);

        Task<ApiResult<ProductVm>> GetById(int productId, string languageId);

        Task<ApiResult<PagedResult<ProductVm>>> GetAllPaging(string languageId, ProductPagingRequest request);

        Task<ApiResult<List<ProductVm>>> GetLatestProduct(string languageId, int take);

        Task<ApiResult<PagedResult<ProductVm>>> GetByCategoryId(int categoryId, ProductPagingRequest request);

        Task<ApiResult<bool>> AddImage(int productId, ProductImageCreateRequest request);

        Task<ApiResult<bool>> UpdateImage(int imageId, ProductImageUpdateRequest request);

        Task<ApiResult<bool>> RemoveImage(int productId, int imageId);

        Task<ApiResult<ProductImageVm>> GetImageById(int imageId);

        Task<ApiResult<List<ProductImageVm>>> GetListImages(int productId);
    }
}