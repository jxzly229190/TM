using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TeamManager.Classes;
using TeamManager.DataProvider;
using TeamManager.Models;

namespace TeamManager.Controllers
{
    public class DailyProjectController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();

        //
        // GET: /DailyProject/

        public ActionResult Index()
        {
            DateTime dateFrom = DateTime.Now.Date;
            DateTime dateTo = DateTime.Now.Date.AddDays(1);

            var dailyProjects = (from addtime in db.DailyProjectReports
                                where addtime.AddDate >= dateFrom &&
                                 addtime.AddDate <= dateTo
                                select addtime).ToList();

            List<Tuple<string, string>> tuples = new List<Tuple<string, string>>();
            foreach (var item in dailyProjects)
            {
                string name = (from d in db.Users
                               where d.UserID == item.UserID
                               select d.UserName).FirstOrDefault();

                ReportFiles reportFiles = new ReportFiles();
                reportFiles.FromXML(item.Report);

                string report = getFiles(reportFiles);
                string time = item.AddDate.ToString();
                var tuple = new Tuple<string, string>(name, report);
                tuples.Add(tuple);

            }

            return View(tuples);
        }

        private string getFiles(ReportFiles items)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder temp = new StringBuilder();

            int count = 1;
            int marches = 0;

            if (items.Files.Count > 0)
            {
                temp.Append("Project:&nbsp" + items.Project + "<br/>");
                temp.Append("Status:&nbsp" + items.Status + "<br/>");
                temp.Append("Files:<br/>");
                foreach (ReportFile item in items.Files)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Guid + "<br/>");
                    temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp" + item.FileName + "<br/>");
                    marches++;
                }
            }
            if (marches > 0)
            {
                builder.Append(temp);
            }
            return builder.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}