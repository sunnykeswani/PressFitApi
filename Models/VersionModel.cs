using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    public class VersionModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string IOSVersion { get; set; }

        [Required]
        public string AndriodVersion { get; set; }

        //[Required]
        public Boolean ForceFulAndriodUpdate { get; set; }

        //[Required]
        public Boolean ForceFulIOSUpdate { get; set; } 

        public DateTime UpdatedOn { get; set; }

        //public VersionModel()
        //{
        //    ForceFulAndriodUpdate = false;
        //    ForceFulIOSUpdate = false;
        //}



    }
}