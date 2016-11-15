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
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/BonusDayOffs/Index";
                return Redirect("~/Accounts/Login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            List<BonusDayOff> bonusDayOffs = new List<BonusDayOff>();
            foreach (var item in db.BonusDayOffs)
            {
                if (int.Parse(((Account)Session["user_login"]).Permission_ID)< item.Employee_ID)
                {
                    bonusDayOffs.Add(item);
                }
            }
            return View(bonusDayOffs.ToList());
        }
        

        // GET: BonusDayOffs/Create
        public ActionResult Create()
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/BonusDayOffs/Create";
                return Redirect("~/Accounts/Login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            List<Employee> tkps = new List<Employee>();
            foreach (var item in db.Employees)
            {
                if (int.Parse(item.Accounts.SingleOrDefault(a => a.Employee_ID == item.ID).Permission_ID) > int.Parse(((Account)Session["user_login"]).Permission_ID))
                {
                    tkps.Add(item);
                }
            }
            ViewBag.Employees = tkps;
            return View();
        }

        // POST: BonusDayOffs/Create
        [HttpPost]
        public ActionResult Create(int employee_id, string number_of_days, string description)
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/BonusDayOffs/Create";
                return Redirect("~/Accounts/Login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            try
            {
                BonusDayOff bonusDayOff = new BonusDayOff();
                bonusDayOff.Employee_ID = employee_id;
                bonusDayOff.TotalDates = byte.Parse(number_of_days);
                bonusDayOff.Description = description;
                bonusDayOff.CreateTime = Static.DatetimeToString(DateTime.Now);
                bonusDayOff.CreateUser_ID = ((Account)Session["user_login"]).ID;
                db.BonusDayOffs.Add(bonusDayOff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }
        }

        // POST: BonusDayOffs/Delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/BonusDayOffs/Create";
                return Redirect("~/Accounts/Login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            try
            {
                BonusDayOff bonusDayOff = db.BonusDayOffs.SingleOrDefault(b=>b.ID==id);
                db.BonusDayOffs.Remove(bonusDayOff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
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
    }
}
