using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    public class Broadcast
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        
    }
}