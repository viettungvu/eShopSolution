using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace eShopSolution.Data.Entites
{
    [Table("AppConfigs")]
    public class AppConfig
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
