﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Languages
{
    public class LanguageUpdateRequest
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}
