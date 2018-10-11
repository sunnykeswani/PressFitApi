using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    public class Broadcast
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public HttpPostedFileBase ImageUrl { get; set; }
    }
}