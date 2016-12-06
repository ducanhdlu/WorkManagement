using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkManagement.Models;
using System.Data.Entity;
using System.Net;
using PagedList;

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
                //Session["tempLink"] = "~/Employees/Index";
                return Redirect("~/Accounts/Login");
            }
            int curID = ((Account)Session["user_login"]).ID;
            Account cur = db.Accounts.FirstOrDefault(a => a.ID == curID);
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
            if (cur != null)
            {
                ViewBag.User = cur;
            }
            
            return View();
        }

        public ActionResult ViewTimeUsed(string sortOrder, string searchString, string currentSearch, int? page)
        {
            if (Session["user_login"] == null)
            {
                //Session["tempLink"] = "~/Employees/ViewTimeUsed";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            Account cur_user = Session["user_login"] as Account;
            Session["DayOffInYear"] = db.DefaultValues.FirstOrDefault(d=>d.Value==12);
            Session["TimeOffInMonth"] = db.DefaultValues.FirstOrDefault(d => d.Value == 8);
            Session["BonusDayOffs"] = db.BonusDayOffs.ToList();
            var employees = new List<Employee>();
            foreach (var item in db.Accounts)
            {
                if (int.Parse(item.Permission_ID)> int.Parse(cur_user.Permission_ID))
                {
                    employees.Add(item.Employee);
                }
            }
            //tìm kiếm, sắp xếp
            ViewBag.NameSort = string.IsNullOrEmpty(sortOrder) ? "desc" : "";
            ViewBag.DayUsedSort = sortOrder == "dayused" ? "dayused_desc" : "dayused";
            ViewBag.HourSort = sortOrder == "hour" ? "hour_desc" : "hour";
            ViewBag.CurrentSort = sortOrder;
            if (searchString == null)
                searchString = currentSearch;
            ViewBag.CurrentSearch = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(a => a.FullName.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "desc":
                    employees = employees.OrderBy(a => a.FullName).ToList();
                    break;
                case "dayused_desc":
                    employees = employees.OrderByDescending(a => a.DaysUsed).ToList();
                    break;
                case "dayused":
                    employees = employees.OrderBy(a => a.DaysUsed).ToList();
                    break;
                case "hour_desc":
                    employees = employees.OrderByDescending(a => a.HoursUsed).ToList();
                    break;
                case "hour":
                    employees = employees.OrderBy(a => a.HoursUsed).ToList();
                    break;
                default:
                    employees = employees.OrderByDescending(a => a.FullName).ToList();
                    break;
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(employees.ToPagedList(pageNumber, pageSize));
        }
    }
}