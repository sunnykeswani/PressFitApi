using PressFitApi.Models;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PressFitApi.Controllers
{
    [Authorize]
    public class Products1Controller : Controller
    {
        private PressFitApiContext db = new PressFitApiContext();

        [HttpPost]
        public void Files(HttpPostedFileBase file)
        {
            //return View(db.Products.ToList());
            try
            {
                if (file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/FilesUpload"), fileName);
                    file.SaveAs(filePath);
                    ViewBag.Message = "File Uploaded Successfully!!";
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        // GET: Products1
        public ActionResult Index()
        {

            return View(db.Product.OrderBy(x => x.HighPriority ? 0 : 1).ToList());
        }

        // GET: Products1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,FileName,SearchTags,HighPriority,PdfUpload,ImageUpload")] Product product)
        {


            if (ModelState.IsValid)
            {

                //***** searchtags in lower ****
                product.SearchTags = product.SearchTags.ToString().ToLower();
                product.CreatedDate = DateTime.Now.ToString();
                product.ModifiedDate = DateTime.Now.ToString();

                db.Product.Add(product);
                db.SaveChanges();

                //****** to save files******
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase uploadedFile = Request.Files[file];
                    SaveFile(uploadedFile, ref product);
                    //Save file here
                }
                return RedirectToAction("Index");
            }

            return View(product);
        }

        private void SaveFile(HttpPostedFileBase file, ref Product product)
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
                    var newFileName = product.FileName.ToString() + ".pdf";

                    var oldpath = Path.Combine(Server.MapPath("~/PdfUploads"), oldFileName);
                    var newpath = Path.Combine(Server.MapPath("~/PdfUploads"), newFileName);

                    DeleteExistFile(oldpath);
                    DeleteExistFile(newpath);
                    file.SaveAs(oldpath);

                    System.IO.File.Move(oldpath, newpath);




                    //Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(myfile, newName);

                    // file.SaveAs(path);
                }
                else if (file.ContentLength > 0 && file.ContentType.Contains("image"))
                {


                    var oldFileName = Path.GetFileName(file.FileName);
                    var newFileName = product.FileName.ToString() + ".png";

                    var oldpath = Path.Combine(Server.MapPath("~/ImageUploads"), oldFileName);
                    var newpath = Path.Combine(Server.MapPath("~/ImageUploads"), newFileName);

                    DeleteExistFile(oldpath);
                    DeleteExistFile(newpath);
                    file.SaveAs(oldpath);

                    System.IO.File.Move(oldpath, newpath);

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

        // GET: Products1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,SearchTags,FileName,HighPriority")] Product product)
        {
            if (ModelState.IsValid)
            {

                //****** to save files******
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase uploadedFile = Request.Files[file];
                    SaveFile(uploadedFile, ref product);
                    //Save file here
                }

                product.ModifiedDate = DateTime.Now.ToString();
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }



        // POST: Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);

            DeleteFile(product);
            db.Product.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        private void DeleteFile(Product product)
        {
            try
            {

                var pdfPath = Server.MapPath("~/PdfUploads");
                var imagePath = Server.MapPath("~/ImageUploads");



                var pdfFileName = product.Id.ToString() + ".pdf";
                var imageFileName = product.Id.ToString() + ".png";

                var pdfFilePath = Path.Combine(Server.MapPath("~/PdfUploads"), pdfFileName);
                var imageFilePath = Path.Combine(Server.MapPath("~/ImageUploads"), imageFileName);

                DeleteExistFile(pdfFilePath);
                DeleteExistFile(imageFilePath);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void DeleteBanners(string Id)
        {
            try
            {

                var pdfPath = Server.MapPath("~/PdfUploads");
                var imagePath = Server.MapPath("~/ImageUploads");



                var bannerName = Id.ToString() + ".pdf";



                var filePath = Path.Combine(Server.MapPath("~/BannerUploads"), bannerName);


                DeleteExistFile(filePath);


            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static void ImageToPNGConverter()

        {

            Image bmpImageToConvert = Image.FromFile("C:/Users/santosh.a.keswani/Pictures/sample3.jpg");

            Image bmpNewImage = new Bitmap(bmpImageToConvert.Width,

                                           bmpImageToConvert.Height);

            Graphics gfxNewImage = Graphics.FromImage(bmpNewImage);

            gfxNewImage.DrawImage(bmpImageToConvert,

                                  new Rectangle(0, 0, bmpNewImage.Width,

                                                bmpNewImage.Height),

                                  0, 0,

                                  bmpImageToConvert.

                                  Width, bmpImageToConvert.Height,

                                  GraphicsUnit.Pixel);

            gfxNewImage.Dispose();

            bmpImageToConvert.Dispose();



            /*bmpNewImage.Save(Server.MapPath("userData/" &

              Request.QueryString("ID") & "/" & e.Item.Cells(2).Text &

              ".jpg"),ImageFormat.Jpeg)

             */



            bmpNewImage.Save("C:/Users/santosh.a.keswani/Pictures/Saved Pictures/sample3t.png", ImageFormat.Png);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public void UpdatePriority(int id)
        {
            try
            {
                Product product = new Product();
                product = db.Product.Find(id);
                product.HighPriority = !product.HighPriority;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
            //return string.Format("Invalid file type. File Types supported are ");
        }


       
    }

    public class ValidateFileAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return false;
            }

            if (file.ContentLength > 3 * 1024 * 1024)
            {
                return false;
            }

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return (img.RawFormat.Equals(ImageFormat.Png) || img.RawFormat.Equals(ImageFormat.Jpeg));
                }
            }
            catch { }
            return false;
        }
    }


    public class FileTypesAttribute : ValidationAttribute
    {
        private readonly List<string> _types;

        public FileTypesAttribute(string types)
        {
            _types = types.Split(',').ToList();
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var fileExt = System.IO.Path.GetExtension((value as HttpPostedFile).FileName).Substring(1);
            return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Invalid file type. File Types supported are ", String.Join(", ", _types));
        }
       
        //[Route("Test/{name}")]

    }
}
