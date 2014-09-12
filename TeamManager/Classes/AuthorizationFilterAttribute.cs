using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TeamManager.Classes
{
    public class VoteAuthAttribute : ActionFilterAttribute
    {
        private AuthRole role;
        private AuthType type;

        public VoteAuthAttribute(AuthType accessAuthType = AuthType.EqualsAndGreatThan, AuthRole role = AuthRole.PM)
        {
            this.type = accessAuthType;
            this.role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userRole = filterContext.HttpContext.User.Identity.Name.Split(',')[2];

            var authRole = ConvertRole(userRole);
            switch (type)
            {
                case AuthType.Equals:
                    if (role != authRole)
                    {
                        filterContext.Result = new RedirectResult("/Common/VoteAuthFail");
                    }
                    break;
                case AuthType.EqualsAndGreatThan:
                    if (authRole < role)
                    {
                        filterContext.Result = new RedirectResult("/Common/VoteAuthFail");
                    }
                    break;
                case AuthType.EqualsAndLessThan:
                    if (authRole > role)
                    {
                        filterContext.Result = new RedirectResult("/Common/VoteAuthFail");
                    }
                    break;
            }


            base.OnActionExecuting(filterContext);
        }

        private AuthRole ConvertRole(string roleId)
        {
            switch (roleId)
            {
                case "SE":
                    return AuthRole.SE;
                case "SSE":
                    return AuthRole.SSE;
                case "UI":
                    return AuthRole.UI;
                case "Trainee":
                    return AuthRole.Trainee;
                case "TL":
                    return AuthRole.TL;
                case "PM":
                    return AuthRole.PM;
                default:
                    return AuthRole.SE;
            }
        }
    }

    public enum AuthType
    {
        Equals//
        ,
        EqualsAndLessThan //Less than
            , EqualsAndGreatThan
    }

    public enum AuthRole
    {
        SE = 0, SSE = 1, UI = 2, Trainee = 3, TL = 10, PM, PMO
    }
}