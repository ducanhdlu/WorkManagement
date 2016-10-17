using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WorkManagement.Models;

namespace WorkManagement.Controllers
{
    public class BonusDayOffsController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();

        // GET: BonusDayOffs
        public ActionResult Index()
        {
            var bonusDayOffs = db.BonusDayOffs.Include(b => b.Account).Include(b => b.Employee);
            return View(bonusDayOffs.ToList());
        }

        // GET: BonusDayOffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BonusDayOff bonusDayOff = db.BonusDayOffs.Find(id);
            if (bonusDayOff == null)
            {
                return HttpNotFound();
            }
            return View(bonusDayOff);
        }

        // GET: BonusDayOffs/Create
        public ActionResult Create()
        {
            ViewBag.CreateUser_ID = new SelectList(db.Accounts, "ID", "Email");
            ViewBag.Employee_ID = new SelectList(db.Employees, "ID", "FullName");
            return View();
        }

        // POST: BonusDayOffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TotalDates,Description,CreateTime,CreateUser_ID,Employee_ID")] BonusDayOff bonusDayOff)
        {
            if (ModelState.IsValid)
            {
                db.BonusDayOffs.Add(bonusDayOff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreateUser_ID = new SelectList(db.Accounts, "ID", "Email", bonusDayOff.CreateUser_ID);
            ViewBag.Employee_ID = new SelectList(db.Employees, "ID", "FullName", bonusDayOff.Employee_ID);
            return View(bonusDayOff);
        }

        // GET: BonusDayOffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BonusDayOff bonusDayOff = db.BonusDayOffs.Find(id);
            if (bonusDayOff == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreateUser_ID = new SelectList(db.Accounts, "ID", "Email", bonusDayOff.CreateUser_ID);
            ViewBag.Employee_ID = new SelectList(db.Employees, "ID", "FullName", bonusDayOff.Employee_ID);
            return View(bonusDayOff);
        }

        // POST: BonusDayOffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TotalDates,Description,CreateTime,CreateUser_ID,Employee_ID")] BonusDayOff bonusDayOff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bonusDayOff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreateUser_ID = new SelectList(db.Accounts, "ID", "Email", bonusDayOff.CreateUser_ID);
            ViewBag.Employee_ID = new SelectList(db.Employees, "ID", "FullName", bonusDayOff.Employee_ID);
            return View(bonusDayOff);
        }

        // GET: BonusDayOffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BonusDayOff bonusDayOff = db.BonusDayOffs.Find(id);
            if (bonusDayOff == null)
            {
                return HttpNotFound();
            }
            return View(bonusDayOff);
        }

        // POST: BonusDayOffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BonusDayOff bonusDayOff = db.BonusDayOffs.Find(id);
            db.BonusDayOffs.Remove(bonusDayOff);
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
