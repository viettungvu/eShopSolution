using eShopSolution.Application.Catalog.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos.Manage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request );
        Task<int> Update(ProductUpdateRequest request);
        Task<int> UpdatePrice(int productId, decimal newPrice);
        Task<int> UpdateStock(int productId, int stock);
        Task UpdateViewCount(int productId);
        Task<int> Delete(int productId);
        Task<PagedResult<ProductViewModel>> GetAllPaging(ProductPagingRequest request);


    }
}
