using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PressFitApi.Models;
using System.Data.Entity.Migrations;

namespace PressFitApi.Controllers
{
    public class VersionController : Controller
    {
        private PressFitApiContext db = new PressFitApiContext();

        // GET: Version
        public ActionResult Index()
        {
            return View(db.VersionModel.ToList());
        }

        // GET: Version/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersionModel versionModel = db.VersionModel.Find(id);
            if (versionModel == null)
            {
                return HttpNotFound();
            }
            return View(versionModel);
        }

        // GET: Version/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Version/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IOSVersion,AndriodVersion,ForceFulAndriodUpdate,ForceFulIOSUpdate")] VersionModel versionModel)
        {
            if (ModelState.IsValid)
            {
                versionModel.UpdatedOn = DateTime.Now;
                db.VersionModel.Add(versionModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(versionModel);
        }

        // GET: Version/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersionModel versionModel = db.VersionModel.Find(id);
            if (versionModel == null)
            {
                return HttpNotFound();
            }
            return View(versionModel);
        }

        // POST: Version/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IOSVersion,AndriodVersion,ForceFulAndriodUpdate,ForceFulIOSUpdate")] VersionModel versionModel)
        {
            if (ModelState.IsValid)
            {
                var lastVersionModel = db.VersionModel
                       .OrderByDescending(p => p.UpdatedOn)
                       .FirstOrDefault();
                //string previousVersion = db.Versions.Find(versionModel);
                string AndriodValue = VersionMatch(lastVersionModel.AndriodVersion, versionModel.AndriodVersion, versionModel.ForceFulAndriodUpdate, lastVersionModel.Id);
                string IOSValue = VersionMatch(lastVersionModel.IOSVersion, versionModel.IOSVersion, versionModel.ForceFulIOSUpdate, lastVersionModel.Id);
                versionModel.UpdatedOn = DateTime.Now;
                //db.Entry(versionModel).State = EntityState.Modified;
                db.Set<VersionModel>().AddOrUpdate(versionModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(versionModel);
        }

        // GET: Version/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VersionModel versionModel = db.VersionModel.Find(id);
            if (versionModel == null)
            {
                return HttpNotFound();
            }
            return View(versionModel);
        }

        // POST: Version/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VersionModel versionModel = db.VersionModel.Find(id);
            db.VersionModel.Remove(versionModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public string VersionMatch(string version, string strLatestVersion, bool forcefulUpdate, int id)
        {

            string[] versionArray = version.Split('.');
            string[] latestVersionArray = strLatestVersion.Split('.');

            for (int i = 0; i < versionArray.Length; i++)
            {
                int latestVersion = Convert.ToInt16(latestVersionArray[i]);
                int previousVersion = Convert.ToInt16(versionArray[i]);

                if (latestVersion > previousVersion)
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
                else if (latestVersion < previousVersion)
                {
                    return "401";
                }
                continue;

            }

            return string.Empty;
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
