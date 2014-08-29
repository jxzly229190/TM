using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QA_System.Models;
using QA_System.Classes;
using QA_System.DataProvider;

namespace QA_System.Controllers
{
    public class PartialViewController : BasePageController
    {
        //
        // GET: /PartialView/

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult RequestView(object requestModel)
        {
            RequestTestModels requestTest = new RequestTestModels();
            try
            {
                requestTest = requestModel as RequestTestModels;
            }
            catch { }

            return PartialView("RequestView", requestTest);
        }

        public PartialViewResult TestIssueView(object requestModel)
        {
            RequestTestModels requestTest = new RequestTestModels();
            try
            {
                requestTest = requestModel as RequestTestModels;
            }
            catch { }

            return PartialView("TestIssueView", requestTest);
        }

    }
}
