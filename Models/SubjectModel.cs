using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PressFitApi.Models
{
    public class SubjectModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Email")]
        //[EmailAddress]
        public string EmailId { get; set; }

    }
}