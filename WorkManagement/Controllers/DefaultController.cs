using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkManagement.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default/Welcome
        public ActionResult Welcome()
        {
            return View();
        }
        // GET: Default/Error
        public ActionResult Error()
        {
            return View();
        }
        // GET: Default/ErrorDB
        public ActionResult ErrorDB()
        {
            return View();
        }
    }
}