using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    public class ProductBanner
    {
        public List<Product> ProductList { get; set; }
        // public string[] Banner { get; set; }
        //public DateTime BannerModifiedDate { get; set; }
        public List<BannerList> BannerList { get; set; }
        public List<ImageList> ImageList { get; set; }

        public string UpdateStatus { get; set; }
    }

    public class BannerList
    {
        public string Banner { get; set; }
        public DateTime BannerModifiedDate { get; set; }
    }

    public class ImageList
    {
        public string Image { get; set; }
        public DateTime ImageModifiedDate { get; set; }
    }

    public class OldProductBanner
    {
        public List<Product> ProductList { get; set; }
         public string[] Banner { get; set; }
        //public DateTime BannerModifiedDate { get; set; }
        
    }

}