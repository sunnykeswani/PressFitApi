using PressFitApi.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PressFitApi.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Search Tags")]
        public string SearchTags { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string CreatedDate { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ModifiedDate { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string UpdatedBy { get; set; }


        //[ValidateFile(ErrorMessage = "Please select a PNG image smaller than 3MB")]
        //[Required]
        [NotMapped]
        [Required]
        public HttpPostedFileBase ImageUpload { get; set; }

        [Required]
        //[FileTypes("pdf")]
        [NotMapped]
        public HttpPostedFileBase PdfUpload { get; set; }

        [Required]
        //[RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        public string ImageUrl { get; set; }

        public string PdfUrl { get; set; }

        public Boolean HighPriority { get; set; }

        [DefaultValue(int.MaxValue)]
        [Display(Name = "Priority Number")]
        public int PriorityNumber { get; set; }



    }
}