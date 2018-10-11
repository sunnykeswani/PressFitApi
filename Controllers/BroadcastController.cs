using PressFitApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PressFitApi.Controllers
{
    [Authorize]
    public class BroadcastController : Controller
    {
        // GET: Broadcast
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(Broadcast broadcastModel)
        {
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase uploadedFile = Request.Files[file];
                SaveFile(uploadedFile);
                //SaveFile(uploadedFile, ref product);
                //Save file here
            }
            return View("Index");
        }

        private void SaveFile(HttpPostedFileBase file)
        {
            try
            {

                var broadcastPath = Server.MapPath("~/App_Data/BroadcastUploads");


                if (!Directory.Exists(broadcastPath))
                {
                    Directory.CreateDirectory(broadcastPath);
                }



                if (file.ContentLength > 0 && file.ContentType.Contains("image"))
                {
                    var newFileName = "1" + ".jpeg";
                    var newpath = Path.Combine(Server.MapPath("~/App_Data/BroadcastUploads"), newFileName);

                    DeleteExistFile(broadcastPath);
                    file.SaveAs(newpath);

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
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private  void Broadcast(string json)
        {

        }
    }
}