using eShopSolution.Data.EF;
using eShopSolution.Ultilities.Exceptions;
using eShopSolution.ViewModels.Catalog.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using eShopSolution.Data.Entites;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModels.Common;

namespace eShopSolution.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly EShopDbContext _context;

        public CategoryService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> Create(CategoryCreateRequest request)
        {
            var category = _context.CategoryTranslations.FirstOrDefault(x => x.Name == request.Name);
            if (category != null)
                return new ApiErrorResult<bool>("Danh muc da ton tai");
            var newCategory = new Category()
            {
                IsShowOnHome = request.IsShowOnHome,
                SortOrder = request.SortOrder,
                //Status=request.Status,
                CategoryTranslations = new List<CategoryTranslation>()
                {
                    new CategoryTranslation(){
                        Name=request.Name,
                        SeoAlias=request.SeoAlias,
                        SeoDescription=request.SeoDescription,
                        SeoTitle=request.SeoTitle,
                        LanguageId=request.LanguageId
                    }
                }
            };
            _context.Categories.Add(newCategory);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể tạo mới danh mục");
        }

        public async Task<ApiResult<bool>> Delete(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                return new ApiErrorResult<bool>("Khong tim thay danh muc");
            _context.Categories.Remove(category);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể xóa danh mục");
        }

        public async Task<ApiResult<List<CategoryVm>>> GetAll()
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        select new { c, ct };
            if (query == null)
                return new ApiErrorResult<List<CategoryVm>>("Null");
            var categories = await query.Select(x => new CategoryVm()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                SeoAlias = x.ct.SeoAlias,
                SeoDescription = x.ct.SeoDescription,
                SeoTitle = x.ct.SeoTitle,
                LanguageId = x.ct.LanguageId,
                SortOrder = x.c.SortOrder,
                IsShowOnHome = x.c.IsShowOnHome
            }).ToListAsync();
            return new ApiSuccessResult<List<CategoryVm>>(categories);
        }

        public async Task<ApiResult<PagedResult<CategoryVm>>> GetAllPaging(CategoryPagingRequest request)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        select new { c, ct };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.ct.Name.Contains(request.Keyword));
            }
            //3 Paging
            int totalRow = await query.CountAsync();
            var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CategoryVm()
                {
                    Id = x.c.Id,
                    Name = x.ct.Name,
                    IsShowOnHome = x.c.IsShowOnHome,
                    SortOrder = x.c.SortOrder,
                    SeoAlias = x.ct.SeoAlias,
                    SeoDescription = x.ct.SeoDescription,
                    SeoTitle = x.ct.SeoTitle,
                    LanguageId = x.ct.LanguageId
                }).ToListAsync();
            //4. Select
            var pagedResult = new PagedResult<CategoryVm>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = totalRow,
                ListItems = await data
            };
            if (data != null)
                return new ApiSuccessResult<PagedResult<CategoryVm>>(pagedResult);
            return new ApiErrorResult<PagedResult<CategoryVm>>("null");
        }

        public async Task<ApiResult<CategoryVm>> GetById(int categoryId)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where c.Id == categoryId
                        select new { c, ct };
            var category = await query.Select(x => new CategoryVm()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                SeoAlias = x.ct.SeoAlias,
                SeoDescription = x.ct.SeoDescription,
                SeoTitle = x.ct.SeoTitle,
                LanguageId = x.ct.LanguageId,
                SortOrder = x.c.SortOrder,
                IsShowOnHome = x.c.IsShowOnHome
            }).FirstOrDefaultAsync();
            if (category != null)
                return new ApiSuccessResult<CategoryVm>(category);
            return new ApiErrorResult<CategoryVm>("Not found");
        }

        public async Task<ApiResult<bool>> Update(int categoryId, CategoryUpdateRequest request)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        select new { c, ct };
            var category = await query.FirstOrDefaultAsync(x => x.c.Id == categoryId);
            var categoryTranslation = await _context.CategoryTranslations.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
            if (categoryTranslation == null)
                return new ApiErrorResult<bool>("Danh mục không tồn tại");
            category.c.IsShowOnHome = request.IsShowOnHome;
            category.c.SortOrder = request.SortOrder;
            categoryTranslation.Name = request.Name;
            categoryTranslation.SeoAlias = request.SeoAlias;
            categoryTranslation.SeoDescription = request.SeoDescription;
            categoryTranslation.SeoTitle = request.SeoTitle;
            categoryTranslation.LanguageId = request.LanguageId;
            _context.Update(categoryTranslation);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Không thể Cập nhật danh mục");
        }
    }
}