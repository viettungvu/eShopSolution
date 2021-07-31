using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Common
{
    public class ConfirmBase
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsShowConfirm { get; set; }
    }
}