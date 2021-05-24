using eShopSolution.Data.Entites;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(int productId, ProductUpdateRequest request);

        Task<int> Delete(int productId);

        Task<int> UpdatePrice(int productId, decimal newPrice);

        Task<int> UpdateStock(int productId, int stock);

        Task<bool> UpdateViewCount(int productId);

        Task<ProductViewModel> GetById(int productId, string languageId);

        Task<PagedResult<ProductViewModel>> GetAllPaging(string languageId, ProductPagingRequest request);

        //Task<List<ProductViewModel>> GetAll();

        Task<List<ProductViewModel>> GetLatestProduct(string languageId, int take);

        Task<PagedResult<ProductViewModel>> GetByCategoryId(string languageId, ProductPagingRequest request);

        Task<int> AddImage(int productId, ProductImageCreateRequest request);

        Task<bool> UpdateImage(int imageId, ProductImageUpdateRequest request);

        Task<bool> RemoveImage(int productId, int imageId);

        Task<ProductImageViewModel> GetImageById(int imageId);

        Task<List<ProductImageViewModel>> GetListImages(int productId);
    }
}