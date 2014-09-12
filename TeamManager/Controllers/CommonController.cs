using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TeamManager.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/

        public ActionResult VoteAuthFail()
        {
            ViewBag.Message = "Sorry, you have not the perssion to access.";
            return View("Alert");
        }
    }
}
