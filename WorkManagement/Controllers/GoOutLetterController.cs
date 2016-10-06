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
        // GET: GoOutLetter
        public ActionResult Index_Manager()
        {
            var goOutLetter = db.GoOutLetters.Include(a => a.Account.Employee);
            return View(goOutLetter.ToList());
        }
    }
}