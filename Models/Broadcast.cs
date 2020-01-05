using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    public class Broadcast
    {
        [Required(ErrorMessage = "Max length is 400")]
        [MaxLength(400)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Max length is 1400")]
        [MaxLength(1400)]
        public string Message { get; set; }
        
    }
}