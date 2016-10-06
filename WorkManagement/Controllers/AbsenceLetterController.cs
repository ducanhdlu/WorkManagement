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
        // GET: AbsenceLetter
        public ActionResult Index_Manager()
        {
            var absenceLetters = db.AbsenceLetters.Include(a => a.Account.Employee);
            return View(absenceLetters.ToList());
        }
    }
}