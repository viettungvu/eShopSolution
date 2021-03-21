using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShopSolution.Data.Entites
{
    [Table("ProductInCarts")]
    public class ProductInCart
    {
        public int ProductID { get; set; }
        public Product Product { get; set; }
        public int CartID { get; set; }
        public Cart Cart { get; set; }
    }
}
