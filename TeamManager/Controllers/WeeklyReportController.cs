using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TeamManager.DataProvider;
using TeamManager.Models;
using TeamManager.Classes;

namespace TeamManager.Controllers
{
    public class WeeklyReportController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private ReportFunction RF = new ReportFunction();


        private List<SelectListItem> GetGroupName()
        {
            var name = User.Identity.Name.Split(',')[0];

            var groupName = (from g in db.UserRoles select g.GroupName).Distinct().ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (var gn in groupName)
            {
                listItem.Add(new SelectListItem { Text = gn.ToString(), Value = gn.ToString() });
            }
            return listItem;
        }
        //
        // GET: /WeeklyReport/

        public ActionResult Index()
        {
            List<SelectListItem> listItem = GetGroupName();

            ViewData["ListItem"] = new SelectList(listItem, "Value", "Text", "");

            DateTime startTime = DateTime.Today.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            DateTime endTime = DateTime.Today.Date;

            ViewBag.searchFrom = startTime.ToShortDateString();
            ViewBag.searchTo = endTime.ToShortDateString();

            List<Tuple<string, string, string>> tuples = GetDisplayData(startTime, endTime);
            return View(tuples);
            
        }

        private List<Tuple<string, string, string>> GetDisplayData(DateTime startTime, DateTime endTime)
        {
            DateTime endDate = endTime.AddDays(1);
            var reports = (from a in db.DailyReports
                           where a.AddDate >= startTime && a.AddDate <= endDate
                           orderby a.AddDate
                           group a by a.UserID into c
                           orderby c.Key
                           select c).ToList();

            List<Tuple<string, string, string>> tuples = new List<Tuple<string, string, string>>();
            foreach (var c in reports)
            {
                string userID = c.Key;
                string name = (from d in db.Users
                               where d.UserID == userID
                               select d.UserName).FirstOrDefault();

                //build the latest WIP report
                var uncompletedReport = (from a in db.DailyReports
                                         where a.AddDate >= endTime && a.AddDate <= endDate && a.UserID == userID
                                         select a).FirstOrDefault();
                string uncompleted = string.Empty;
                if (uncompletedReport != null)
                {
                    ReportItems uncompletedReportItems = new ReportItems();
                    uncompletedReportItems.FromXML(uncompletedReport.Report);
                    string str = RF.GetWIPReport(uncompletedReportItems);
                    if (str != "")
                    {
                        uncompleted = str;
                    }
                }

                //build Finished report
                StringBuilder builderF = new StringBuilder();

                foreach (var item in c)
                {
                    ReportItems reportItems = new ReportItems();
                    reportItems.FromXML(item.Report);
                    string str = RF.GetFinishedReport(reportItems);
                    if (str != "")
                    {
                        builderF.Append(item.AddDate + "</br>");
                        builderF.Append(str);
                    }
                }
                string completed = builderF.ToString();
                var tuple = new Tuple<string, string, string>(name, completed, uncompleted);
                tuples.Add(tuple);
            }
            return tuples;
        }

        private List<Tuple<string, string, string>> GetDisplayData(string groupName, DateTime startTime, DateTime endTime)
        {
            DateTime endDate = endTime.AddDays(1);
            List<Tuple<string, string, string>> tuples = new List<Tuple<string, string, string>>();
            
            var reports = (from a in db.DailyReports
                            join b in db.UserRoles on a.UserID equals b.UserID
                            where a.AddDate >= startTime && a.AddDate <= endDate && b.GroupName == groupName
                            orderby a.AddDate
                            group a by a.UserID into c
                            orderby c.Key
                            select c).ToList();
                           
            foreach (var c in reports)
            {
                string userID = c.Key;
                string name = (from d in db.Users
                                where d.UserID == userID
                                select d.UserName).FirstOrDefault();


                //build the latest WIP report
                var uncompletedReport = (from a in db.DailyReports
                                            where a.AddDate >= endTime && a.AddDate <= endDate && a.UserID == userID
                                            select a).FirstOrDefault();
                string uncompleted = string.Empty;
                if (uncompletedReport != null)
                {
                    ReportItems uncompletedReportItems = new ReportItems();
                    uncompletedReportItems.FromXML(uncompletedReport.Report);
                    string str = RF.GetWIPReport(uncompletedReportItems);
                    if (str != "")
                    {
                        uncompleted = str;
                    }
                }

                //build Finished report
                StringBuilder builderF = new StringBuilder();
                foreach (var item in c)
                {
                    ReportItems reportItems = new ReportItems();
                    reportItems.FromXML(item.Report);
                    string str = RF.GetFinishedReport(reportItems);
                    if (str != "")
                    {
                        builderF.Append(item.AddDate + "</br>");
                        builderF.Append(str);
                    }
                }


                string completed = builderF.ToString();
                var tuple = new Tuple<string, string, string>(name, completed, uncompleted);
                tuples.Add(tuple);
            }
               
            return tuples;
        }

        [HttpPost]
        public ActionResult Search(string groupName, string searchFrom, string searchTo)
        {
            DateTime startTime = Convert.ToDateTime(searchFrom);
            DateTime endTime = Convert.ToDateTime(searchTo);
            if (groupName == "All")
            {
                return PartialView("SearchPartial", GetDisplayData(startTime, endTime));
            }
            else
            {
                return PartialView("SearchPartial", GetDisplayData(groupName, startTime, endTime));
            }
           
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}