using PagedList;
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
    public class AccountsController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();

        // GET: Accounts
        public ActionResult Index(string sortOrder, string searchString, string currentSearch, int? page)
        {

            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                Session["tempLink"] = "~/Accounts/Index";
                return Redirect("~/accounts/login");
            }
            //không có quyền admin thì trả về trang lỗi
            if (((Account)Session["user_login"]).Permission_ID!="1")
            {
                return Redirect("~/Default/Error");
            }
            var accounts = db.Accounts.Include(a => a.Permission);
            //tìm kiếm, sắp xếp
            ViewBag.NameSort = string.IsNullOrEmpty(sortOrder) ? "desc" : "";
            ViewBag.EmailSort = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.PermissionSort = sortOrder == "permission" ? "permission_desc" : "permission";
            ViewBag.CurrentSort = sortOrder;
            if (searchString == null)
                searchString = currentSearch;
            ViewBag.CurrentSearch = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                accounts = accounts.Where(a => a.Email.Contains(searchString)|| a.Employee.FullName.Contains(searchString) );
            }
            switch (sortOrder)
            {
                case "desc":
                    accounts = accounts.OrderByDescending(a => a.Employee.FullName);
                    break;
                case "email_desc":
                    accounts = accounts.OrderByDescending(a => a.Email);
                    break;
                case "email":
                    accounts = accounts.OrderBy(a => a.Email);
                    break;
                case "permission_desc":
                    accounts = accounts.OrderByDescending(a => a.Permission_ID);
                    break;
                case "permission":
                    accounts = accounts.OrderBy(a => a.Permission_ID);
                    break;
                default:
                    accounts = accounts.OrderBy(a => a.Employee.FullName);
                    break;
            }
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(accounts.ToPagedList(pageNumber, pageSize));
        }

        
        // GET: Accounts/Login
        public ActionResult Login()
        {

            return View();
        }
        // post: Accounts/LoginPost
        [HttpPost]
        public ActionResult LoginPost()
        {
            string email = Request["email_login"].ToString();
            string password = Request["password"].ToString();
            Account user = db.Accounts.FirstOrDefault(u => u.Email == email && u.Password == password);
            
            if (user!=null)
            {
                Session["user_login"] = user;
                Session["employee_name"] = user.Employee.FullName+"";
                if (user.Permission_ID=="3" )
                {
                    Static.setMesseger(db);
                }
                if (user.Permission_ID == "2")
                {
                    Static.setMessegerSuper(db);
                }
                if (Session["tempLink"] != null)
                {
                    return Redirect(Session["tempLink"] as string);
                }
                switch (user.Permission_ID)
                {
                    case "1":
                        return Redirect("/Accounts/Index");
                    case "2":
                        return Redirect("/Employees/ViewTimeUsed");
                    case "3":
                        return Redirect("/TimeKeeping/Check");
                    case "4":
                        return Redirect("/TimeKeeping/Check");
                    default:
                        return Redirect("/Account/Login");
                }
            }
            else
            {
                TempData["LoginFailed"] = "Email hoặc Mật khẩu không chính xác!";
            }
            return Redirect("~/accounts/login");
        }
        //post: Accounts/LogOut
        
        public ActionResult LogOut()
        {
            Session["user_login"] = null;
            return Redirect("~/accounts/login");
        }
        // GET: Accounts/Create
        public ActionResult Create()
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                Session["tempLink"] = "~/Accounts/Create";
                return Redirect("~/accounts/login");
            }
            //không có quyền admin thì trả về trang lỗi
            if (((Account)Session["user_login"]).Permission_ID != "1")
            {
                return Redirect("~/Default/Error");
            }
            ViewBag.Permission_ID = db.Permissions.ToList();
            ViewBag.Employee_ID = db.Employees.ToList();
            return View();
        }

        // POST: Accounts/CreatePost
        [HttpPost]
        public ActionResult CreatePost()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            //không có quyền admin thì trả về trang lỗi
            if (((Account)Session["user_login"]).Permission_ID != "1")
            {
                return Redirect("~/Default/Error");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    string email = Request.Form["account"].ToString();
                    if (db.Accounts.FirstOrDefault(a => a.Email == email) != null)
                    {
                        return Redirect("~/Accounts/Create");
                    }
                    Employee employee = new Employee();
                    employee.FullName = Request.Form["employee"].ToString();
                    employee.DaysUsed = 0;
                    employee.HoursUsed = 0;
                    Account account = new Account();
                    account.Email = email;
                    account.Password = Request.Form["password"].ToString();
                    account.Permission_ID = Request.Form["permission"].ToString();
                    account.Employee_ID = employee.ID;

                    db.Employees.Add(employee);
                    db.Accounts.Add(account);
                    db.SaveChanges();
                    return Redirect("~/Accounts/Index");
                    
                }
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }
            return Redirect("~/Accounts/Create");
        }

        // GET: Accounts/ResetPassword
        public ActionResult ResetPassword()
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                Session["tempLink"] = "~/Accounts/ResetPassword";
                return Redirect("~/accounts/login");
            }
            //không có quyền admin thì trả về trang lỗi
            if (((Account)Session["user_login"]).Permission_ID != "1")
            {
                return Redirect("~/Default/Error");
            }
            return View(db.Accounts.ToList());
        }
        // POST: Accounts/ResetPasswordPost
        [HttpPost]
        public ActionResult ResetPasswordPost()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            //không có quyền admin thì trả về trang lỗi
            if (((Account)Session["user_login"]).Permission_ID != "1")
            {
                return Redirect("~/Default/Error");
            }

            int id = int.Parse(Request["account"]);
            string pass = Request["password"].ToString();
            var acc = db.Accounts.Single(a => a.ID == id);
            try
            {
                acc.Password = pass;
                db.Entry(acc).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ResetPasswordSuccess"] = "Mật khẩu đã được thay đổi";
            }
            catch (Exception)
            {
                TempData["ResetPasswordFailed"] = "Có lỗi xảy ra trong việc reset. Hãy thử thực hiện lại!";
            }
                return Redirect("~/Accounts/ResetPassword");
        }

        // GET: Accounts/ChangePermission
        public ActionResult ChangePermission()
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                Session["tempLink"] = "~/Accounts/ChangePermission";
                return Redirect("~/accounts/login");
            }
            //không có quyền admin thì trả về trang lỗi
            if (((Account)Session["user_login"]).Permission_ID != "1")
            {
                return Redirect("~/Default/Error");
            }
            var list = db.Accounts;
            ViewBag.Permission_ID = db.Permissions.ToList();
            return View(list.ToList());
        }
        // POST: Accounts/ChangePermissionPost
        [HttpPost]
        public ActionResult ChangePermissionPost()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            int id = int.Parse(Request["employee"].ToString());
            Account acc = db.Accounts.FirstOrDefault(a => a.ID == id);
            try
            {
                acc.Permission_ID = Request["permission"].ToString();
                db.Entry(acc).State = EntityState.Modified;
                db.SaveChanges();
                TempData["ChangePermissionSuccess"] = "Thay đổi chức vụ thành công!";

            }
            catch (Exception)
            {
                TempData["ChangePermissionFailed"] = "Không thể thay đổi. Hãy thử lại!";
            }
            return Redirect("~/Accounts/ChangePermission");
        }

        // GET: Accounts/ChangePassword
        public ActionResult ChangePassword()
        {
            if (Session["user_login"] == null)
            {
                //link tạm để sau khi đăng nhập sẽ đi tới link này
                Session["tempLink"] = "~/Accounts/ChangePassword";
                return RedirectToAction("login");
            }
            return View();
        }
        // POST: Accounts/ChangePasswordPost
        public ActionResult ChangePasswordPost()
        {
            if (Session["user_login"] == null)
            {
                return RedirectToAction("login");
            }
            //không có quyền admin thì trả về trang lỗi
            if (((Account)Session["user_login"]).Permission_ID != "1")
            {
                return Redirect("~/Default/ErrorDB");
            }

            Account acc = Session["user_login"] as Account;
            if (acc.Password== Request["old_password"].ToString())
            {
                acc.Password = Request["new_password"].ToString();
                try
                {
                    db.Entry(acc).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["ChangePasswordSuccess"] = "Thay đổi mật khẩu thành công!";
                }
                catch (Exception)
                {
                    TempData["ChangePasswordFailed"] = "Thay đổi mật khẩu không thành công. Hãy thử lại!";
                }
            }
            else
            {
                TempData["ChangePasswordDifferent"] = "Mật khẩu cũ không chính xác. Hãy thử lại!";
            }
            
            return RedirectToAction("ChangePassword");
        }

        // POST: Accounts/Delete/5
        public ActionResult Delete(int id)
        {
            if (Session["user_login"] == null)
            {
                return RedirectToAction("login");
            }
            //không có quyền admin thì trả về trang lỗi
            if (((Account)Session["user_login"]).Permission_ID != "1")
            {
                return Redirect("~/Default/ErrorDB");
            }
            Account acc = db.Accounts.FirstOrDefault(a => a.ID == id);
            Employee emp = db.Employees.FirstOrDefault(e => e.ID == acc.Employee_ID);

            try
            {
                db.Employees.Remove(emp);
                db.Accounts.Remove(acc);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Redirect("~/Default/ErrorDB");
            }
            return Redirect("~/Accounts/Index");
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
