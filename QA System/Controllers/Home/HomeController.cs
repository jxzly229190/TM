using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using QA_System.Classes;
using QA_System.DataProvider;
using QA_System.Models;

namespace QA_System.Controllers
{
    public class HomeController : BasePageController
    {
        [SessionExpireFilter]
        public ActionResult Index()
        {
            ViewBag.Message = "Internal QA System used to tracking all works.";

            TestProvider testProvider = new TestProvider();
            DataBase db = new DataBase();
            //get counts of untest
            int sumTest = testProvider.GetTestUnResolveNum(Session[SESSION_USERID].ToString(), (int)QA_System.Models.Test.Results.Pass);
            //get counts of requests with untests
            int sumRequest = db.GetRequestUnResolveNum(Session[SESSION_USERID].ToString(), (int)QA_System.Models.Test.Results.Pass);

            if (sumRequest > 0){
                ViewBag.ShowMsg = string.Format("Your have <font color='red'> {0} </font> {1} unpass.", sumRequest, sumRequest > 1 ? "requests" : "request");
            }
            if (sumTest > 0){
                ViewBag.ShowMsg += string.Format("<br/>Your have <font color='red'> {0} </font> {1} untest.", sumTest, sumTest > 1 ? "tests" : "test");
            }          

            return View();
        }

        public ActionResult Login(string uid)
        {
            bool isLogin = setSession(uid);
            if (isLogin)
                Response.Redirect("~/Home/Index", false);
            else
                Response.Redirect("~/Error/Index", false);

            return View();
        }

        private bool setSession(string uid)
        {
            UserProvider user = new UserProvider();
            bool validateUser = false;
            if (!string.IsNullOrEmpty(uid))
            {
                string userName = string.Empty;
                if (user.GetUserInfo(uid, out userName))
                {
                    Session[SESSION_USERID] = uid;
                    Session[SESSION_USERNAME] = userName;
                    Session[SESSION_AttachmentDir] = AttachmentDir();
                    validateUser = true;
                }
            }
            return validateUser;
        }

        private string AttachmentDir()
        {
            string attachmentDir = this.Request.PhysicalApplicationPath + "Attachment\\";
            if (!Directory.Exists(attachmentDir))
            {
                Directory.CreateDirectory(attachmentDir);
            }
            return attachmentDir;
        }

        
    }
}
