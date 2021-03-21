using eShopSolution.Data.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class ProductInCartConfiguration : IEntityTypeConfiguration<ProductInCart>
    {
        public void Configure(EntityTypeBuilder<ProductInCart> builder)
        {
            builder.HasKey(e => new { e.CartID, e.ProductID });

            builder.HasOne(e => e.Product)
                .WithMany(e => e.ProductInCarts)
                .HasForeignKey(e => e.ProductID);

            builder.HasOne(e => e.Cart)
                .WithMany(e => e.ProductInCarts)
                .HasForeignKey(e => e.CartID);
        }
    }
}
