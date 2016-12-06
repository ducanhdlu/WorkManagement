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
    public class AbsenceLetterController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();
        
        // GET: AbsenceLetter/Index_Employee
        public ActionResult Index_Employee(string sortOrder, string searchString, string currentSearch, int? page)
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                //Session["tempLink"] = "~/AbsenceLetter/Index_Employee";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý hoặc nhân viên thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "4" && ((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }
            Account user = Session["user_login"] as Account;
            var absenceLetters = db.AbsenceLetters.Where(a => a.Account.ID == user.ID).ToList();

            //tìm kiếm, sắp xếp
            ViewBag.StartTimeSort = string.IsNullOrEmpty(sortOrder) ? "desc" : "";
            ViewBag.CreateTimeSort = sortOrder == "create" ? "create_desc" : "create";
            ViewBag.CurrentSort = sortOrder;
            if (searchString == null)
                searchString = currentSearch;
            ViewBag.CurrentSearch = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                absenceLetters = absenceLetters.Where(a => a.StartTime.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "desc":
                    absenceLetters = absenceLetters.OrderBy(a => a.StartTime).ToList();
                    break;
                case "create_desc":
                    absenceLetters = absenceLetters.OrderByDescending(a => a.CreateTime).ToList();
                    break;
                case "create":
                    absenceLetters = absenceLetters.OrderBy(a => a.CreateTime).ToList();
                    break;
                default:
                    absenceLetters = absenceLetters.OrderByDescending(a => a.StartTime).ToList();
                    break;
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(absenceLetters.ToPagedList(pageNumber, pageSize));

        }
        /*
         giá trị Status của absence letter và ý nghĩa
                case "0":
                    return "Chưa duyệt";
                case "1":
                    return "Đã chấp nhận";
                case "2":
                    return "Đã từ chối";
                case "3":
                    return "Đã hủy";
                case "4": 
                    return "Chưa duyệt";
                case "5":
                    return "Đã chuyển lên";
                case "6":
                    return "QLCC đã chấp nhận";
                case "7":
                    return "QLCC đã từ chối";
             */

        // GET: AbsenceLetter/Index_Manager
        public ActionResult Index_Manager(string sortOrder, string searchString, string currentSearch, int? page)
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                //Session["tempLink"] = "~/AbsenceLetter/Index_Manager";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "3")
            {
                return Redirect("~/Default/Error");
            }

            List<AbsenceLetter> absenceLetters = new List<AbsenceLetter>();
            foreach (var item in db.AbsenceLetters)
            {
                if (int.Parse(((Account)Session["user_login"]).Permission_ID)< int.Parse(item.Account.Permission_ID))
                {
                    absenceLetters.Add(item);
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
                absenceLetters = absenceLetters.Where(a => a.Account.Employee.FullName.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "desc":
                    absenceLetters = absenceLetters.OrderBy(a => a.StartTime).ToList();
                    break;
                case "create_desc":
                    absenceLetters = absenceLetters.OrderByDescending(a => a.CreateTime).ToList();
                    break;
                case "create":
                    absenceLetters = absenceLetters.OrderBy(a => a.CreateTime).ToList();
                    break;
                default:
                    absenceLetters = absenceLetters.OrderByDescending(a => a.StartTime).ToList();
                    break;
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(absenceLetters.ToPagedList(pageNumber, pageSize));
        }
        /// <summary>
        /// hoaippt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult XuLyYeuCauChapNhan(int id)
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                AbsenceLetter absen = db.AbsenceLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "1";
                db.Entry(absen).State = EntityState.Modified;
                //cập nhật số ngày nghỉ đã dùng của nhân viên đó
                Employee cur_emp = db.Employees.FirstOrDefault(e => e.ID == absen.Account.Employee_ID);
                if (cur_emp!=null)
                {
                    cur_emp.DaysUsed += absen.TotalTime;
                }
                db.Entry(cur_emp).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMesseger(db);
                return Redirect("~/AbsenceLetter/Index_Manager");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }
            
        }
        /// <summary>
        /// hoaippt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult XuLyYeuCauTuChoi(int id)
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                AbsenceLetter absen = db.AbsenceLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "2";
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMesseger(db);
                return Redirect("~/AbsenceLetter/Index_Manager");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }
            
        }
        public ActionResult ChuyenLenQLCC(int id)
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                AbsenceLetter absen = db.AbsenceLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "5";
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMesseger(db);
                return Redirect("~/AbsenceLetter/Index_Manager");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }

        }


        // GET: AbsenceLetter/Create
        public ActionResult Create()
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                //Session["tempLink"] = "~/AbsenceLetter/Create";
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

        // post: AbsenceLetter/Create
        [HttpPost]
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
                    AbsenceLetter absen = new AbsenceLetter();
                    absen.StartTime = Static.ShortDatetimeToString(Request["startDay"]);
                    if (Static.StringToDatetime(absen.StartTime)<=DateTime.Now)
                    {
                        TempData["ErrorDay"] = "Thời gian bắt đầu nghỉ phép không hợp lệ";
                        return RedirectToAction("Create");
                    }
                    var absenceLetters = db.AbsenceLetters.Where(a => a.Account.ID == user.ID).ToList();
                    foreach (var item in absenceLetters)
                    {
                        if (item.StartTime.Substring(0, 8) == absen.StartTime.Substring(0, 8))
                        {
                            TempData["ErrorDay"] = "Đã có đơn xin nghỉ phép vào ngày hôm đó";
                            return RedirectToAction("Create");
                        }
                    }
                    var x = Request["startDay"];
                    absen.TotalTime = Convert.ToInt16(Request["totalTime"]);
                    absen.Account_ID = user.ID;
                    absen.Reason_ID = Request.Form["reason"];
                    absen.CreateTime = Static.DatetimeToString(DateTime.Now);
                    //nếu quản lý gửi thì trạng thái bằng 4, nhân viên thì trạng thái bằng 1
                    if (user.Permission_ID=="3")
                    {
                        absen.Status = "4";
                    }
                    else
                    {
                        absen.Status = "0";
                    }
                    
                    db.AbsenceLetters.Add(absen);
                    db.SaveChanges();
                    return RedirectToAction("Index_Employee");
                }
                catch (Exception)
                {
                    return Redirect("~/Default/ErrorDB");
                }
            }
            return Redirect("~/AbsenceLetter/Create");
        }
        
        //hủy một đơn xin nghỉ phép
        public ActionResult CancelAbsenceLetter(int id)
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                Static.setMesseger(db);
                AbsenceLetter absen = db.AbsenceLetters.FirstOrDefault(x => x.ID == id);
                //nếu đơn này đã được chấp nhận thì trừ số ngày nghỉ này trong thông tin nhân viên vì chưa dùng tới
                if (absen.Status=="1"|| absen.Status == "6")
                {
                    Employee cur_emp = db.Employees.FirstOrDefault(e => e.ID == absen.Account.Employee_ID);
                    if (cur_emp!=null)
                    {
                    cur_emp.DaysUsed -= absen.TotalTime;
                    db.Entry(cur_emp).State = EntityState.Modified;
                    }
                    
                }
                absen.Status = "3";
                absen.CreateTime = Static.DatetimeToString(DateTime.Now);
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                
                return Redirect("~/AbsenceLetter/Index_Employee");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }
            
        }
        // GET: AbsenceLetter/ViewAbsenceLetter_SuperManager
        public ActionResult ViewAbsenceLetter_SuperManager(string sortOrder, string searchString, string currentSearch, int? page)
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                //Session["tempLink"] = "~/AbsenceLetter/ViewAbsenceLetter_SuperManager";
                return Redirect("~/accounts/login");
            }
            //không có quyền quản lý cấp cao thì trả về trang không tìm thấy
            if (((Account)Session["user_login"]).Permission_ID != "2" )
            {
                return Redirect("~/Default/Error");
            }
            //quản lý cấp cao chỉ thấy các đơn của quản lý gửi, đơn được chuyển lên hoặc được duyệt
            List<AbsenceLetter> absenceLetters = new List<AbsenceLetter>();
            foreach (var item in db.AbsenceLetters)
            {
                if (item.Status == "4" || item.Status == "5" || item.Status == "6" || item.Status == "7")
                {
                    absenceLetters.Add(item);
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
                absenceLetters = absenceLetters.Where(a => a.Account.Employee.FullName.Contains(searchString)).ToList();
            }
            switch (sortOrder)
            {
                case "desc":
                    absenceLetters = absenceLetters.OrderBy(a => a.StartTime).ToList();
                    break;
                case "create_desc":
                    absenceLetters = absenceLetters.OrderByDescending(a => a.CreateTime).ToList();
                    break;
                case "create":
                    absenceLetters = absenceLetters.OrderBy(a => a.CreateTime).ToList();
                    break;
                default:
                    absenceLetters = absenceLetters.OrderByDescending(a => a.StartTime).ToList();
                    break;
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(absenceLetters.ToPagedList(pageNumber, pageSize));
        }
        //quản lý cấp cao từ chối 1 đơn
        public ActionResult Cancel_SuperManager(int id)
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                AbsenceLetter absen = db.AbsenceLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "7";
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMessegerSuper(db);
                return Redirect("~/AbsenceLetter/ViewAbsenceLetter_SuperManager");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }

        }
        //quản lý cấp cao chấp nhận 1 đơn
        public ActionResult Accept_SuperManager(int id)
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            try
            {
                AbsenceLetter absen = db.AbsenceLetters.FirstOrDefault(x => x.ID == id);
                absen.Status = "6";
                // cập nhật số ngày nghỉ đã dùng của nhân viên đó
                Employee cur_emp = db.Employees.FirstOrDefault(e => e.ID == absen.Account.Employee_ID);
                if (cur_emp != null)
                {
                    cur_emp.DaysUsed += absen.TotalTime;
                    db.Entry(cur_emp).State = EntityState.Modified;
                }
                
                db.Entry(absen).State = EntityState.Modified;
                db.SaveChanges();
                Static.setMessegerSuper(db);
                return Redirect("~/AbsenceLetter/ViewAbsenceLetter_SuperManager");
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }

        }
    }
}