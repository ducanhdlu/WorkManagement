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
        public ActionResult Index()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            var accounts = db.Accounts.Include(a => a.Permission);
            return View(accounts.ToList());
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
            Account user = db.Accounts.SingleOrDefault(u => u.Email == email && u.Password == password);
            
            if (user!=null)
            {
                Session["user_login"] = user;
                
                return Redirect("~/accounts/index");
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
                return Redirect("~/accounts/login");
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
            Employee employee = new Employee();
            employee.FullName= Request.Form["employee"].ToString();
            employee.DaysUsed = 0;
            employee.HoursUsed = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Employees.Add(employee);
                    db.SaveChanges();
                    Account account = new Account();
                    account.Email = Request.Form["account"].ToString();
                    account.Password = Request.Form["password"].ToString();
                    account.Permission_ID = Request.Form["permission"].ToString();
                    account.Employee_ID = employee.ID;
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            db.Accounts.Add(account);
                            db.SaveChanges();
                            return Redirect("~/Accounts/Index");
                        }
                    }
                    catch (Exception)
                    {
                        db.Employees.Remove(employee);
                        db.SaveChanges();
                        
                    }
                }
            }
            catch (Exception)
            {
                
            }

            
            
            return RedirectToAction("~/Accounts/Create");
        }

        // GET: Accounts/ResetPassword
        public ActionResult ResetPassword()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
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
                return Redirect("~/accounts/login");
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
            Account acc = db.Accounts.SingleOrDefault(a => a.ID == id);
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
                    throw;
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
            Account acc = db.Accounts.SingleOrDefault(a => a.ID == id);
            Employee emp = db.Employees.SingleOrDefault(e => e.ID == acc.Employee_ID);

            try
            {
                db.Employees.Remove(emp);
                db.Accounts.Remove(acc);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
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
