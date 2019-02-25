using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    
    public class ContactUs
    {
        [NotMapped]
        [Required]
        public string Name { get; set; }
        [Required]
        [NotMapped]
        [EmailAddress]
        public string Email { get; set; }

        
        [NotMapped]
        [Phone]
        public string MobileNo { get; set; }

        [NotMapped]
        public string State { get; set; }

        [NotMapped]
        public string City { get; set; }


        [NotMapped]
        public string Message { get; set; }

        [Required]
        [NotMapped]
        public string Subject { get; set; }


    }
}