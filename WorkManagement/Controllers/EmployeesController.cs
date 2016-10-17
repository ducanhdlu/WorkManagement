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
        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees;
            return View(employees.ToList());
        }
        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }
        // GET: Employees/ViewTimeUsed
        public ActionResult ViewTimeUsed()
        {
            ViewBag.DayOffInYear = db.DefaultValues.Where(d => d.ID.Equals(1));
            var employees = from e in db.Employees
                            join b in db.BonusDayOffs on e.ID equals b.Employee_ID
                            select e; 

            return View(employees);
        }
    }
}