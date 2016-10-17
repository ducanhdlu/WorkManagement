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
    public class AbsenceLetterController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();
        // GET: AbsenceLetter/Index_Manager
        public ActionResult Index_Manager()
        {
            var absenceLetters = db.AbsenceLetters.Include(a => a.Account.Employee);
            return View(absenceLetters.ToList());
        }
        // GET: AbsenceLetter/Index_Employee
        public ActionResult Index_Employee()
        {
            var absenceLetters = db.AbsenceLetters.Include(a => a.Account.Employee);
            return View(absenceLetters.ToList());
        }
        // GET: AbsenceLetter/Create
        public ActionResult Create()
        {
            return View();
        }
        // GET: AbsenceLetter/ViewAbsenceLetter
        public ActionResult ViewAbsenceLetter_SuperManager()
        {
            var absenceLetters = db.AbsenceLetters.Include(a => a.Account.Employee);
            return View(absenceLetters.ToList()); return View();
        }
    }
}