﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShopSolution.Data.Entites
{
    [Table("Products")]
    public class Product
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int Stock { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
        public string SeoAlias { get; set; }

        public List<ProductTranslation> ProductTranslations { get; set; }
        public List<ProductInCategory> ProductInCategories { get; set; }
        public List<ProductInCart> ProductInCarts { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
