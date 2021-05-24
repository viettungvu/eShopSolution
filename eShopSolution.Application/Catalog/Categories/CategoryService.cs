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

namespace eShopSolution.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly EShopDbContext _context;

        public CategoryService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(CategoryCreateRequest request)
        {
            var category = _context.CategoryTranslations.FirstOrDefault(x => x.Name == request.Name);
            if (category != null)
                throw new EShopException($"Category {request.Name} da ton tai");
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
            await _context.SaveChangesAsync();
            return newCategory.Id;
        }

        public async Task<bool> Delete(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
                throw new EShopException("Category not found");
            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<List<CategoryViewModel>> GetAll()
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        select new { c, ct };
            if (query == null)
                throw new EShopException("Empty");
            var categories = query.Select(x => new CategoryViewModel()
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
            return categories;
        }

        public async Task<CategoryViewModel> GetById(int categoryId, string languageId)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId && c.Id == categoryId
                        select new { c, ct };
            var category = query.Select(x => new CategoryViewModel()
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
            return await category;
        }

        public async Task<bool> Update(int categoryId, CategoryUpdateRequest request)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        select new { c, ct };
            var category = query.FirstOrDefault(x => x.c.Id == categoryId);
            if (category == null)
                throw new EShopException("Not found");
            category.c.IsShowOnHome = request.IsShowOnHome;
            category.c.SortOrder = request.SortOrder;
            category.ct.Name = request.Name;
            category.ct.SeoAlias = request.SeoAlias;
            category.ct.SeoDescription = request.SeoDescription;
            category.ct.SeoTitle = request.SeoTitle;
            category.ct.LanguageId = request.LanguageId;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}