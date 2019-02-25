using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PressFitApi.Models;
using System.IO;
using Newtonsoft.Json;

namespace PressFitApi.Controllers
{
    public class ProductsController : ApiController
    {
        private PressFitApiContext db = new PressFitApiContext();

        // GET: api/Products
        public IHttpActionResult GetProducts()
        {
            //return Ok(db.Product.OrderBy(x => x.HighPriority ? 1 : 0).ToList());
            return Ok(db.Product.OrderBy(x => x.PriorityNumber).ToList());
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Product.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }


        // POST: api/GetProductList
        //  [ResponseType(typeof(Product))]
        [Route("GetProductList")]
        [HttpPost]
        public IHttpActionResult GetProductList(Token token)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (token.DeviceId == null)
                {
                    return BadRequest();
                }

                // Token objToken = db.Token.Find(token.DeviceId);

                if (!db.Token.Any(o => o.DeviceId == token.DeviceId))
                {
                    db.Token.Add(token);
                    db.SaveChanges();
                }
                else
                {
                    //db.Token = token;
                    var tokenRecord = db.Token.Where(x => x.DeviceId == token.DeviceId).FirstOrDefault();
                    tokenRecord.ChannelId = token.ChannelId;
                    tokenRecord.TokenId = token.TokenId;
                    //  db.Entry(token).State = EntityState.Modified;
                    db.SaveChanges();
                }

                OldProductBanner objProductBanner = new OldProductBanner();
                objProductBanner.ProductList = db.Product.OrderByDescending(x => x.HighPriority).ToList();
                objProductBanner.Banner = new string[] { };
                objProductBanner.Banner = getBannersPath();

                getPdfModifiedDate(objProductBanner.ProductList);

                return Ok(objProductBanner);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [Route("GetNewProductList")]
        [HttpPost]

        public IHttpActionResult GetNewProductList(Token token)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (token.DeviceId == null)
                {
                    return BadRequest();
                }

                // Token objToken = db.Token.Find(token.DeviceId);

                if (!db.Token.Any(o => o.DeviceId == token.DeviceId))
                {
                    db.Token.Add(token);
                    db.SaveChanges();
                }
                else
                {
                    //db.Token = token;
                    var tokenRecord = db.Token.Where(x => x.DeviceId == token.DeviceId).FirstOrDefault();
                    tokenRecord.ChannelId = token.ChannelId;
                    tokenRecord.TokenId = token.TokenId;
                    //  db.Entry(token).State = EntityState.Modified;
                    db.SaveChanges();
                }

                ProductBanner objProductBanner = new ProductBanner();
                objProductBanner.ProductList = db.Product.OrderBy(x => x.PriorityNumber).ToList();
                objProductBanner.BannerList = new List<BannerList>();
                objProductBanner.ImageList = new List<ImageList>();

                //BannerList objBannerList = new BannerList();
                //List<BannerList> lstBannerList = new List<BannerList>();

                //lstBannerList.
                //objProductBanner.Banner = new string[] { };
                //objProductBanner.Banner = getBannersPath();
                objProductBanner.BannerList = getNewBannersPath();

                getPdfModifiedDate(objProductBanner.ProductList);
                //getBannerModifiedDate(objProductBanner.BannerList);
                getImageModifiedDate(objProductBanner.ProductList);

                //***** version comparison *****
                var lastVersionModel = db.VersionModel
                      .OrderByDescending(p => p.UpdatedOn)
                      .FirstOrDefault();

                //string UpdateStatus;

                if (token.ChannelId.ToLower() == "ios")
                {
                    objProductBanner.UpdateStatus = VersionMatch(lastVersionModel.IOSVersion, token.Version, lastVersionModel.ForceFulIOSUpdate, lastVersionModel.Id);
                }
                else
                {
                    objProductBanner.UpdateStatus = VersionMatch(lastVersionModel.AndriodVersion, token.Version, lastVersionModel.ForceFulAndriodUpdate, lastVersionModel.Id);
                }

                //string AndriodValue = VersionMatch(lastVersionModel.AndriodVersion, token.Version, lastVersionModel.ForceFulAndriodUpdate, lastVersionModel.Id);
                //getImageDate(objProductBanner);

                return Ok(objProductBanner);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //private void getImageDate(ProductBanner objProductBanner)
        //{

        //    try
        //    {
        //        var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/ImageUploads")).ToList();

        //        for (int i = 0; i < files.Count; i++)
        //        {
        //            ImageList objImageList = new ImageList();
        //            FileInfo fi = new FileInfo(files[i]);

        //            if (fi.Name.ToLower() == objProductBanner.ProductList[i].FileName.ToLower())
        //            {
        //                objProductBanner.ProductList[i].ImageModifiedDate = fi.LastWriteTime;
        //                objProductBanner.ProductList[i].ImageUrl = files[i].ToString();
        //            }
        //            //lstImageList.Add(objImageList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //private void getImageModifiedDate(List<ImageList> lstImageList)
        //{
        //    try
        //    {

        //        var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/ImageUploads")).ToList();

        //        for (int i = 0; i < files.Count; i++)
        //        {
        //            ImageList objImageList = new ImageList();
        //            FileInfo fi = new FileInfo(files[i]);
        //            objImageList.ImageModifiedDate = fi.LastWriteTime;
        //            objImageList.Image = files[i].ToString();
        //            lstImageList.Add(objImageList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        private void getImageModifiedDate(List<Product> productList)
        {
            try
            {
                var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/ImageUploads")).ToList();
                foreach (var item in files)
                {
                    FileInfo fi = new FileInfo(item);
                    var lastmodified = fi.LastWriteTime;
                    var filename = fi.Name.Replace(".png", "");

                    var flag = productList.Any(x => x.FileName == filename);

                    //if (flag)
                    //{
                    //    var modifiedDate=productList.Where(x => x.FileName == filename).ToDictionary(x => x.ModifiedDate) ;
                    //    MyObject found;
                    //    if (modifiedDate.TryGetValue(lastmodified, out found)) found.OtherProperty = newValue;
                    //}

                    if (flag)
                    {
                        foreach (var obj in productList)
                        {
                            if (obj.FileName == filename)
                            {
                                obj.ImageUrl = item.ToString();
                                obj.ImageModifiedDate = lastmodified;
                                break;
                            }
                        }
                    }

                    //productList.Where(x=>x.FileName)
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void getBannerModifiedDate(List<BannerList> lstBannerList)
        {
            try
            {
                var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/BannerUploads")).ToList();

                BannerList objBanner;
                //foreach (var item in files)
                //{
                //    FileInfo fi = new FileInfo(item);
                //    var lastmodified = fi.LastWriteTime;
                //}

                for (int i = 0; i < files.Count; i++)
                {
                    //objBanner = new BannerList();
                    FileInfo fi = new FileInfo(files[i]);
                    //objBanner.Banner = fi.ToString();
                    // objBanner.BannerModifiedDate = fi.LastWriteTime;
                    lstBannerList[i].BannerModifiedDate = fi.LastWriteTime;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void getPdfModifiedDate(List<Product> productList)
        {
            try
            {
                var files = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/PdfUploads")).ToList();
                foreach (var item in files)
                {
                    FileInfo fi = new FileInfo(item);
                    var lastmodified = fi.LastWriteTime;
                    var filename = fi.Name.Replace(".pdf", "");

                    var flag = productList.Any(x => x.FileName == filename);

                    //if (flag)
                    //{
                    //    var modifiedDate=productList.Where(x => x.FileName == filename).ToDictionary(x => x.ModifiedDate) ;
                    //    MyObject found;
                    //    if (modifiedDate.TryGetValue(lastmodified, out found)) found.OtherProperty = newValue;
                    //}

                    if (flag)
                    {
                        foreach (var obj in productList)
                        {
                            if (obj.FileName == filename)
                            {
                                obj.ModifiedDate = lastmodified.ToString();
                                break;
                            }
                        }
                    }

                    //productList.Where(x=>x.FileName)
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [Route("GetProductTest")]
        public IHttpActionResult GetProductTest()
        {


            return Ok(db.Product.ToList());
        }

        [HttpGet]
        [Route("GetPath")]
        public IHttpActionResult GetPath()
        {
            //return Server.MapPath("~/App_Data/Pdf Uploads");
            var mappedPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Pdf");

            return Ok(mappedPath);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Product.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Product.Count(e => e.Id == id) > 0;
        }


        private string[] getBannersPath()
        {
            var localPaths = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/BannerUploads")).Select(f => Path.GetFileName(f)).ToList();

            if (localPaths.Count > 0)
            {


                string[] filePaths = new string[localPaths.Count];

                for (int i = 0; i < localPaths.Count; i++)
                {
                    filePaths[i] = @Url.Content("~/BannerUploads/" + localPaths[i]);
                }
                //= Directory.GetFiles(@Url.Content("~/BannerUploads"));
                return filePaths;
            }
            else
            {
                return new string[0];
            }
        }


        private List<BannerList> getNewBannersPath()
        {
            BannerList objBannerList;
            List<BannerList> lstBannerList = new List<BannerList>();

            var localPaths = Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/BannerUploads")).Select(f => Path.GetFileName(f)).ToList();

            if (localPaths.Count > 0)
            {
                for (int i = 0; i < localPaths.Count; i++)
                {

                    objBannerList = new BannerList();
                    FileInfo fi = new FileInfo(localPaths[i]);
                    //filePaths[i] = @Url.Content("~/BannerUploads/" + localPaths[i]);
                    //                    string modifiedDate=localPaths[i].
                    objBannerList.Banner = @Url.Content("~/BannerUploads/" + localPaths[i]);
                    objBannerList.BannerModifiedDate = fi.LastWriteTime;
                    lstBannerList.Add(objBannerList);
                }
                //= Directory.GetFiles(@Url.Content("~/BannerUploads"));
                return lstBannerList;
            }
            else
            {
                return new List<BannerList>();
            }
        }

        public string VersionMatch(string version, string strLatestVersion, bool forcefulUpdate, int id)
        {

            string[] versionArray = version.Split('.');
            string[] currentVersionArray = strLatestVersion.Split('.');

            for (int i = 0; i < versionArray.Length; i++)
            {
                int currentVersion = Convert.ToInt16(currentVersionArray[i]);
                int latestVersion = Convert.ToInt16(versionArray[i]);

                if (currentVersion < latestVersion)
                {
                    // if (i == versionArray.Length - 1)
                    //{
                    if (forcefulUpdate)
                    {
                        return "f";
                    }
                    else
                    {
                        return "o";
                    }
                    // }
                }
                else if (currentVersion < latestVersion)
                {
                    return "401";
                }
                continue;

            }

            return string.Empty;
        }

        [Route("GetSubjects")]
        [HttpGet]
        public IHttpActionResult GetSubject()
        {
            var lstSubject = db.Subject.ToList();
            var subjectArray = new String[db.Subject.ToList().Count];


            for (int i = 0; i < lstSubject.Count; i++)
            {
                subjectArray[i] = lstSubject[i].Subject;
            }

            var json = JsonConvert.SerializeObject(subjectArray);

            return Ok(json);
        }
    }
}