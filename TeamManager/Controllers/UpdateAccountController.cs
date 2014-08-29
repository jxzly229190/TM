using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TeamManager.Classes;
using TeamManager.DataProvider;

namespace TeamManager.Controllers
{
    public class UpdateAccountController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();

        //
        // GET: /UpdateAccout/

        public ActionResult Index()
        {
            User user = db.Users.Find(User.Identity.Name.Split(',')[0]);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /UpdateAccout/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Index(User user)
        {
            MD5Hash md5Hash = new MD5Hash();
            MD5 md5 = MD5.Create();
           
            var newPassword = Request.Form["newLoginPassword"];
            var confirmPassword = Request.Form["confirmLoginPassword"];

            var newEMPassword = Request.Form["newEMPassword"];
            var confirmEMPassword = Request.Form["confirmEMPassword"];
            if (md5Hash.VerifyMd5Hash(md5,newPassword,user.LoginPassword))
            {
                return "The LoginPassword should be different from the previous!";
            }
            //if (newPassword != confirmPassword)
            //{
            //    return "ConfirmPassword is incorrect!";
            //}
            else
            {
                user.UserID = User.Identity.Name.Split(',')[0];
                if (ModelState.IsValid)
                {
                    if (newPassword!="")
                    {                       
                        string hash = md5Hash.GetMd5Hash(md5, newPassword);
                            
                        //string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "MD5");
                        user.LoginPassword = hash;

                    }

                    if (newEMPassword != "")
                    {
                        string hash = md5Hash.GetMd5Hash(md5, newEMPassword);

                        //string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "MD5");
                        user.EMPassword = hash;

                    } 
                  
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return "Update Successfully.";
                }
                return "Update Failed.";
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}