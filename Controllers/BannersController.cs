using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PressFitApi.Controllers
{
    [Authorize]
    public class BannersController : Controller
    {
        // GET: Banners
        public ActionResult Index()
        {


            var bannerPath = Server.MapPath("~/BannerUploads");

            if (!Directory.Exists(bannerPath))
            {
                Directory.CreateDirectory(bannerPath);
            }

            string[] filePaths = Directory.GetFiles(Server.MapPath("~/BannerUploads"));
            string[] files = new string[filePaths.Length];


            for (int i = 0; i < filePaths.Length; i++)
            {
                //string fileNameWithoutPath = info.Name;

                FileInfo info = new FileInfo(filePaths[i]);
                string fileNameWithoutPath = info.Name;
                files[i] = info.Name;

            }
            //FileInfo info = new FileInfo(filePaths);

            return View(files);
        }

        [HttpGet, ActionName("Create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase[] files)
        {


            var uploadedFilesPath = Server.MapPath("~/BannerUploads");

            if (!Directory.Exists(uploadedFilesPath))
            {
                Directory.CreateDirectory(uploadedFilesPath);
            }


            foreach (HttpPostedFileBase item in files)
            {
                //Checking file is available to save.  
                if (item != null)
                {
                    SaveFile(item);
                    // var InputFileName = Path.GetFileName(file.FileName);
                    //var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                    //Save file to server folder  
                    // file.SaveAs(ServerSavePath);
                    //assigning file uploaded status to ViewBag for showing message to user.  
                }

            }
            ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";

            //return View("Index");
            return RedirectToAction("Index");
        }

        public void SaveFile(HttpPostedFileBase uploadFile)

        {
            try
            {
                if (uploadFile.ContentLength > 0)
                {
                    var oldFileName = Path.GetFileName(uploadFile.FileName);
                    string[] filenames = oldFileName.Split('.');
                    var newFileName = filenames[0] + ".png";

                    var oldpath = Path.Combine(Server.MapPath("~/BannerUploads"), oldFileName);
                    var newpath = Path.Combine(Server.MapPath("~/BannerUploads"), newFileName);

                    DeleteExistFile(oldpath);
                    DeleteExistFile(newpath);
                    uploadFile.SaveAs(oldpath);

                    System.IO.File.Move(oldpath, newpath);


                    //string _FileName = Path.GetFileName(uploadFile.FileName);
                    //string _path = Path.Combine(Server.MapPath("~/BannerUploads"), _FileName);
                    //uploadFile.SaveAs(_path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        private void SaveBanners(HttpPostedFileBase file)
        {
            try
            {

                var pdfPath = Server.MapPath("~/PdfUploads");
                var imagePath = Server.MapPath("~/ImageUploads");

                if (!Directory.Exists(pdfPath))
                {
                    Directory.CreateDirectory(pdfPath);
                }

                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                }

                if (file.ContentLength > 0 && file.ContentType.Contains("pdf"))
                {

                    var oldFileName = Path.GetFileName(file.FileName);
                    //var newFileName = product.FileName.ToString() + ".pdf";

                    var oldpath = Path.Combine(Server.MapPath("~/PdfUploads"), oldFileName);
                    //var newpath = Path.Combine(Server.MapPath("~/PdfUploads"), newFileName);

                    DeleteExistFile(oldpath);
                    // DeleteExistFile(newpath);
                    file.SaveAs(oldpath);

                    // System.IO.File.Move(oldpath, newpath);




                    //Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(myfile, newName);

                    // file.SaveAs(path);
                }
                else if (file.ContentLength > 0 && file.ContentType.Contains("image"))
                {


                    var oldFileName = Path.GetFileName(file.FileName);
                    // var newFileName = product.FileName.ToString() + ".png";

                    var oldpath = Path.Combine(Server.MapPath("~/ImageUploads"), oldFileName);
                    //var newpath = Path.Combine(Server.MapPath("~/ImageUploads"), newFileName);

                    DeleteExistFile(oldpath);
                    // DeleteExistFile(newpath);
                    file.SaveAs(oldpath);

                    // System.IO.File.Move(oldpath, newpath);

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void DeleteExistFile(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        // GET: Products1/Delete/5

        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewData["Image"] = id;

            return View();
        }

        //[HttpGet]
        //public ActionResult Delete(int? id)
        //{
        //        if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    return View();
        //}
        //public ActionResult Delete(string id)
        //{

        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    string path = Server.MapPath("~/BannerUploads/"+id+".png");
        //    DeleteExistFile(path);
        //    //Product product = db.Product.Find(id);

        //    return View("Index");
        //}

        // POST: Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string path = Server.MapPath("~/BannerUploads/" + id + ".png");
            DeleteExistFile(path);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewData["Image"] = id;

            return View();
        }

        // POST: Products1/Delete/5
        [HttpPost, ActionName("Edit")]
        public ActionResult EditFile(string id)
        {
            //SaveFile(id);
            string newFileName = id + ".png";
            var newpath = Path.Combine(Server.MapPath("~/BannerUploads"), newFileName);
            DeleteExistFile(newpath);

            HttpPostedFileBase file = Request.Files[0];
            SaveFile(file);
            return RedirectToAction("Index");
        }

        public ActionResult Create(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewData["Image"] = id;

            return View();
        }



    }
}