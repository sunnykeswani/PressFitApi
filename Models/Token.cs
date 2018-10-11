using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string TokenId { get; set; }
    }
}