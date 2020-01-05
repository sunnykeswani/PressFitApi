using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PressFitApi.Controllers
{
    public class UploadBannerController : Controller
    {
        // GET: UploadBanner
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult UploadMainBanner(HttpPostedFileBase file)
        {
//C:\Users\Santosh\Documents\Visual Studio 2015\Projects\PressFitApi\PressFitApi\Content\Images\

            var uploadedFilesPath = Server.MapPath("~/HomeBanner");

            if (!Directory.Exists(uploadedFilesPath))
            {
                Directory.CreateDirectory(uploadedFilesPath);
            }



            //Checking file is available to save.  
            if (file != null)
            {
                var path = Path.Combine(Server.MapPath("~/HomeBanner"), "PressFitImg.jpg");
                //var newpath = Path.Combine(Server.MapPath("~/HomeBanner"), "PressFitImg.jpg");

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                file.SaveAs(path);

                //System.IO.File.Move(oldpath, newpath);
            }


            ViewBag.UploadStatus = "Banner uploaded successfully.";

            //return View("Index");
            return RedirectToAction("Index");
        }
    }
}