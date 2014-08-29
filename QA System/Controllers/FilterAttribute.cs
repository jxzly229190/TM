using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QA_System.Controllers
{ 
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionExpireFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            var user = session["session_ID"];
            if (user == null)
            {
                //send them off to the login page
                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("http://10.10.73.201/Home/Login");
                session.RemoveAll();
                session.Clear();
                session.Abandon();
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }           
        }
    }  
}
