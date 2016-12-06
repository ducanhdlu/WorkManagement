using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkManagement.Models;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using PagedList;

namespace WorkManagement.Controllers
{
    public class GoOutLetterController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();
        // GET: GoOutLetter/Index_Manager
        public ActionResult Index_Manager(string sortOrder, string searchString, string currentSearch, int? page)
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                //Session["tempLink"] = "~/GoOutLetter/Index_Manager";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý thì trả về trang không tìm thấy
            if ( ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            
            List<GoOutLetter> goOutLetters = new List<GoOutLetter>();
            foreach (var item in db.GoOutLetters)
            {
                if (item.Account.Permission_ID == "4")
                {
                    goOutLetters.Add(item);
                }
            }

            //tìm kiếm, sắp xếp
            ViewBag.StartTimeSort = string.IsNullOrEmpty(sortOrder) ? "desc" : "";
            ViewBag.CreateTimeSort = sortOrder == "create" ? "create_desc" : "create";
            ViewBag.CurrentSort = sortOrder;
            if (searchString == null)
                searchString = currentSearch;
            ViewBag.CurrentSearch = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                goOutLetters = goOutLetters.Where(a => a.Account.Employee.FullName.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "desc":
                    goOutLetters = goOutLetters.OrderBy(a => a.StartTime).ToList();
                    break;
                case "create_desc":
                    goOutLetters = goOutLetters.OrderByDescending(a => a.CreateTime).ToList();
                    break;
                case "create":
                    goOutLetters = goOutLetters.OrderBy(a => a.CreateTime).ToList();
                    break;
                default:
                    goOutLetters = goOutLetters.OrderByDescending(a => a.StartTime).ToList();
                    break;
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(goOutLetters.ToPagedList(pageNumber, pageSize));
        }
        // GET: GoOutLetter/Index_Employee
        public ActionResult Index_Employee(string sortOrder, string searchString, string currentSearch, int? page)
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                //Session["tempLink"] = "~/GoOutLetter/Index_Employee";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc nhân viên thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "4" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            Account user = Session["user_login"] as Account;
            var goOutLetters = db.GoOutLetters.Where(a => a.Account.ID == user.ID).ToList();

            //tìm kiếm, sắp xếp
            ViewBag.StartTimeSort = string.IsNullOrEmpty(sortOrder) ? "desc" : "";
            ViewBag.CreateTimeSort = sortOrder == "create" ? "create_desc" : "create";
            ViewBag.CurrentSort = sortOrder;
            if (searchString == null)
                searchString = currentSearch;
            ViewBag.CurrentSearch = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                goOutLetters = goOutLetters.Where(a => a.StartTime.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "desc":
                    goOutLetters = goOutLetters.OrderBy(a => a.StartTime).ToList();
                    break;
                case "create_desc":
                    goOutLetters = goOutLetters.OrderByDescending(a => a.CreateTime).ToList();
                    break;
                case "create":
                    goOutLetters = goOutLetters.OrderBy(a => a.CreateTime).ToList();
                    break;
                default:
                    goOutLetters = goOutLetters.OrderByDescending(a => a.StartTime).ToList();
                    break;
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(goOutLetters.ToPagedList(pageNumber, pageSize));
        }
        // GET: GoOutLetter/Create
        public ActionResult Create()
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                //Session["tempLink"] = "~/GoOutLetter/Create";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc nhân viên thì trả về trang không tìm thấy
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
                    var goOutLetters = db.GoOutLetters.Where(a => a.Account.ID == user.ID).ToList();
                    foreach (var item in goOutLetters)
                    {
                        if (item.StartTime.Substring(0,8) == goOut.StartTime.Substring(0, 8))
                        {
                            TempData["ErrorDay"] = "Đã có đơn xin nghỉ sớm vào ngày hôm đó";
                            return RedirectToAction("Create");
                        }
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
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                GoOutLetter goOut = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                if (goOut.Status == "1" || goOut.Status == "6")
                {
                    Employee cur_emp = db.Employees.SingleOrDefault(e => e.ID == goOut.Account.Employee_ID);
                    cur_emp.HoursUsed -= goOut.TimeLeave;
                    db.Entry(cur_emp).State = EntityState.Modified;
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
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                GoOutLetter absen = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "1";
                db.Entry(absen).State = EntityState.Modified;
                // cập nhật số ngày nghỉ đã dùng của nhân viên đó
                Employee cur_emp = db.Employees.SingleOrDefault(e => e.ID == absen.Account.Employee_ID);
                cur_emp.HoursUsed += absen.TimeLeave;
                db.Entry(cur_emp).State = EntityState.Modified;

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
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
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
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
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
        public ActionResult ViewGoOutLetter_SuperManager(string sortOrder, string searchString, string currentSearch, int? page)
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                //Session["tempLink"] = "~/GoOutLetter/ViewGoOutLetter_SuperManager";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2")
            {
                return Redirect("~/Default/Error");
            }
            List<GoOutLetter> goOutLetters = new List<GoOutLetter>();
            foreach (var item in db.GoOutLetters)
            {
                if (item.Status == "4" || item.Status == "5" || item.Status == "6" || item.Status == "7")
                {
                    goOutLetters.Add(item);
                }
            }
            //tìm kiếm, sắp xếp
            ViewBag.StartTimeSort = string.IsNullOrEmpty(sortOrder) ? "desc" : "";
            ViewBag.CreateTimeSort = sortOrder == "create" ? "create_desc" : "create";
            ViewBag.CurrentSort = sortOrder;
            if (searchString == null)
                searchString = currentSearch;
            ViewBag.CurrentSearch = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                goOutLetters = goOutLetters.Where(a => a.Account.Employee.FullName.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "desc":
                    goOutLetters = goOutLetters.OrderBy(a => a.StartTime).ToList();
                    break;
                case "create_desc":
                    goOutLetters = goOutLetters.OrderByDescending(a => a.CreateTime).ToList();
                    break;
                case "create":
                    goOutLetters = goOutLetters.OrderBy(a => a.CreateTime).ToList();
                    break;
                default:
                    goOutLetters = goOutLetters.OrderByDescending(a => a.StartTime).ToList();
                    break;
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(goOutLetters.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult QLCCChapNhan(int id)
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                GoOutLetter absen = db.GoOutLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "6";
                // cập nhật số ngày nghỉ đã dùng của nhân viên đó
                Employee cur_emp = db.Employees.SingleOrDefault(e => e.ID == absen.Account.Employee_ID);
                cur_emp.HoursUsed += absen.TimeLeave;
                db.Entry(cur_emp).State = EntityState.Modified;
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
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
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