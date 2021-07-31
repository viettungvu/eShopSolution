using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Roles
{
    public class RolePagingRequest : PagingRequestBase
    {
        public string RoleName { get; set; }
    }
}