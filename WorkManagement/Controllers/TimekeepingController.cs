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
    public class TimekeepingController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();
        // GET: Check
        public ActionResult Check()
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/TimeKeeping/Check";
                return Redirect("~/accounts/login");
            }
            int accID = ((Account)Session["user_login"]).ID;
            DateTime now = DateTime.Now;
            string date = now.Year.ToString() + now.Month.ToString() + now.Day.ToString();
            Timekeeping tkp = db.Timekeepings.SingleOrDefault(t => t.Account_ID == accID && t.CheckIn.Substring(0, 8) == date);
            if (tkp!=null)
            {
                TempData["CheckIn"] = "Bạn đã check in";
                if (tkp.CheckOut != null)
                {
                    TempData["CheckOut"] = "Bạn đã check out";
                }
            }
            return View();
        }

        // Post: CheckIn
        public ActionResult CheckIn()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            int accID = ((Account)Session["user_login"]).ID;
            DateTime now = DateTime.Now;
            string date = now.Year.ToString() + now.Month.ToString() + now.Day.ToString();
            Timekeeping tkp = db.Timekeepings.SingleOrDefault(t => t.Account_ID == accID && t.CheckIn.Substring(0,8) == date);
            if (tkp==null)
            {
                string checkin = DatetimeToString(now);
                tkp = new Timekeeping();
                tkp.CheckIn = checkin;
                tkp.Account_ID = accID;
                db.Timekeepings.Add(tkp);
                db.SaveChanges();
            }
            else
            {
                TempData["CheckInFailed"] = "Check in không thành công, hãy chắc chắn rằng bạn chưa check in trước đó!";
            }
            return RedirectToAction("Check");
        }
        // Post: CheckOut
        public ActionResult CheckOut()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }

            int accID = ((Account)Session["user_login"]).ID;
            DateTime now = DateTime.Now;
            string date = now.Year.ToString() + now.Month.ToString() + now.Day.ToString();
            Timekeeping tkp = db.Timekeepings.SingleOrDefault(t => t.Account_ID == accID && t.CheckIn.Substring(0, 8) == date);
            if (tkp != null)
            {
                tkp.CheckOut = DatetimeToString(now);
                db.Entry(tkp).State = EntityState.Modified;
                db.SaveChanges();
            }


                return RedirectToAction("Check");
        }


        // GET: ManagerToday
        public ActionResult ManagerToday()
        {
            var list = db.Timekeepings;
            return View(list.ToList());
        }

        // GET: ManagerOption
        public ActionResult ManagerOption()
        {
            
            return View();
        }

        // GET: Show a month of a employee
        public ActionResult ShowInMonthOneEmployee()
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/TimeKeeping/ShowInMonthOneEmployee";
                return Redirect("~/accounts/login");
            }
            DateTime dt = new DateTime();
            if (Session["DateTime"] ==null)
            {
                dt = DateTime.Now;
                Session["DateTime"] = dt;
            }
            else
            {
                dt = (DateTime)Session["DateTime"];
            }
            
            int accID = ((Account)Session["user_login"]).ID;

            var list = db.Timekeepings.Where(t => t.Account_ID == accID && t.CheckIn.Substring(4, 2) == dt.Month.ToString());

            return View(list.ToList());
        }
        // POST: TimeKeeping/ShowInMonthOneEmployeePost
        [HttpPost]
        public ActionResult ShowInMonthOneEmployeePost()
        {
            int month = int.Parse(Request["month"]);
            DateTime dt = new DateTime(DateTime.Now.Year, month, DateTime.DaysInMonth(DateTime.Now.Year, month));
            Session["DateTime"] = dt;
            
            return RedirectToAction("ShowInMonthOneEmployee");
        }

        public DateTime StringToDatetime(string s)
        {
            DateTime rs ;
            string[] l = s.Split();
            rs = new DateTime(int.Parse(s.Substring(0,4)), int.Parse(s.Substring( 4,2)),
                int.Parse(s.Substring(6, 2)), int.Parse(s.Substring(8, 2)), int.Parse(s.Substring(10, 2)), 0);
            return rs;
        }

        public string DatetimeToString(DateTime dt)
        {
            string rs;
            rs = dt.Year.ToString() + dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() 
                + dt.Minute.ToString();
            return rs;
        }

        public int TakeDay(string s)
        {
            int rs = int.Parse(s.Substring(6, 2));
            return rs;
        }

        public int TakeMonth(string s)
        {
            int rs = int.Parse(s.Substring(4, 2));
            return rs;
        }
    }
}