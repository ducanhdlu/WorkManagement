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
        // GET: Timekeeping
        public ActionResult Index()
        {
            return View();
        }
        // GET: Check
        public ActionResult Check()
        {
            return View();
        }

        // GET: ManagerToday
        public ActionResult ManagerToday()
        {
            var list = db.Timekeepings;
            return View(list.ToList());
        }

        // GET: ManagerOption
        public ActionResult ManagerOption()
        {
            
            return View();
        }

        // GET: Show a month of a employee
        public ActionResult ShowInMonthOneEmployee()
        {

            return View();
        }
    }
}