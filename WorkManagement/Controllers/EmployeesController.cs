using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkManagement.Models;
using System.Data.Entity;
using System.Net;

namespace WorkManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();
        // GET: Employees/Index
        public ActionResult Index()
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/Employees/Index";
                return Redirect("~/Accounts/Login");
            }
            int curID = ((Account)Session["user_login"]).ID;
            Account cur = db.Accounts.SingleOrDefault(a => a.ID == curID);
            int sumDayOff = 12;
            List<BonusDayOff> listBDO = db.BonusDayOffs.Where(b => b.Employee.ID == cur.Employee.ID).ToList();
            if (listBDO.Count>0)
            {
                foreach (var item in listBDO)
                {
                    sumDayOff += item.TotalDates;
                }
            }
            ViewBag.SumDayOff = sumDayOff;
            ViewBag.SumBonus = sumDayOff - 12;
            ViewBag.SumHourOut = 8;
            ViewBag.User = cur;
            return View();
        }

        public ActionResult ViewTimeUsed()
        {
            if (Session["user_login"] == null)
            {
                Session["tempLink"] = "~/Employees/ViewTimeUsed";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            Account cur_user = Session["user_login"] as Account;
            Session["DayOffInYear"] = db.DefaultValues.SingleOrDefault(d=>d.Value==12);
            Session["TimeOffInMonth"] = db.DefaultValues.SingleOrDefault(d => d.Value == 8);
            Session["BonusDayOffs"] = db.BonusDayOffs.ToList();
            var employees = new List<Employee>();
            foreach (var item in db.Employees)
            {
                if (int.Parse((item.Accounts.SingleOrDefault(a=>a.Employee_ID==item.ID)).Permission_ID)> int.Parse(cur_user.Permission_ID))
                {
                    employees.Add(item);
                }
            }
            return View(employees);
        }
    }
}