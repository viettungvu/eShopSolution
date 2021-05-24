using eShopSolution.Data.Entites;
using eShopSolution.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace eShopSolution.Data.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppConfig>().HasData(
                new AppConfig() { Key = "HomeTitle", Value = "This is home page of eShopSolution" },
                new AppConfig() { Key = "HomeKeyword", Value = "This is keyword of eShopSolution" },
                new AppConfig() { Key = "HomeDescription", Value = "This is description of eShopSolution" }
                );
            modelBuilder.Entity<Language>().HasData(
                new Language() { Id = "vi-Vi", Name = "vi-Vietnam", IsDefault = true },
                new Language() { Id = "en-US", Name = "English", IsDefault = false }
                );
            modelBuilder.Entity<Category>().HasData(
                new Category() { Id = 1, SortOrder = 1, Status = Status.Active, IsShowOnHome = true },
                new Category() { Id = 2, SortOrder = 2, Status = Status.Active, IsShowOnHome = true },
                new Category() { Id = 3, SortOrder = 1, Status = Status.Active, IsShowOnHome = true },
                new Category() { Id = 4, SortOrder = 2, Status = Status.Active, IsShowOnHome = true }
                );
            modelBuilder.Entity<CategoryTranslation>().HasData(
                new CategoryTranslation() { Id = 1, CategoryId = 1, Name = "Danh mục 1", LanguageId = "vi-Vi", SeoAlias = "Danh mục 1", SeoDescription = "Danh mục 1", SeoTitle = "Danh mục 1" },
                new CategoryTranslation() { Id = 2, CategoryId = 1, Name = "Category 1", LanguageId = "en-US", SeoAlias = "Category 1", SeoDescription = "Category 1", SeoTitle = "Category 1" },
                new CategoryTranslation() { Id = 3, CategoryId = 2, Name = "Danh mục 2", LanguageId = "vi-Vi", SeoAlias = "Danh mục 2", SeoDescription = "Danh mục 2", SeoTitle = "Danh mục 2" },
                new CategoryTranslation() { Id = 4, CategoryId = 2, Name = "Category 2", LanguageId = "en-US", SeoAlias = "Category 2", SeoDescription = "Category 2", SeoTitle = "Category 2" },
                new CategoryTranslation() { Id = 5, CategoryId = 3, Name = "Danh mục 3", LanguageId = "vi-Vi", SeoAlias = "Danh mục 3", SeoDescription = "Danh mục 2", SeoTitle = "Danh mục 3" },
                new CategoryTranslation() { Id = 6, CategoryId = 4, Name = "Category 3", LanguageId = "en-US", SeoAlias = "Category 3", SeoDescription = "Category 3", SeoTitle = "Category 3" },
                new CategoryTranslation() { Id = 7, CategoryId = 4, Name = "Danh mục 4", LanguageId = "vi-Vi", SeoAlias = "Danh mục 4", SeoDescription = "Danh mục 4", SeoTitle = "Danh mục 4" }
                );
            modelBuilder.Entity<Product>().HasData(
                new Product()
                {
                    Id = 1,
                    DateCreated = DateTime.Now,
                    OriginalPrice = 100000,
                    Price = 200000,
                    Stock = 0,
                    ViewCount = 0,
                },
                new Product()
                {
                    Id = 2,
                    DateCreated = DateTime.Now,
                    OriginalPrice = 100000,
                    Price = 200000,
                    Stock = 0,
                   ViewCount = 0,
                },
                new Product()
                {
                    Id = 3,
                    DateCreated = DateTime.Now,
                    OriginalPrice = 100000,
                    Price = 200000,
                    Stock = 0,
                    ViewCount = 0,
                });
            modelBuilder.Entity<ProductTranslation>().HasData(
                 new ProductTranslation()
                 {
                     Id = 1,
                     ProductId = 1,
                     Name = "Sản phẩm 1",
                     LanguageId = "vi-Vi",
                     SeoAlias = "san-pham-1",
                     SeoDescription = "San pham 1",
                     SeoTitle = "San pham 1",
                     Details = "San pham 1",
                     Description = "San pham 1"
                 },
                 new ProductTranslation()
                 {
                     Id = 2,
                     ProductId = 1,
                     Name = "Product 1",
                     LanguageId = "en-US",
                     SeoAlias = "product-1",
                     SeoDescription = "Product 1",
                     SeoTitle = "Product 1",
                     Details = "Product 1",
                     Description = "Product 1"
                 },
                 new ProductTranslation()
                 {
                    Id = 3,
                     ProductId = 2,
                     Name = "Sản phẩm 2",
                     LanguageId = "vi-Vi",
                     SeoAlias = "san-pham-2",
                     SeoDescription = "San pham 2",
                     SeoTitle = "San pham 2",
                     Details = "San pham 2",
                     Description = "San pham 2"
                 },
                  new ProductTranslation()
                  {
                     Id = 4,
                      ProductId = 2,
                      Name = "Product 2",
                      LanguageId = "en-US",
                      SeoAlias = "product-2",
                      SeoDescription = "Product 2",
                      SeoTitle = "Product 2",
                      Details = "Product 2",
                      Description = "Product 2"
                  },
                 new ProductTranslation()
                 {
                     Id = 5,
                     ProductId = 3,
                     Name = "Sản phẩm 3",
                     LanguageId = "vi-Vi",
                     SeoAlias = "san-pham-3",
                     SeoDescription = "San pham 3",
                     SeoTitle = "San pham 3",
                     Details = "San pham 3",
                     Description = "San pham 3"
                 },
                  new ProductTranslation()
                  {
                      Id = 6,
                      ProductId = 3,
                      Name = "Product 3",
                      LanguageId = "en-US",
                      SeoAlias = "product-3",
                      SeoDescription = "Product 3",
                      SeoTitle = "Product 3",
                      Details = "Product 3",
                      Description = "Product 3"
                  });
            modelBuilder.Entity<ProductInCategory>().HasData(
                new ProductInCategory() { ProductID = 1, CategoryID = 1 },
                new ProductInCategory() { ProductID = 1, CategoryID = 2 },
                new ProductInCategory() { ProductID = 2, CategoryID = 1 },
                new ProductInCategory() { ProductID = 2, CategoryID = 3 },
                new ProductInCategory() { ProductID = 2, CategoryID = 2 },
                new ProductInCategory() { ProductID = 3, CategoryID = 1 }
               );

            const string ADMIN_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
            // any guid, but nothing is against to use the same one
            const string ROLE_ID = ADMIN_ID;
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = new Guid(ROLE_ID),
                Name = "admin",
                NormalizedName = "admin"
            });


            var hasher = new PasswordHasher<AppUser>();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = new Guid(ADMIN_ID),
                UserName = "admin",
                NormalizedUserName = "admin",
                Email = "vi-Viettungtvhd@gmail.com",
                NormalizedEmail = "vi-Viettungtvhd@gmail.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "123456a"),
                SecurityStamp = string.Empty,
                FirstName = "Vu vi-Viet",
                LastName = "Tung",
                DoB = DateTime.Today
            }); ;

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = new Guid(ROLE_ID),
                UserId = new Guid(ADMIN_ID)
            });

        }
    }
}
