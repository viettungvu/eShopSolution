using eShopSolution.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductTranslation> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.Language)
                .WithMany(e => e.ProductTranslations)
                .HasForeignKey(e => e.LanguageId);
            builder.HasOne(e => e.Product)
                .WithMany(e => e.ProductTranslations)
                .HasForeignKey(e => e.ProductId);
        }
    }
}
