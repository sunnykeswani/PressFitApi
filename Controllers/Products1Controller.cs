﻿using PressFitApi.Models;
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
using System.Net.Http;
using System.Net.Http.Headers;
using PushSharp.Apple;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Configuration;

namespace PressFitApi.Controllers
{
    [Authorize]
    public class Products1Controller : Controller
    {
        private PressFitApiContext db = new PressFitApiContext();

        //[HttpPost]
        //public void Files(HttpPostedFileBase file)
        //{
        //    //return View(db.Products.ToList());
        //    try
        //    {
        //        if (file.ContentLength > 0)
        //        {
        //            string fileName = Path.GetFileName(file.FileName);
        //            string filePath = Path.Combine(Server.MapPath("~/FilesUpload"), fileName);
        //            file.SaveAs(filePath);
        //            ViewBag.Message = "File Uploaded Successfully!!";
        //        }
        //    }
        //    catch (System.Exception)
        //    {

        //        throw;
        //    }
        //}

        // GET: Products1
        public ActionResult Index()
        {
            try
            {
                // return View(db.Product.OrderBy(x => x.HighPriority ? 0 : 1).ToList());
                var count = db.Product.OrderBy(x => x.PriorityNumber).ToList().Count;

                var productSizeList = GetProductsWithSize(db.Product.ToList());

                // return View(db.Product.OrderBy(x => x.PriorityNumber).ToList());
                return View(productSizeList);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private object GetProductsWithSize(List<Product> product)
        {

            try
            {
                List<Product> lstProductSize = new List<Product>();

                foreach (var item in product)
                {
                    if (item.FileName != null)
                    {
                        Product objProduct = new Product();
                        var pdfPath = Server.MapPath("~/PdfUploads/" + item.FileName + ".pdf");
                        var imagePath = Server.MapPath("~/ImageUploads/" + item.FileName + ".png");
                        float pdfLengthBytes = new System.IO.FileInfo(pdfPath).Length;
                        float imgLengthBytes = new System.IO.FileInfo(imagePath).Length;

                        float pdfLengthMB = pdfLengthBytes / (1024 * 1024);
                        float imgLengthMB = imgLengthBytes / (1024 * 1024);
                        objProduct = item;
                        objProduct.PdfSize = Math.Round(pdfLengthMB, 2);
                        objProduct.ImageSize = Math.Round(imgLengthMB, 2);
                        lstProductSize.Add(objProduct);
                    }

                }
                return lstProductSize.OrderBy(x => x.PriorityNumber);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //  var pdfPath = Server.MapPath("~/PdfUploads");
            //  var imagePath = Server.MapPath("~/ImageUploads");
        }

        // GET: Products1/Details/5
        public ActionResult Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
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
        public ActionResult Create([Bind(Include = "Id,Title,FileName,SearchTags,HighPriority,PdfUpload,ImageUpload,PriorityNumber,IsMobileUploaded")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // check for existing product
                    bool productExist = db.Product.ToList().Any(x => x.FileName.ToLower().Equals(product.FileName.ToLower()));

                    if (!productExist)
                    {

                        //***** searchtags in lower ****
                        product.SearchTags = product.SearchTags.ToString().ToLower();
                        product.CreatedDate = DateTime.Now.ToString();
                        product.ModifiedDate = DateTime.Now.ToString();
                        // product.PriorityNumber = int.MaxValue;

                        db.Product.Add(product);
                        db.SaveChanges();
                        bool printFlag = false;
                        //****** to save files******
                        foreach (string file in Request.Files)
                        {
                            if (file == "PrintPdfUpload")
                            {
                                printFlag = true;
                            }
                            HttpPostedFileBase uploadedFile = Request.Files[file];
                            SaveFile(uploadedFile, ref product, printFlag);
                            //Save file here
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "File Name already exist";
                        ModelState.Clear();
                        //return RedirectToAction("Create");
                    }
                }

                return View(product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveFile(HttpPostedFileBase file, ref Product product, bool flag)
        {
            try
            {

                var pdfPath = Server.MapPath("~/PdfUploads");
                var imagePath = Server.MapPath("~/ImageUploads");
                var oldFileName = string.Empty;
                var newFileName = string.Empty;

                if (file.ContentLength > 0 && file.ContentType.Contains("pdf"))
                {
                    if (flag)
                    {
                        pdfPath = Server.MapPath("~/PrintPdfUploads");
                        oldFileName = Path.GetFileName("print_" + file.FileName);
                        newFileName = "print_" + product.FileName.ToString() + ".pdf";
                    }
                    else
                    {
                        oldFileName = Path.GetFileName(file.FileName);
                        newFileName = product.FileName.ToString() + ".pdf";
                    }
                    SaveInFolder(pdfPath, oldFileName, newFileName, file);
                }
                else if (file.ContentType.Contains("image"))
                {
                    oldFileName = Path.GetFileName(file.FileName);
                    newFileName = product.FileName.ToString() + ".png";
                    SaveInFolder(imagePath, oldFileName, newFileName, file);
                }


                //var pdfPath = Server.MapPath("~/PdfUploads");
                //if (file.ContentType.Contains("pdf"))
                //{
                //    if (flag)
                //    {
                //        pdfPath = Server.MapPath("~/PrintPdfUploads");
                //    }
                //}


                //var printPdfPath = Server.MapPath("~/PrintPdfUploads");

                //if (!Directory.Exists(pdfPath))
                //{
                //    Directory.CreateDirectory(pdfPath);
                //}

                //if (!Directory.Exists(imagePath))
                //{
                //    Directory.CreateDirectory(imagePath);
                //}

                //*******To upload print pdf file ******
                //if (flag)
                //{
                //    if (file.ContentLength > 0 && file.ContentType.Contains("pdf"))
                //    {

                //        //var oldFileName = Path.GetFileName("print_" + file.FileName);
                //        // var newFileName = "print_" + product.FileName.ToString() + ".pdf";

                //        //var oldpath = Path.Combine(Server.MapPath("~/PdfUploads"), oldFileName);
                //        //var newpath = Path.Combine(Server.MapPath("~/PdfUploads"), newFileName);

                //        var oldpath = Path.Combine(pdfPath, oldFileName);
                //        var newpath = Path.Combine(pdfPath, newFileName);

                //        DeleteExistFile(oldpath);
                //        DeleteExistFile(newpath);
                //        file.SaveAs(oldpath);

                //        System.IO.File.Move(oldpath, newpath);
                //    }
                //}
                //else if (file.ContentLength > 0 && file.ContentType.Contains("pdf"))
                //{

                // var oldFileName = Path.GetFileName(file.FileName);
                //var newFileName = product.FileName.ToString() + ".pdf";

                //var oldpath = Path.Combine(Server.MapPath("~/PdfUploads"), oldFileName);
                //var newpath = Path.Combine(Server.MapPath("~/PdfUploads"), newFileName);

                //    var oldpath = Path.Combine(pdfPath, oldFileName);
                //    var newpath = Path.Combine(pdfPath, newFileName);

                //    DeleteExistFile(oldpath);
                //    DeleteExistFile(newpath);
                //    file.SaveAs(oldpath);

                //    System.IO.File.Move(oldpath, newpath);
                //}
                //else if (file.ContentLength > 0 && file.ContentType.Contains("image"))
                //{


                //    var oldFileName = Path.GetFileName(file.FileName);
                //    var newFileName = product.FileName.ToString() + ".png";

                //    var oldpath = Path.Combine(Server.MapPath("~/ImageUploads"), oldFileName);
                //    var newpath = Path.Combine(Server.MapPath("~/ImageUploads"), newFileName);

                //    DeleteExistFile(oldpath);
                //    DeleteExistFile(newpath);
                //    file.SaveAs(oldpath);

                //    System.IO.File.Move(oldpath, newpath);

                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveInFolder(string filePath, string oldFile, string NewFile, HttpPostedFileBase file)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var oldpath = Path.Combine(filePath, oldFile);
            var newpath = Path.Combine(filePath, NewFile);

            DeleteExistFile(oldpath);
            DeleteExistFile(newpath);
            file.SaveAs(oldpath);

            System.IO.File.Move(oldpath, newpath);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Products1/Edit/5
        public ActionResult Edit(int? id)
        {
            try
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
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,FileName,SearchTags,HighPriority,PdfUpload,ImageUpload,PriorityNumber,IsMobileUploaded")] Product product)
        {
            try
            {

                Boolean currentHiddenvalue = Convert.ToBoolean(Request.Form["hiddenValue"]);
                //****** to save files******
                //bool printFlag = Convert.ToBoolean(Request.Form["hiddenPrintPdfValue"]);
                bool printFlag = false;

                //***** to check & save pdf files ****
                foreach (string file in Request.Files)
                {
                    if (file == "PrintPdfUpload")
                    {
                        printFlag = true;
                    }
                    HttpPostedFileBase uploadedFile = Request.Files[file];
                    if (uploadedFile.ContentLength != 0)
                    {
                        SaveFile(uploadedFile, ref product, printFlag);
                    }
                    //Save file here
                }

                product.ModifiedDate = DateTime.Now.ToString();

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                if (currentHiddenvalue)
                {
                    //var objProduct = db.Product.Select(i=>i).ToList();
                    Broadcast(product);
                }

                return RedirectToAction("Index");
                // }
                //return View(product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Broadcast(Product product)
        {
            try
            {

                SendIOSNotification(product);
                SendAndroidNotification(product);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void SendAndroidNotification(Product product)
        {
            try
            {
                string tokenIds = getAndroidTokenId();
                HttpClient client = new HttpClient();

                client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/send");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                string key = "AIzaSyCeWSgk4KlqsoqIMA0XRuSs___jN2lMy4I";

                client.DefaultRequestHeaders.Add("Authorization", " key = AIzaSyCeWSgk4KlqsoqIMA0XRuSs___jN2lMy4I");
                // var jsonObject = "{\"registration_ids\":[\"fP1x1T4AQYo:APA91bEu_aEVl-kv8PoUtUTTAykVX_-45XsB_b_bvBR5SArTLERX3mAM0-Yp369tRNUK4T3lvx5ohfkwLHYITqznIJDBns4X-HzCuDcqb_0XeFzRUT9pep38QiN0sTx_A4vty8_aaVdt\",\"fRjyHmcLdaM:APA91bGXK0ggaPrebWjeqlpGp2HLMm36hYEPGCj1hpkzEeikdBAw_lDOb-oid3ExtTYYzX20KIokZofPSAvffDDv7WW5oa3LZfdvsw7Zsgea6GuJ9oUYoHMsxvqMgUc8R15UkasLD2Cz\"],\"collapse_key\":\"type_a\",\"content_available\":true,\"mutable-content \":true,\"data\":{\"body\":\"New Diwali Diyas\",\"message\":\"Diwali offers\",\"url\":\"https://static1.squarespace.com/static/55705e90e4b03651f8736427/t/557062f6e4b0e9b175a7d533/1536068419238/?format=1500w\"}}";

                var jsonObject = "{\"registration_ids\":[" + tokenIds + "],\"collapse_key\":\"type_a\",\"content_available\":true,\"mutable-content \":true,\"data\":{\"message\":\"The price lists of " + product.Title + " has been changed.\",\"body\":\"Press Fit Price Lists\",\"url\":\"" + string.Empty + "\"}}";
                var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(client.BaseAddress, content).Result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void SendIOSNotification(Product product)
        {
            try
            {
                string[] TokenIds = getIOSTokenId();
                foreach (var deviceToken in TokenIds)
                {

                    //8FA978DB52FC2B09D79F039C9928BF4867A01B106689232F8D75234F37D04396


                    //Get Certificate
                    var certicatePath = (ConfigurationManager.AppSettings["ProdCertificate"]);
                    var appleCert = System.IO.File.ReadAllBytes(Server.MapPath(certicatePath));
                    //var appleCert = System.IO.File.ReadAllBytes(Server.MapPath("~/Files/APNS_DEV_Certificates.p12"));


                    // Configuration (NOTE: .pfx can also be used here)
                    var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, appleCert, "");
                    //var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, appleCert, "");


                    // Create a new broker
                    var apnsBroker = new ApnsServiceBroker(config);

                    // Wire up events
                    apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
                    {
                        aggregateEx.Handle(ex =>
                        {
                            // See what kind of exception it was to further diagnose
                            if (ex is ApnsNotificationException)
                            {
                                var notificationException = (ApnsNotificationException)ex;

                                // Deal with the failed notification
                                var apnsNotification = notificationException.Notification;
                                var statusCode = notificationException.ErrorStatusCode;
                                string desc = $"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}";
                                //               Console.WriteLine(desc);
                                //lblStatus.Text = desc;
                                //return  new HttpStatusCodeResult(HttpStatusCode.OK);
                            }
                            else
                            {
                                string desc = $"Apple Notification Failed for some unknown reason : {ex.InnerException}";
                                // Inner exception might hold more useful information like an ApnsConnectionException			
                                Console.WriteLine(desc);
                                //lblStatus.Text = desc;
                            }

                            // Mark it as handled
                            return true;
                        });
                    };

                    apnsBroker.OnNotificationSucceeded += (notification) =>
                    {
                        //lblStatus.Text = "Apple Notification Sent successfully!";

                        var test = notification;
                    };

                    var fbs = new FeedbackService(config);
                    fbs.FeedbackReceived += (string devicToken, DateTime timestamp) =>
                    {
                        // Remove the deviceToken from your database
                        // timestamp is the time the token was reported as expired
                    };

                    // Start Proccess 
                    apnsBroker.Start();
                    var jsonObject = JObject.Parse("{\"aps\":{\"alert\":{\"body\":\"The price lists of " + product.Title + " has been changed.\",\"title\":\"Press Fit Price Lists\"},\"mutable-content\":1},\"sound\":\"default\",\"media-url\":\"" + string.Empty + "\"}");
                    //var jsonObject = "{\"data\":{\"body\":\"" + broadcastModel.Message + "\",\"message\":\"" + broadcastModel.Title + "\",\"url\":\"" + httpPath + "\"}}";
                    //var jsonObject = JObject.Parse(("{\"aps\":{\"badge\":1,\"sound\":\"oven.caf\",\"alert\":\"" + (broadcastModel.Message + "\"}}")));
                    if (deviceToken != "")
                    {
                        apnsBroker.QueueNotification(new ApnsNotification
                        {
                            DeviceToken = deviceToken,
                            Payload = jsonObject
                            //Payload = JObject.Parse(("{\"aps\":{\"badge\":1,\"sound\":\"oven.caf\",\"alert\":\"" + (message + "\"}}")))
                        });
                    }

                    apnsBroker.Stop();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string[] getIOSTokenId()
        {

            string[] token_array = db.Token.Where(y => y.ChannelId.ToLower() == "ios").Select(x => x.TokenId).ToList().Select(i => i.ToString()).ToArray();
            // string tokenIds = string.Join(",", token_array.Select(f => "\"" + f + "\""));
            string tokenIds = string.Join(",", token_array);

            return token_array;
        }
        private string getAndroidTokenId()
        {
            string[] token_array = db.Token.Where(y => y.ChannelId.ToLower() == "android").Select(x => x.TokenId).ToList().Where(e => e != null).Select(i => i.ToString()).ToArray();
            string tokenIds = string.Join(",", token_array.Select(f => "\"" + f + "\""));
            //string tokenIds = string.Join(",", token_array);

            return tokenIds;
        }
        // GET: Products1/Delete/5
        public ActionResult Delete(int? id)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }



        // POST: Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product product = db.Product.Find(id);

                DeleteFile(product);
                db.Product.Remove(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private void DeleteFile(Product product)
        {
            try
            {

                var pdfPath = Server.MapPath("~/PdfUploads");
                var printPdfPath = Server.MapPath("~/PrintPdfUploads");
                var imagePath = Server.MapPath("~/ImageUploads");


                var pdfFileName = product.FileName.ToString() + ".pdf";
                var printPdfFileName = "print_" + product.FileName.ToString() + ".pdf";
                var imageFileName = product.FileName.ToString() + ".png";

                var pdfFilePath = Path.Combine(Server.MapPath("~/PdfUploads"), pdfFileName);
                var printPdfFilePath = Path.Combine(Server.MapPath("~/PrintPdfUploads"), printPdfFileName);
                var imageFilePath = Path.Combine(Server.MapPath("~/ImageUploads"), imageFileName);

                DeleteExistFile(pdfFilePath);
                DeleteExistFile(printPdfFilePath);
                DeleteExistFile(imageFilePath);

            }
            catch (Exception ex)
            {

                throw ex;
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
                throw ex;
            }
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


        // GET: Products1/Delete/5
        public ActionResult FileEdit(int? id)
        {
            try
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
                return View("FileEdit", product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult FileEditName([Bind(Include = "Id,FileName")] Product product)
        {
            try
            {

                var pdfPath = Server.MapPath("~/PdfUploads");
                var printPdfPath = Server.MapPath("~/PrintPdfUploads");

                string updatedFileName = product.FileName;

                Product objProduct = db.Product.Find(product.Id);
                string previousFileName = objProduct.FileName;
                objProduct.FileName = updatedFileName;
                //****** to save files******

                string pdfFilePath = Server.MapPath("~/PdfUploads/" + previousFileName + ".pdf");

                string updatedFilePath = Server.MapPath("~/PdfUploads/" + updatedFileName + ".pdf");
                string printPdfFilePath = Server.MapPath("~/PrintPdfUploads/print_" + previousFileName + ".pdf");
                string printUpdatedFilePath = Server.MapPath("~/PrintPdfUploads/print_" + updatedFileName + ".pdf");
                string imageFilePath = Server.MapPath("~/ImageUploads/" + previousFileName + ".png");
                string imageUpdatedFilePath = Server.MapPath("~/ImageUploads/" + updatedFileName + ".png");

                System.IO.File.Move(pdfFilePath, updatedFilePath);
                System.IO.File.Move(printPdfFilePath, printUpdatedFilePath);
                System.IO.File.Move(imageFilePath, imageUpdatedFilePath);

                db.Entry(objProduct).State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("Index");
                // }
                //return View(product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }


}
