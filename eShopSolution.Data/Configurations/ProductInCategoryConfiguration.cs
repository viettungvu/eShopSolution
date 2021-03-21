using eShopSolution.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.HasKey(e => new { e.ProductID, e.CategoryID });

            builder.HasOne(e => e.Category)
                .WithMany(e => e.ProductInCategories)
                .HasForeignKey(e => e.CategoryID);


            builder.HasOne(e => e.Product)
                .WithMany(e => e.ProductInCategories)
                .HasForeignKey(e => e.ProductID);
        }
    }
}
