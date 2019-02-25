using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PressFitApi.Models;

namespace PressFitApi.Controllers
{
    public class SubjectController : Controller
    {
        private PressFitApiContext db = new PressFitApiContext();

        // GET: MsgSubjects
        public ActionResult Index()
        {
            return View(db.Subject.ToList());
        }

        // GET: MsgSubjects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubjectModel msgSubject = db.Subject.Find(id);
            if (msgSubject == null)
            {
                return HttpNotFound();
            }
            return View(msgSubject);
        }

        // GET: MsgSubjects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MsgSubjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Subject")] SubjectModel msgSubject)
        {
            if (ModelState.IsValid)
            {
                db.Subject.Add(msgSubject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(msgSubject);
        }

        // GET: MsgSubjects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubjectModel msgSubject = db.Subject.Find(id);
            if (msgSubject == null)
            {
                return HttpNotFound();
            }
            return View(msgSubject);
        }

        // POST: MsgSubjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Subject")] SubjectModel msgSubject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(msgSubject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(msgSubject);
        }

        // GET: MsgSubjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubjectModel msgSubject = db.Subject.Find(id);
            if (msgSubject == null)
            {
                return HttpNotFound();
            }
            return View(msgSubject);
        }

        // POST: MsgSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubjectModel msgSubject = db.Subject.Find(id);
            db.Subject.Remove(msgSubject);
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
