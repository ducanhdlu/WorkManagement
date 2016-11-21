using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkManagement.Models;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;

namespace WorkManagement.Controllers
{
    public class GoOutLetterController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();
        // GET: GoOutLetter/Index_Manager
        public ActionResult Index_Manager()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            else
            {
                List<GoOutLetter> goOutLetters = new List<GoOutLetter>();
                foreach (var item in db.GoOutLetters)
                {
                    if (item.Account.Permission_ID == "4")
                    {
                        goOutLetters.Add(item);
                    }
                }
                return View(goOutLetters.ToList());
            }
        }
        // GET: GoOutLetter/Index_Employee
        public ActionResult Index_Employee()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            else
            {
                Account user = Session["user_login"] as Account;
                var goOutLetters = db.GoOutLetters.Where(a => a.Account.ID == user.ID);
                return View(goOutLetters.ToList());
            }
        }
        // GET: GoOutLetter/Create
        public ActionResult Create()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            if (((Account)Session["user_login"]).Permission_ID != "4" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            ViewBag.ListReason = db.Reasons;
            return View();
        }
        public ActionResult CreatePost()
        {
            if (ModelState.IsValid)
            {

                if (Session["user_login"] == null)
                {
                    return Redirect("~/accounts/login");
                }
                try
                {
                    Account user = Session["user_login"] as Account;
                    GoOutLetter goOut = new GoOutLetter();
                    goOut.StartTime = Static.ShortDatetimeToString(Request["startTime"], Request["hour"], Request["minute"]);
                    if (Static.StringToDatetime(goOut.StartTime) <= DateTime.Now)
                    {
                        TempData["ErrorDay"] = "Thời gian bắt đầu nghỉ phép không hợp lệ";
                        return RedirectToAction("Create");
                    }
                    goOut.TimeLeave = Byte.Parse(Request["totalTime"]);
                    goOut.CreateTime = Static.DatetimeToString(DateTime.Now);
                    if (user.Permission_ID == "3")
                    {
                        goOut.Status = "4";
                    }
                    else
                    {
                        goOut.Status = "0";
                    }
                    goOut.Reason_ID = Request.Form["reason"];
                    goOut.Account_ID = user.ID;
                    db.GoOutLetters.Add(goOut);
                    db.SaveChanges();
                    return RedirectToAction("Index_Employee");
                }
                catch (Exception)
                {

                    return Redirect("~/Default/ErrorDB");
                }
            }
            return Redirect("~/GoOutLetter/Create");

        }
        public ActionResult CancelGoOutLetter(int id)
        {
            try
            {
                GoOutLetter goOut = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                if (goOut.Status == "1" || goOut.Status == "6")
                {
                    Employee cur_emp = db.Employees.SingleOrDefault(e => e.ID == goOut.Account.Employee_ID);
                    cur_emp.HoursUsed -= goOut.TimeLeave;
                }
                goOut.Status = "3";
                goOut.CreateTime = Static.DatetimeToString(DateTime.Now);
                db.Entry(goOut).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("~/GoOutLetter/Index_Employee");
            }
            catch (Exception)
            {

                return Redirect("~/Default/ErrorDB");
            }

        }
        public ActionResult XuLyYeuCauChapNhan(int id)
        {
            try
            {
                GoOutLetter absen = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "1";
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMesseger(db);
                return Redirect("~/GoOutLetter/Index_Manager");
            }
            catch (Exception)
            {

                return Redirect("~/Default/ErrorDB");
            }

        }
        public ActionResult XuLyYeuCauCTuChoi(int id)
        {
            try
            {
                GoOutLetter absen = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "2";
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMesseger(db);
                return Redirect("~/GoOutLetter/Index_Manager");
            }
            catch (Exception)
            {

                return Redirect("~/Default/ErrorDB");
            }
        }
        public ActionResult XyLyChuyen(int id)
        {
            try
            {
                GoOutLetter absen = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "5";
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMesseger(db);
                return Redirect("~/GoOutLetter/Index_Manager");
            }
            catch (Exception)
            {

                return Redirect("~/Default/ErrorDB");
            }

        }
        // GET: GoOutLetter/ViewGoOutLetter
        public ActionResult ViewGoOutLetter_SuperManager()
        {
            List<GoOutLetter> goOutLetters = new List<GoOutLetter>();
            foreach (var item in db.GoOutLetters)
            {
                if (item.Status == "4" || item.Status == "5" || item.Status == "6" || item.Status == "7")
                {
                    goOutLetters.Add(item);
                }
            }
            return View(goOutLetters.ToList());
        }
        public ActionResult QLCCChapNhan(int id)
        {
            try
            {
                GoOutLetter absen = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "6";
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMessegerSuper(db);
                return Redirect("~/GoOutLetter/ViewGoOutLetter_SuperManager");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }

        }
        public ActionResult QLCCTuChoi(int id)
        {
            try
            {
                GoOutLetter absen = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "7";
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMessegerSuper(db);
                return Redirect("~/GoOutLetter/ViewGoOutLetter_SuperManager");
            }
            catch (Exception)
            {

                return Redirect("~/Default/ErrorDB");
            }

        }
    }
}