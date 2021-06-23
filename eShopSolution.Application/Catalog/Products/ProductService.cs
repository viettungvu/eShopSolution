using eShopSolution.Data.EF;
using eShopSolution.Data.Entites;
using eShopSolution.Ultilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using eShopSolution.ViewModels.Common;

namespace eShopSolution.Application.Catalog.Products
{
    public class ProductService : IProductService
    {
        private readonly EShopDbContext _context;
        private readonly IStorageService _storageService;
        private string USER_CONTENT_FOLDER_NAME = "user-content";

        public ProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.Name == request.Name);
            if (productTranslation != null)
                throw new EShopException($"Product {request.Name} existed");
            var newProduct = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = request.ViewCount,
                DateCreated = request.DateCreated,
                ProductTranslations = new List<ProductTranslation>()
                    {
                       new ProductTranslation()
                       {
                           Name=request.Name,
                           SeoAlias=request.SeoAlias,
                           SeoDescription=request.SeoDescription,
                           SeoTitle=request.SeoTitle,
                           Description=request.Description,
                           Details=request.Details,
                           LanguageId=request.LanguageId
                       }
                    },
                ProductInCategories = new List<ProductInCategory>()
                    {
                        new ProductInCategory()
                        {
                            CategoryID=request.CategoryId,
                        }
                    }
            };
            if (request.ThumbnailImage != null)
            {
                newProduct.ProductImages = new List<ProductImage>()
                    {
                        new ProductImage()
                        {
                            Caption = "Thumbnail Image",
                            SortOrder = 1,
                            DateCreated = DateTime.Now,
                            IsDefault = true,
                            Path = await this.SaveFile(request.ThumbnailImage),
                            FileSize = request.ThumbnailImage.Length
                        }
                    };
            }
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return newProduct.Id;
        }

        public async Task<int> Update(int productId, ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(productId);
            var productTranslations = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId && x.LanguageId == request.LanguageId);
            if (product == null || productTranslations == null)
                throw new EShopException($"Cant find a product {productId}");
            productTranslations.Name = request.Name;
            productTranslations.Description = request.Description;
            productTranslations.Details = request.Details;
            productTranslations.SeoDescription = request.SeoDescription;
            productTranslations.SeoAlias = request.SeoAlias;
            productTranslations.SeoTitle = request.SeoTitle;
            productTranslations.LanguageId = request.LanguageId;
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<int> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new EShopException($"Not found {productId}");
            product.Price = newPrice;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateStock(int productId, int addedStock)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new EShopException($"Product not found {productId}'");
            product.Stock += addedStock;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new EShopException($"Product not found {productId}");
            product.ViewCount += 1;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new EShopException($"Failed to delete with error: 'Cannot find a product {productId}'");
            }
            var productTranslation = _context.ProductTranslations.FirstOrDefault(x => x.ProductId == product.Id);
            _context.ProductTranslations.Remove(productTranslation);
            var image = _context.ProductImages.Where(x => x.ProductId == productId);
            foreach (var img in image)
            {
                _context.ProductImages.Remove(img);
                await _storageService.DeleteFileAsync(img.Path);
            }
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<ProductViewModel>> GetAllPaging(string languageId, ProductPagingRequest request)
        {
            //1. Select
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductID
                        join c in _context.Categories on pic.CategoryID equals c.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        select new { p, pt, pic, pi };
            //2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));
            }
            if (languageId != null)
            {
                query = query.Where(p => languageId == p.pt.LanguageId);
            }
            if (request.CategoryId != null)
            {
                query = query.Where(p => request.CategoryId == p.pic.CategoryID);
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
                    ThumbnailImage = x.pi.Path
                }).ToListAsync();
            //4. Select
            var pagedResult = new PagedResult<ProductViewModel>()
            {
                TotalRecords = totalRow,
                ListItems = await data
            };
            return pagedResult;
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

        public async Task<ProductViewModel> GetById(int productId, string languageId)
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        select new { p, pt, pi };
            if (query == null)
                throw new EShopException($"Product not found {productId}");
            if (languageId != null)
                query = query.Where(x => x.p.Id == productId);
            query = query.Where(x => x.pt.LanguageId == languageId);
            var data = query.Select(x => new ProductViewModel()
            {
                Id = x.p.Id,
                Description = x.pt.Description,
                Details = x.pt.Details,
                SeoAlias = x.pt.SeoAlias,
                SeoDescription = x.pt.SeoDescription,
                SeoTitle = x.pt.SeoTitle,
                LanguageId = x.pt.LanguageId,
                ThumbnailImage = x.pi.Path,
                Stock = x.p.Stock,
                Price = x.p.Price,
                OriginalPrice = x.p.OriginalPrice,
                DateCreated = x.p.DateCreated,
                ViewCount = x.p.ViewCount,
                Name = x.pt.Name,
            }).FirstOrDefaultAsync();
            return await data;
        }

        public async Task<PagedResult<ProductViewModel>> GetByCategoryId(string lanugageId, ProductPagingRequest request)
        {
            //1. Select
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductID
                        join c in _context.Categories on pic.CategoryID equals c.Id
                        select new { p, pt, pic };
            //2. Filter
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(p => p.pic.CategoryID == request.CategoryId);
            }
            if (lanugageId != null)
            {
                query = query.Where(p => p.pt.LanguageId == lanugageId);
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
                TotalRecords = totalRow,
                ListItems = await data
            };
            return pagedResult;
        }

        //public async Task<List<ProductViewModel>> GetAll()
        //{
        //    var query = from p in _context.Products
        //                join pt in _context.ProductTranslations on p.Id equals pt.ProductId
        //                join pic in _context.ProductInCategories on p.Id equals pic.ProductID
        //                join c in _context.Categories on pic.CategoryID equals c.Id
        //                join pi in _context.ProductImages on p.Id equals pi.ProductId
        //                select new { p, pt, pic, pi };
        //    var data = await query.Select(x => new ProductViewModel()
        //    {
        //        Id = x.p.Id,
        //        Name = x.pt.Name,
        //        Description = x.pt.Description,
        //        Details = x.pt.Details,
        //        SeoAlias = x.pt.SeoAlias,
        //        SeoTitle = x.pt.SeoTitle,
        //        SeoDescription = x.pt.SeoDescription,
        //        Stock = x.p.Stock,
        //        Price = x.p.Price,
        //        OriginalPrice = x.p.OriginalPrice,
        //        ViewCount = x.p.ViewCount,
        //        LanguageId = x.pt.LanguageId,
        //        ThumbnailImage = x.pi.Path
        //    }).ToListAsync();
        //    return data;
        //}

        //Image APIs
        public async Task<int> AddImage(int productId, ProductImageCreateRequest request)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new EShopException($"Product {productId} is null");
            var newImage = new ProductImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                SortOrder = request.SortOrder,
                ProductId = productId
            };
            if (request.ImageFile != null)
            {
                newImage.Path = await this.SaveFile(request.ImageFile);
                newImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Add(newImage);
            await _context.SaveChangesAsync();
            return newImage.Id;
        }

        public async Task<bool> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                throw new EShopException($"Cannot find image {imageId}");
            if (request.ImageFile != null)
            {
                image.Caption = request.Caption;
                image.IsDefault = request.IsDefault;
                image.FileSize = request.ImageFile.Length;
                image.Path = await this.SaveFile(request.ImageFile);
            }
            _context.ProductImages.Update(image);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveImage(int productId, int imageId)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(x => x.ProductId == productId && x.Id == imageId);
            if (image == null)
                throw new EShopException($"Image not found {imageId}");
            _context.ProductImages.Remove(image);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                throw new EShopException($"Image not found {imageId}");
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
                ProductId = image.ProductId
            };
            return productImageVm;
        }

        public async Task<List<ProductImageViewModel>> GetListImages(int productId)
        {
            var images = await _context.ProductImages.Where(x => x.ProductId == productId)
                .Select(x => new ProductImageViewModel()
                {
                    Caption = x.Caption,
                    DateCreated = x.DateCreated,
                    Id = x.Id,
                    ProductId = x.ProductId,
                    SortOrder = x.SortOrder,
                    IsDefault = x.IsDefault
                }).ToListAsync();
            return images;
        }
    }
}