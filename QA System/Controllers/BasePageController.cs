using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QA_System.Controllers
{
    public class BasePageController : Controller
    {
        //
        // GET: /BasePage/
        public static string SESSION_USERNAME = "session_userName";
        public static string SESSION_USERID = "session_ID";
        public static string SESSION_AttachmentDir = "session_AttachmentDir";

        public static int PAGESIZE = 25;

        public virtual void OnInit()
        {
            if (HttpContext.Session[SESSION_USERID] != null)
            {

                ViewBag.UserName = this.HttpContext.Session[SESSION_USERNAME].ToString();
                ViewBag.UserID = this.HttpContext.Session[SESSION_USERID].ToString();
                if (this.HttpContext.Request.QueryString["requestID"] != null)
                {
                    ViewBag.requestID = this.HttpContext.Request.QueryString["requestID"].ToString();
                }
            }   
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            requestContext.HttpContext.Response.Expires = 0;
            requestContext.HttpContext.Response.CacheControl = "no-cache";
            base.Initialize(requestContext);

            this.OnInit();
        }
        
        
    }
}
