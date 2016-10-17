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
    public class GoOutLetterController : Controller
    {
        private QLNghiPhepEntities1 db = new QLNghiPhepEntities1();
        // GET: GoOutLetter/Index_Manager
        public ActionResult Index_Manager()
        {
            var goOutLetter = db.GoOutLetters.Include(a => a.Account.Employee);
            return View(goOutLetter.ToList());
        }
        // GET: GoOutLetter/Index_Employee
        public ActionResult Index_Employee()
        {
            var goOutLetter = db.GoOutLetters.Include(a => a.Account.Employee);
            return View(goOutLetter.ToList());
        }
        // GET: GoOutLetter/Create
        public ActionResult Create()
        {
            return View();
        }
        // GET: GoOutLetter/ViewGoOutLetter
        public ActionResult ViewGoOutLetter_SuperManager()
        {
            var goOutLetter = db.GoOutLetters.Include(a => a.Account.Employee);
            return View(goOutLetter.ToList());
        }
    }
}