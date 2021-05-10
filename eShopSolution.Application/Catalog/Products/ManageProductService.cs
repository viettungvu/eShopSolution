using eShopSolution.Data.EF;
using eShopSolution.Data.Entites;
using eShopSolution.Ultilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModels.Catalog.Common;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext _context;
        private readonly IStorageService _storageService;
        private string USER_CONTENT_FOLDER_NAME = "user-content";
        public ManageProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
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
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        Path = await SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
            _context.Products.Add(product);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslations = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId);
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
            var product = await _context.Products.FindAsync(productId);
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
            var image = _context.ProductImages.Where(x => x.ProductId == productId);
            foreach (var img in image)
            {
                _context.ProductImages.Remove(img);
                await _storageService.DeleteFileAsync(img.Path);
            }
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }
        public async Task<PagedResult<ProductViewModel>> GetAllPaging(ManageProductPagingRequest request)
        {
            //1. Select
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.Id
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductID
                        join c in _context.Categories on pic.CategoryID equals c.Id
                        select new { p, pt, pic };

            //2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
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
        public async Task<int> AddImage(ProductImageCreateRequest request)
        {
            var productImage = new ProductImage()
            {
                Caption = request.Caption,
                SortOrder = request.SortOrder,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                ProductId = request.ProductId,
            };
            if (request.ImageFile != null)
            {
                productImage.Path = await SaveFile(request.ImageFile);
            }
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();
            return productImage.Id;
        }
        public async Task<bool> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                throw new EShopException($"Can not find image {imageId}");
            }
            if (request.ImageFile != null)
            {
                image.FileSize = request.ImageFile.Length;
                // image.Path = await this.SaveFile(request.ImageFile);
            }
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<int> RemoveImage(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                throw new EShopException($"Can not find image {imageId}");
            }
            _context.ProductImages.Remove(image);
            return await _context.SaveChangesAsync();
        }
        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                throw new EShopException($"Can not find image {imageId}");
            }
            var productImageVm = new ProductImageViewModel()
            {
                Id = image.Id,
                Caption = image.Caption,
                Path = image.Path,
                DateCreated = image.DateCreated,
                IsDefault = image.IsDefault,
                FileSize = image.FileSize,
                SortOrder = image.SortOrder,
            };
            return productImageVm;
        }
        public async Task<List<ProductImage>> GetListImages(int productId)
        {
            var listImage = _context.ProductImages.Where(x => x.ProductId == productId);
            return await listImage.ToListAsync();
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
        }
        public async Task<List<ProductViewModel>> GetLatestProduct(string languageId, int take)
        {
            //1. Select
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.Id
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductID
                        into ppic
                        from pic in ppic.DefaultIfEmpty()
                        join pi in _context.ProductImages on p.Id equals pi.ProductId into ppi
                        from pi in ppi.DefaultIfEmpty()
                        join c in _context.Categories on pic.CategoryID equals c.Id into picc
                        from c in picc.DefaultIfEmpty()
                        where pt.LanguageId == languageId && (pi == null || pi.IsDefault == true)
                        select new { p, pt, pic, pi };

            var data = query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    ThumbnailImage = x.pi.Path
                }).ToListAsync();
            return await data;
        }
        
    }
}
