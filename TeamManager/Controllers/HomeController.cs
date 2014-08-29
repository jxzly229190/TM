using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TeamManager.DataProvider;
using WebMatrix.WebData;
using TeamManager.Controllers;
using TeamManager.Classes;
using TeamManager.Models;
using System.Security.Cryptography;

namespace TeamManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ReportFunction rf = new ReportFunction();

        [Authorize]
        public ActionResult Index()
        {
            //if (Session["IsTrainee"] == null)
            //{
            //    FormsAuthentication.SignOut();
            //    return RedirectToAction("Index", "Home");
            //}
            List<Tuple<string, string,string>> tuples = new List<Tuple<string, string,string>>();
            using (var db = new TeamManage_Entities())
            {
                DateTime fromDate = DateTime.Now.Date;
                DateTime toDate = fromDate.AddDays(1);
                string userid = User.Identity.Name.Split(',')[0];
                DailyReport dailyreport = db.DailyReports.Where(c => c.UserID == userid && c.AddDate >= fromDate && c.AddDate <= toDate).FirstOrDefault();
                if (dailyreport != null)
                {
                    ReportItems reportItems = new ReportItems();
                    reportItems.FromXML(dailyreport.Report);

                    ViewData["MyTodayReport"] = rf.GetReport(reportItems);
                }
                else
                {
                    ViewData["MyTodayReport"] = "";
                }
                //var query = (from u in db.Users
                //             where u.IsTrainee == false
                //             select u.UserID).Except(
                //            from d in db.DailyReports
                //            where d.AddDate >= fromDate && d.AddDate <= toDate
                //            select d.UserID).ToList();

                var query=(from u in db.Users
                           where u.IsTrainee==false && u.IsDeparted==false
                           orderby u.UserName
                           select u).ToList();
                
                foreach (var item in query)
                {
                    string name = item.UserName;
                    string status = "";

                    var reportUser=(from d in db.DailyReports
                              where d.UserID==item.UserID && d.AddDate>=fromDate && d.AddDate<=toDate 
                              select d.UserID).FirstOrDefault();
                    if (reportUser != null)
                    {
                        status = "Reported";
                    }
                    else 
                    {
                        status = "Not Reported";
                    }

                    //string name=(from u in db.Users
                    //            where u.UserID==item
                    //            select u.UserName).FirstOrDefault();
                    Tuple<string, string, string> tuple = new Tuple<string, string, string>(item.UserID, name, status);
                    tuples.Add(tuple);
                }

                //var result = from a in db.Users
                //             from q in query
                //             where a.UserID == q
                //             select a.UserName;

                //ViewData["UserName"] = query;

                
            }
            DateTime from = DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            DateTime to = DateTime.Now;
            ViewData["WeekBreakOff"] = BreakOffManagerController.breakOffTable(from, to, "Approve");
            ViewData["traineeAttendance"] = TraineeAttendanceManagerController.traineeTable(from, to, "");
            from = new DateTime(2013,12,31);
            ViewData["BreakOffRequest"] = BreakOffManagerController.breakOffTable(from, to, "Ask");
            ViewData["OverTimeRequest"] = OverTimeManagerController.overTimeTable(from, to, "Ask");
            return View(tuples);
        }

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Models.Users user, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "MD5").ToLower();               

                MD5Hash md5Hash = new MD5Hash();
                MD5 md5 = MD5.Create();
                string hash = md5Hash.GetMd5Hash(md5, user.Password);

                if (user.IsValid(user.UserID, hash))
                {                   
                    //FormsAuthentication.SetAuthCookie(user.UserID, false);
                    using (var db = new TeamManage_Entities())
                    {
                        var userRole = db.Users.Where(o => o.UserID == user.UserID).FirstOrDefault();
                        string Role = null;
                        if (userRole.IsTrainee == true)
                        {
                            Role = "Trainee";
                        }
                        else
                        {
                            Role = db.UserRoles.Where(o => o.UserID == user.UserID).FirstOrDefault().RoleName;
                        }
                        
                        FormsAuthentication.SetAuthCookie(string.Format("{0},{1}",user.UserID,Role), false);
                    
                    }
                    //return Redirect(returnUrl);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                   
                }

                else if (user.IsValid(user.UserID, user.Password))
                {
                    var database = new TeamManage_Entities();
                    var person = database.Users.Where(u => u.UserID == user.UserID).FirstOrDefault();
                    person.LoginPassword = hash;
                    database.SaveChanges();
                     using (var db = new TeamManage_Entities())
                    {
                        var userRole = db.Users.Where(o => o.UserID == user.UserID).FirstOrDefault();
                        string Role = null;
                        if (userRole.IsTrainee == true)
                        {
                            Role = "Trainee";
                        }
                        else
                        {
                            Role = db.UserRoles.Where(o => o.UserID == user.UserID).FirstOrDefault().RoleName;
                        }
                        
                        FormsAuthentication.SetAuthCookie(string.Format("{0},{1}",user.UserID,Role), false);
                    
                    }
                    //return Redirect(returnUrl);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Remove("IsTrainee");
            return RedirectToAction("Index", "Home");
        }
    }
    
}
