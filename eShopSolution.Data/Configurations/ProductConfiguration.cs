using eShopSolution.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Price).IsRequired();
            builder.Property(e => e.OriginalPrice).IsRequired();
            builder.Property(e => e.Stock).IsRequired().HasDefaultValue(0);
            builder.Property(e => e.ViewCount).IsRequired().HasDefaultValue(0);

        }
    }
}
