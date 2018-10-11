using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    public class ProductBanner
    {
        public List<Product> ProductList { get; set; }
        public string[] Banner { get; set; }
    }
}