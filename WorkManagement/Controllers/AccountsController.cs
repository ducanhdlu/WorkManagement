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
            if (Session["user_login"]==null)
            {
                return Redirect("~/accounts/login");
            }
            var accounts = db.Accounts.Include(a => a.Permission);
            return View(accounts.ToList());
        }

        // GET: Accounts/ResetPassword
        public ActionResult ResetPassword()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            return View();
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
            var user = db.Accounts.SingleOrDefault(u => u.Email == email && u.Password == password);
            
            if (user!=null)
            {
                Session["user_login"] = user as Account;
                
                return Redirect("~/accounts/index");
            }
            else
            {
                //ViewBag.LoginError = "Email hoặc Mật khẩu không chính xác!";
                //ModelState.AddModelError("LoginError", "Email hoặc Mật khẩu không chính xác!");
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
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            
            
            return View();
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

        // GET: Accounts/ChangePassword
        public ActionResult ChangePassword()
        {
            if (Session["user_login"] == null)
            {
                return Redirect("~/accounts/login");
            }
            return View();
        }

        
        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.Permission_ID = new SelectList(db.Permissions, "ID", "Name", account.Permission_ID);
            return View(account);
        }

        
        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
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
