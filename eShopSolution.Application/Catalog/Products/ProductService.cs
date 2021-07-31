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

        public ProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<ApiResult<bool>> Create(ProductCreateRequest request)
        {
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.Name == request.Name);
            if (productTranslation != null)
                return new ApiErrorResult<bool>("Sản phẩm đã tồn tại");
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
                            Path = await _storageService.UploadFileAsync(request.ThumbnailImage),
                            FileSize = request.ThumbnailImage.Length
                        }
                    };
            }
            _context.Products.Add(newProduct);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể tạo mới sản phẩm");
        }

        public async Task<ApiResult<bool>> Update(int productId, ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(productId);
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == productId && x.LanguageId == request.LanguageId);
            if (product == null || productTranslation == null)
                return new ApiErrorResult<bool>("Sản phẩm không tồn tại");
            productTranslation.Name = request.Name;
            productTranslation.Description = request.Description;
            productTranslation.Details = request.Details;
            productTranslation.SeoDescription = request.SeoDescription;
            productTranslation.SeoAlias = request.SeoAlias;
            productTranslation.SeoTitle = request.SeoTitle;
            productTranslation.LanguageId = request.LanguageId;
            _context.Update(productTranslation);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể cập nhật sản phẩm");
        }

        public async Task<ApiResult<bool>> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return new ApiErrorResult<bool>("Sản phẩm không tồn tại");
            product.Price = newPrice;
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể cập nhật giá sản phẩm");
        }

        public async Task<ApiResult<bool>> UpdateStock(int productId, int addedStock)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return new ApiErrorResult<bool>("Sản phẩm không tồn tại");
            product.Stock += addedStock;
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể cập nhật giá sản phẩm");
        }

        public async Task<ApiResult<bool>> UpdateViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return new ApiErrorResult<bool>("Sản phẩm không tồn tại");
            product.ViewCount += 1;
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể cập nhật sản phẩm");
        }

        public async Task<ApiResult<bool>> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return new ApiErrorResult<bool>("Sản phẩm không tồn tại");
            var productTranslation = _context.ProductTranslations.FirstOrDefault(x => x.ProductId == product.Id);
            _context.ProductTranslations.Remove(productTranslation);
            var image = _context.ProductImages.Where(x => x.ProductId == productId);
            foreach (var img in image)
            {
                _context.ProductImages.Remove(img);
                await _storageService.DeleteFileAsync(img.Path);
            }
            _context.Products.Remove(product);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể xóa sản phẩm");
        }

        public async Task<ApiResult<PagedResult<ProductVm>>> GetAllPaging(string languageId, ProductPagingRequest request)
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
            //3 Paging
            int totalRow = await query.CountAsync();
            var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductVm()
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
            var pagedResult = new PagedResult<ProductVm>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = totalRow,
                ListItems = await data
            };
            if (data != null)
                return new ApiSuccessResult<PagedResult<ProductVm>>(pagedResult);
            return new ApiErrorResult<PagedResult<ProductVm>>("null");
        }

        public async Task<ApiResult<List<ProductVm>>> GetLatestProduct(string languageId, int take)
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

            var data = await query.OrderByDescending(x => x.p.DateCreated).Take(take)
                .Select(x => new ProductVm()
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
            if (data != null)
                return new ApiSuccessResult<List<ProductVm>>(data);
            return new ApiErrorResult<List<ProductVm>>("null");
        }

        public async Task<ApiResult<ProductVm>> GetById(int productId, string languageId = "vi-vn")
        {
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        //join pic in _context.ProductInCategories on p.Id equals pic.ProductID
                        //join c in _context.Categories on pic.CategoryID equals c.Id
                        join pi in _context.ProductImages on p.Id equals pi.ProductId
                        select new { p, pt, pi };
            query = query.Where(x => x.p.Id == productId);
            if (!string.IsNullOrEmpty(languageId))
                query = query.Where(x => x.pt.LanguageId == languageId);
            if (query.Count() <= 0)
                return new ApiErrorResult<ProductVm>("Sản phẩm không tồn tại");
            var data = await query.Select(x => new ProductVm()
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
            if (data != null)
                return new ApiSuccessResult<ProductVm>(data);
            return new ApiErrorResult<ProductVm>("null");
        }

        public async Task<ApiResult<PagedResult<ProductVm>>> GetByCategoryId(int categoryId, ProductPagingRequest request)
        {
            //1. Select
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductID
                        join c in _context.Categories on pic.CategoryID equals c.Id
                        select new { p, pt, pic };
            //2. Filter
            if (categoryId > 0)
            {
                query = query.Where(p => p.pic.CategoryID == categoryId);
            }
            //if (lanugageId != null)
            //{
            //    query = query.Where(p => p.pt.LanguageId == lanugageId);
            //}
            //3 Paging
            int totalRow = await query.CountAsync();
            var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductVm()
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
            var pagedResult = new PagedResult<ProductVm>()
            {
                TotalRecords = totalRow,
                ListItems = await data
            };
            if (data != null)
                return new ApiSuccessResult<PagedResult<ProductVm>>(pagedResult);
            return new ApiErrorResult<PagedResult<ProductVm>>("null");
        }

        public async Task<ApiResult<bool>> AddImage(int productId, ProductImageCreateRequest request)
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
                newImage.Path = await _storageService.UploadFileAsync(request.ImageFile);
                newImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Add(newImage);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể thêm hình ảnh cho sản phẩm");
        }

        public async Task<ApiResult<bool>> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                return new ApiErrorResult<bool>("không tìm thấy ảnh");
            if (request.ImageFile != null)
            {
                image.Caption = request.Caption;
                image.IsDefault = request.IsDefault;
                image.FileSize = request.ImageFile.Length;
                image.Path = await _storageService.UploadFileAsync(request.ImageFile);
            }
            _context.ProductImages.Update(image);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể cập nhật hình ảnh sản phẩm");
        }

        public async Task<ApiResult<bool>> RemoveImage(int productId, int imageId)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(x => x.ProductId == productId && x.Id == imageId);
            if (image == null)
                return new ApiErrorResult<bool>("Không tìm thấy ảnh");
            _context.ProductImages.Remove(image);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể xóa hình ảnh sản phẩm");
        }

        public async Task<ApiResult<ProductImageVm>> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
                return new ApiErrorResult<ProductImageVm>("Ảnh không tồn tại");
            var productImageVm = new ProductImageVm()
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
            return new ApiSuccessResult<ProductImageVm>(productImageVm);
        }

        public async Task<ApiResult<List<ProductImageVm>>> GetListImages(int productId)
        {
            var images = await _context.ProductImages.Where(x => x.ProductId == productId)
                .Select(x => new ProductImageVm()
                {
                    Caption = x.Caption,
                    DateCreated = x.DateCreated,
                    Id = x.Id,
                    ProductId = x.ProductId,
                    SortOrder = x.SortOrder,
                    IsDefault = x.IsDefault
                }).ToListAsync();
            return new ApiSuccessResult<List<ProductImageVm>>(images);
        }
    }
}