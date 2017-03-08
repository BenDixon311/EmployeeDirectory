using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeDirectory.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Employees");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Description";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Information";

            return View();
        }
    }
}