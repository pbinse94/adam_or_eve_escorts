using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Common
{
    public class CountryModel
    {
        public string? Country { get; set; }
    }

    public class CategoryViewModel
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public bool IsSelected { get; set; } // To track if the category is selected
    }
}
