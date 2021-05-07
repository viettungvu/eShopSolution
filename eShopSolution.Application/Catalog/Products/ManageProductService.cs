using eShopSolution.Application.Catalog.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos;
using eShopSolution.Application.Catalog.Products.Dtos.Manage;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entites;
using eShopSolution.Ultilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace eShopSolution.Application.Catalog.Products.DTOs.Manage
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext _context;
        public ManageProductService(EShopDbContext context)
        {
            _context = context;
        }
        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation(){
                        Name=request.Name,
                        Description=request.Description,
                        Details=request.Details,
                       SeoDescription=request.SeoDescription,
                       SeoAlias=request.SeoAlias,
                       SeoTitle=request.SeoTitle,
                       LanguageId=request.LanguageId
                    }
                }
            };
            _context.Products.Add(product);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product =await _context.Products.FindAsync(request.Id);
            var productTranslations =await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId==request.LanguageId);
            if (product == null || productTranslations == null)
            {
                throw new EShopException($"Cant find a product {request.Id}");
            }
            else
            {
                productTranslations.Name = request.Name;
                productTranslations.Description = request.Description;
                productTranslations.Details = request.Details;
                productTranslations.SeoDescription = request.SeoDescription;
                productTranslations.SeoAlias = request.SeoAlias;
                productTranslations.SeoTitle = request.SeoTitle;
                productTranslations.LanguageId = request.LanguageId;
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<int> UpdatePrice(int productId, decimal newPrice)
        {
            var product =await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new EShopException($"Cannot find a product {productId}");
            }
            else
            {
                product.Price = newPrice;
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<int> UpdateStock(int productId, int addedStock)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                throw new EShopException($"Failed to update stock with error: 'Cannot find a product {productId}'");
            }
            else
            {
                product.Stock += addedStock;
            }
            return await _context.SaveChangesAsync();
        }
        public async Task UpdateViewCount(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                throw new EShopException($"Failed to update viewcount with error: 'Cannot find a product {productId}'");
            }
            else
            {
                product.ViewCount += 1;
            }
            await _context.SaveChangesAsync();
        }
        public async Task<int> Delete(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                throw new EShopException($"Failed to delete with error: 'Cannot find a product {productId}'");
            }
            else
            {
                _context.Products.Remove(product);
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<PagedResult<ProductViewModel>> GetAllPaging(ProductPagingRequest request)
        {
            //1. Select
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.Id
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductID
                        join c in _context.Categories on pic.CategoryID equals c.Id
                        select new { p, pt, pic };

            //2. Filter
            if (!String.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            }
            if (request.CategoryIds.Count > 0)
            {
                query = query.Where(p => request.CategoryIds.Contains(p.pic.CategoryID));
            }

            //3 Paging
            int totalRow = await query.CountAsync();
            var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    SeoAlias = x.pt.SeoAlias,
                    SeoTitle = x.pt.SeoTitle,
                    SeoDescription = x.pt.SeoDescription,
                    Stock = x.p.Stock,
                    Price = x.p.Price,
                    OriginalPrice = x.p.OriginalPrice,
                    ViewCount = x.p.ViewCount,
                    LanguageId = x.pt.LanguageId,
                }).ToListAsync();
            //4. Select
            var pagedResult = new PagedResult<ProductViewModel>()
            {
                TotalRecord = totalRow,
                ListItems = await data
            };
            return pagedResult;
        }
    }
}
