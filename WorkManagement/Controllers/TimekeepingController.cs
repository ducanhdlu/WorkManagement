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
            //không có quyền quản lý hoặc nhân viên thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "4" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
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
            //không có quyền quản lý hoặc nhân viên thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "4" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
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
            //không có quyền quản lý hoặc nhân viên thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "4" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
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
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
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
        // GET: ManagerOneDay
        public ActionResult ManagerOneDay(string day, string month)
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/TimeKeeping/ManagerOneDay";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }

            Account cur_user = Session["user_login"] as Account;
            if (!string.IsNullOrEmpty(day)&& !string.IsNullOrEmpty(month))
            {
                try
                {

                    int d = int.Parse(Request["day"]);
                    int m = int.Parse(Request["month"]);
                    DateTime dt = new DateTime(DateTime.Now.Year, m, d);
                    ViewBag.current_day = dt;
                    List<Timekeeping> tkps = new List<Timekeeping>();
                    foreach (var item in db.Timekeepings)
                    {
                        if (Static.StringToDatetime(item.CheckIn).Date == dt.Date && int.Parse(item.Account.Permission_ID) > int.Parse(cur_user.Permission_ID))
                        {
                            tkps.Add(item);
                        }
                    }
                    return View(tkps);
                }
                catch (Exception)
                {
                    return RedirectToAction("ManagerOption");
                }
            }
            return RedirectToAction("ManagerOption");
        }

        // GET: ManagerOption
        public ActionResult ManagerOption()
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/TimeKeeping/ManagerOption";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
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

        // GET: TimeKeeping/ShowInMonthOneEmployee
        // Xem thông tin chấm công của nhân viên đang đăng nhập trong 1 tháng
        public ActionResult ShowInMonthOneEmployee(string month)
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/TimeKeeping/ShowInMonthOneEmployee";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            DateTime dt = DateTime.Now;
            if (!string.IsNullOrEmpty(month))
            {
                try
                {

                    int mon = int.Parse(month);
                    dt = new DateTime(DateTime.Now.Year, mon, DateTime.Now.Day);
                }
                catch (Exception)
                {
                    TempData["WrongMonthFormat"] = "Tháng nhập sai.";
                    return View();
                }
            }
            Session["DateTime"] = dt;
            int accID = ((Account)Session["user_login"]).ID;

            var list = db.Timekeepings.Where(t => t.Account_ID == accID && t.CheckIn.Substring(4, 2) == dt.Month.ToString());

            return View(list.ToList());
        }
        // Get: TimeKeeping/ManagerOneEmployeeOneMonth
        // Quản lý xem 1 nhân viên dưới quyền trong 1 tháng
        public ActionResult ManagerOneEmployeeOneMonth(string employee, string month)
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/TimeKeeping/ManagerOneEmployeeOneMonth";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            DateTime dt = DateTime.Now;
            int accID = -1;
            if (!string.IsNullOrEmpty(month))
            {
                try
                {
                    accID = int.Parse(employee);
                    int mon = int.Parse(month);
                    dt = new DateTime(DateTime.Now.Year, mon, DateTime.Now.Day);
                    TempData["SelectedEmployeeName"] = db.Employees.Single(e => e.ID == accID).FullName;
                }
                catch (Exception)
                {
                    TempData["WrongMonthFormat"] = "Tháng hoặc nhân viên nhập sai.";
                    return View();
                }
            }
            Session["DateTime"] = dt;

            var list = db.Timekeepings.Where(t => t.Account_ID == accID && t.CheckIn.Substring(4, 2) == dt.Month.ToString());

            return View(list.ToList());
        }
        
    }
}