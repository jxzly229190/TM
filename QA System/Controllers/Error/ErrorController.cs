using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QA_System.Classes;

namespace QA_System.Controllers.Login
{
    public class ErrorController : Controller
    {
        //
        // GET: /Login/        

        public ActionResult Index()
        {
            ViewBag.Message = "Error";
            return View();
        }
    }
}
