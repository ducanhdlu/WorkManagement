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
        private QLNghiPhepEntities db = new QLNghiPhepEntities();
        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(a => a.Account);
            return View(employees.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}