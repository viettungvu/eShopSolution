
using eShopSolution.Data.Entites;
using eShopSolution.ViewModels.Catalog.Common;
using eShopSolution.ViewModels.Catalog.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request );
        Task<int> Update(ProductUpdateRequest request);
        Task<int> Delete(int productId);
        Task<int> UpdatePrice(int productId, decimal newPrice);
        Task<int> UpdateStock(int productId, int stock);
        Task UpdateViewCount(int productId);
        Task<PagedResult<ProductViewModel>> GetAllPaging(ManageProductPagingRequest request);
        Task<int> AddImage(ProductImageCreateRequest request);
        Task<bool> UpdateImage(int imageId, ProductImageUpdateRequest request);
        Task<int> RemoveImage(int imageId);
        Task<ProductImageViewModel> GetImageById(int imageId);
        Task<List<ProductImage>> GetListImages(int productId);
        Task<List<ProductViewModel>> GetLatestProduct(string languageId, int take);
        
    }
}
