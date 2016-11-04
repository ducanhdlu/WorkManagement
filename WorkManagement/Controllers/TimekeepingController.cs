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
            string date = now.Year.ToString() + (now.Month>9? now.Month.ToString():"0"+ now.Month.ToString()) + (now.Day > 9 ? now.Day.ToString() : "0" + now.Day.ToString());
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
                string checkin = Static.DatetimeToString(now);
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
            string date = now.Year.ToString() + (now.Month > 9 ? now.Month.ToString() : "0" + now.Month.ToString()) + (now.Day > 9 ? now.Day.ToString() : "0" + now.Day.ToString());
            Timekeeping tkp = db.Timekeepings.SingleOrDefault(t => t.Account_ID == accID && t.CheckIn.Substring(0, 8) == date);
            if (tkp != null)
            {
                tkp.CheckOut = Static.DatetimeToString(now);
                db.Entry(tkp).State = EntityState.Modified;
                db.SaveChanges();
            }


                return RedirectToAction("Check");
        }


        // GET: ManagerToday
        public ActionResult ManagerToday()
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/TimeKeeping/ManagerToday";
                return Redirect("~/accounts/login");
            }
            Account cur_user = Session["user_login"] as Account;
            DateTime dt = DateTime.Now;
            List<Timekeeping> tkps = new List<Timekeeping>();
            foreach (var item in db.Timekeepings)
            {
                if (Static.StringToDatetime(item.CheckIn).Date == dt.Date && int.Parse(item.Account.Permission_ID)> int.Parse(cur_user.Permission_ID))
                {
                    tkps.Add(item);
                }
            }
            return View(tkps);
        }

        // GET: ManagerOption
        public ActionResult ManagerOption()
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/TimeKeeping/ManagerOption";
                return Redirect("~/accounts/login");
            }
            Account cur_user = Session["user_login"] as Account;
            var rs = new List<Account>();
            foreach (var item in db.Accounts.ToList())
            {
                if (int.Parse(item.Permission_ID)> int.Parse(cur_user.Permission_ID))
                {
                    rs.Add(item);
                }
            }
            Session["employees"] = rs;
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
            //xóa session tạo ra từ action manageroption truy cập trước đó
            Session["employees"] = null;
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

            //nhân viên tự xem thông tin chính mình 
            int accID = 0;
            if (Session["accID"]==null)
            {
                accID = ((Account)Session["user_login"]).ID;

            }
            //quản lý xem thông tin của một nhân viên
            //Session["accID"] được tạo ở action managerOption truy cập bởi quản lý
            else
            {
                accID = (int)Session["accID"];
            }

            var list = db.Timekeepings.Where(t => t.Account_ID == accID && t.CheckIn.Substring(4, 2) == dt.Month.ToString());

            return View(list.ToList());
        }
        // POST: TimeKeeping/ShowInMonthOneEmployeePost
        [HttpPost]
        public ActionResult ShowInMonthOneEmployeePost()
        {
            int month = int.Parse(Request["month"]);
            //kiểm tra nếu đi từ manageroption thì tạo thêm session accID cần xem
            if (Session["employees"] != null)
            {
                int accID = int.Parse(Request["employee"]);
                Session["accID"] = accID;
            }
            
            DateTime dt = new DateTime(DateTime.Now.Year, month, DateTime.DaysInMonth(DateTime.Now.Year, month), 0, 0, 0);
            if (month == DateTime.Now.Month)
            {
                dt = new DateTime(DateTime.Now.Year, month, DateTime.Now.Day, 0, 0, 0);
            }
            Session["DateTime"] = dt;
            return RedirectToAction("ShowInMonthOneEmployee");
        }
        
    }
}