using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShopSolution.Data.Entites
{

    [Table("AppRoles")]
    public class AppRole:IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}
