using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PressFitApi.Models;
using System.IO;


namespace PressFitApi.Controllers
{
    public class UserRequestController : Controller
    {
        private PressFitApiContext db = new PressFitApiContext();

        // GET: UserRequest
        public ActionResult Index()
        {

            List<KeyValuePair<string, string>> lstKeyValue = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> keyValue = new KeyValuePair<string, string>();
            string[] filePaths = Directory.GetFiles(Server.MapPath("~/ImageStorage"));

            foreach (string file in filePaths)
            {
                var filename = Path.GetFileName(file);
                string id = filename.Split('_')[0];
                keyValue = new KeyValuePair<string, string>(id, filename);
                lstKeyValue.Add(keyValue);
            }
            ViewBag.lstValues = lstKeyValue.ToList().OrderByDescending(x=>x.Key).ToList();


            //ViewData["Files"] = filePaths.ToList();
            //ViewBag.filePaths = filePaths.ToList().Where(x=>x.StartsWith(""));
            //string[] files = new string[filePaths.Length];


            //for (int i = 0; i < filePaths.Length; i++)
            //{
            //    //string fileNameWithoutPath = info.Name;

            //    FileInfo info = new FileInfo(filePaths[i]);
            //    string fileNameWithoutPath = info.Name;
            //    files[i] = info.Name;

            //}

            return View(db.ContactUs.ToList().OrderByDescending(x => x.Id));
        }

        // GET: UserRequest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            return View(contactUs);
        }

        // GET: UserRequest/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,MobileNo,State,City,Message,Subject")] ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                db.ContactUs.Add(contactUs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactUs);
        }

        // GET: UserRequest/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            return View(contactUs);
        }

        // POST: UserRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,MobileNo,State,City,Message,Subject")] ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactUs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactUs);
        }

        // GET: UserRequest/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUs contactUs = db.ContactUs.Find(id);
            if (contactUs == null)
            {
                return HttpNotFound();
            }
            return View(contactUs);
        }

        // POST: UserRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactUs contactUs = db.ContactUs.Find(id);
            db.ContactUs.Remove(contactUs);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
