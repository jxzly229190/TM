using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamManager.DataProvider;
using TeamManager.Models;
using TeamManager.Classes;
using System.IO;

namespace TeamManager.Controllers
{
    public class ReportsController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private ReportFunction RF = new ReportFunction();

        public List<SelectListItem> GetWorkFor()
        {
            string userid = User.Identity.Name.Split(',')[0];
            List<SelectListItem> workFor = db.UserRoles
                .Where(c => c.UserID == userid)
                .Select(c => new SelectListItem
                {
                    Value = c.GroupName,
                    Text = c.GroupName
                }).ToList();

            workFor.Add(new SelectListItem { Value = "Other", Text = "Other" });
           
            return workFor;
        }


        public ActionResult Details()
        {
            string userid = User.Identity.Name.Split(',')[0];
            DateTime dateFrom = DateTime.Now.Date;
            DateTime dateTo = DateTime.Now.Date.AddDays(1);

            ViewBag.searchFrom = dateFrom.ToString("MM/dd/yyyy");

            DailyReport dailyreport = db.DailyReports.Where(c => c.UserID == userid && c.AddDate >=dateFrom && c.AddDate<=dateTo).FirstOrDefault();
            if (dailyreport!=null)
            {
                ReportItems reportItems = new ReportItems();
                reportItems.FromXML(dailyreport.Report);
                ViewBag.Report = RF.GetReport(reportItems);
            }
                        
            return View();
        }

        [HttpPost]
        public ActionResult DailyReportSearch(string searchFrom)
        {
            string userid = User.Identity.Name.Split(',')[0];
            DateTime dateFrom = Convert.ToDateTime(searchFrom);
            DateTime dateTo = dateFrom.AddDays(1);
            DailyReport dailyreport = db.DailyReports.Where(c => c.UserID == userid && c.AddDate >= dateFrom && c.AddDate <= dateTo).FirstOrDefault();
            if (dailyreport != null)
            {
                string name = dailyreport.UserID;

                ReportItems reportItems = new ReportItems();
                reportItems.FromXML(dailyreport.Report);
                ViewBag.Report = RF.GetReport(reportItems);
                return PartialView("DailyReportPartial");
            }
            else
                return Content("<script >alert('You have not report today！');</script >", "text/html");
        }


        public ActionResult Create()
        {
            ViewData["WorkFor"] = GetWorkFor();
            string userid = User.Identity.Name.Split(',')[0];
            DateTime dateFrom = DateTime.Now.Date;
            DateTime dateTo = dateFrom.AddDays(1);
            DailyReport dailyreport = db.DailyReports.Where(c => c.UserID == userid && c.AddDate >= dateFrom && c.AddDate <= dateTo).FirstOrDefault();
            if (dailyreport!=null)
            {
                return RedirectToAction("Edit", new { id = dailyreport.DailyReportID });
            }
            else
               return View();
        }
        [HttpPost]
        public ActionResult Create(Reports reports)
        {
            if (ModelState.IsValid)
            {
                ViewData["WorkFor"] = GetWorkFor();

                int finishedLength = int.Parse(Request.Form["hideFinished"]);
                int wIPLength = int.Parse(Request.Form["hideWIP"]);
                int planningLength = int.Parse(Request.Form["hidePlanning"]);
                int blockingLength = int.Parse(Request.Form["hideBlocking"]);

                ReportItems reportItems = new ReportItems();

                reportItems.Finished = GetReportItems(finishedLength, "Finished");
                reportItems.WIP = GetReportItems(wIPLength, "WIP");
                reportItems.Planning = GetReportItems(planningLength, "Planning");
                reportItems.Blocking = GetReportItems(blockingLength, "Blocking");

                if (reportItems.Finished.Count != 0 || reportItems.WIP.Count != 0 || reportItems.Planning.Count != 0 || reportItems.Blocking.Count != 0)
                {
                    reports.DailyReport.Report = reportItems.ToXML();
                    reports.DailyReport.DailyReportID = Guid.NewGuid();
                    reports.DailyReport.AddDate = DateTime.Now;
                    reports.DailyReport.UserID = User.Identity.Name.Split(',')[0];

                    db.DailyReports.Add(reports.DailyReport);
                    db.SaveChanges();

                    return RedirectToAction("Edit", new { id = reports.DailyReport.DailyReportID });
                }

                ReportFiles reportFiles = new ReportFiles();
                reportFiles.Files = GetFiles();

                if (reportFiles.Files.Count != 0)
                {
                    reportFiles.Status = Request.Form["ProjectStatus_TB"];
                    reportFiles.Project = Request.Form["Project_DDL"];

                    reports.DailyProjectReort.Report = reportFiles.ToXML();

                    reports.DailyProjectReort.DailyProjectReportID = Guid.NewGuid();
                    reports.DailyProjectReort.AddDate = DateTime.Now;
                    reports.DailyProjectReort.UserID = User.Identity.Name.Split(',')[0];
                    reports.DailyProjectReort.Project = Request.Form["Project_DDL"];

                    db.DailyProjectReports.Add(reports.DailyProjectReort);
                    db.SaveChanges();
                }
            }
            return View();
        }


        public ActionResult Edit(Guid id)
        {
            ViewBag.ReportProject  = GetWorkFor();

            string userid = User.Identity.Name.Split(',')[0];
            //var project = db.UserRoles.Where(c => c.UserID == userid).ToList();
            //ViewBag.ReportProject = project;
            DailyReport dailyreport = db.DailyReports.Find(id);
            ViewData["DailyReportID"] = id;
            ReportItems reportItems = new ReportItems();
            reportItems.FromXML(dailyreport.Report);
           
            

            return View(reportItems);
        }
        [HttpPost]
        public ActionResult Edit(ReportItems reportItems)
        {
            if (ModelState.IsValid)
            {
 
                Guid dailyReportID = new Guid(Request.Form["hideID"]); 
                DailyReport dailyreport = db.DailyReports.Find(dailyReportID);

                int finishedLength = int.Parse(Request.Form["hideFinished"]);
                for (int i = 1; i < finishedLength+1; i++)
                {
                    var reportItem = GetReportItem("Finished", i);
                    if (reportItem != null)
                    {
                        reportItems.Finished.Add(reportItem);
                    }
                }

                int wIPLength = int.Parse(Request.Form["hideWIP"]);
                for (int i = 1; i < wIPLength + 1; i++)
                {
                    var reportItem = GetReportItem("WIP", i);
                    if (reportItem != null)
                    {
                        reportItems.WIP.Add(reportItem);
                    }
                }

                int planningLength = int.Parse(Request.Form["hidePlanning"]);
                for (int i = 1; i < planningLength + 1; i++)
                {
                    var reportItem = GetReportItem("Planning", i);
                    if (reportItem != null)
                    {
                        reportItems.Planning.Add(reportItem);
                    }
                }

                int blockingLength = int.Parse(Request.Form["hideBlocking"]);
                for (int i = 1; i < blockingLength + 1; i++)
                {
                    var reportItem = GetReportItem("Blocking", i);
                    if (reportItem != null)
                    {
                        reportItems.Blocking.Add(reportItem);
                    }
                }

                dailyreport.Report=reportItems.ToXML();

                db.Entry(dailyreport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = dailyReportID });
            }
            return View(reportItems);
        }


        private List<ReportItem> GetReportItems(int length, string itemType)
        {
            List<ReportItem> items = new List<ReportItem>();
            for (int i = 1; i < length+1; i++)
            {
                var item=GetReportItem(itemType, i);
                if (item != null)
                {
                    items.Add(item);
                }             
            }
            return items;
        }


        private ReportItem GetReportItem(string itemType, int i)
        {
            ReportItem item = new ReportItem();

            var worktName = string.Format("{0}{1}_TB", itemType, i);
            var descriptionName = string.Format("{0}{1}_TA", itemType, i);
            var projectName = string.Format("{0}{1}_DDL", itemType, i);

            item.Work = Request.Form[worktName];
            item.Description = Request.Form[descriptionName];
            item.Project = Request.Form[projectName];

            if (string.IsNullOrEmpty(item.Work) || item.Work == "Finished " || item.Work == "Working on ")
                return null;
            else
                return item;
        }


        private List<ReportFile> GetFiles()
        {
            List<ReportFile> reportFiles = new List<ReportFile>();
            for (int index = 0; index < this.Request.Files.Count; index++)
            {
                HttpPostedFileBase file = Request.Files[index] as HttpPostedFileBase;
                if (file == null || file.ContentLength == 0) continue;

                ReportFile reportFile = new ReportFile();
                reportFile.Guid = Guid.NewGuid().ToString();

                string mimeType = Request.Files[index].ContentType;
                Stream fileStream = Request.Files[index].InputStream;
                reportFile.FileName = Path.GetFileName(Request.Files[index].FileName);

                int fileLength = Request.Files[index].ContentLength;
                byte[] fileData = new byte[fileLength];
                fileStream.Read(fileData, 0, fileLength);
                fileStream.Close();

                reportFiles.Add(reportFile);
            }
            return reportFiles;
        }


        public ActionResult RP(string item)
        {
            ViewData["WorkFor"] = GetWorkFor();
            return PartialView("ReportPartial",item);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
