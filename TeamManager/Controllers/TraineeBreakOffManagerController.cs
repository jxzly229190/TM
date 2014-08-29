using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamManager.Classes;
using TeamManager.DataProvider;
using TeamManager.Models;

namespace TeamManager.Controllers
{
    public class TraineeBreakOffManagerController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            DateTime from = DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            DateTime to = DateTime.Now;

            ViewData["statusList"] = GetStatusList();
            //ViewData["monthsList"] = GetMonthList();

            ViewBag.fromTime = from.ToString("MM'/'dd'/'yyyy");
            ViewBag.toTime = to.ToString("MM'/'dd'/'yyyy");
            ViewBag.flag = false;

            return View(traineeBreakOffTable(from, to, ""));
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult Search(DateTime from, DateTime to, string status)
        {
            return PartialView("TraineeBreakOffTable", traineeBreakOffTable(from, to, status));
        }

        /// <summary>
        /// data to export into excel
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult Export(DateTime from, DateTime to, string status)
        {
            //search data   
            var list = traineeBreakOffTable(from, to, status);

            //open excel template file  
            FileStream fileOne = new FileStream(Server.MapPath("/Content/Template.xls"), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wbOne = new HSSFWorkbook(fileOne);

            //get first sheet  
            HSSFSheet sheet = (HSSFSheet)wbOne.GetSheetAt(2);

            //Insert data from second row    
            int startRow = 1;

            //Insert data 
            foreach (var item in list)
            {
                HSSFRow rowOne = (HSSFRow)sheet.CreateRow(startRow);
                String userName = item.user.UserName;
                rowOne.CreateCell(0).SetCellValue(userName);

                String breakOffFrom = item.traineeBreakOff.BreakOffFrom.ToString();
                rowOne.CreateCell(1).SetCellValue(breakOffFrom);

                String breakOffTo = item.traineeBreakOff.BreakOffTo.ToString();
                rowOne.CreateCell(2).SetCellValue(breakOffTo);

                String time = item.time;
                rowOne.CreateCell(3).SetCellValue(time);

                String bfStatus = item.traineeBreakOff.Status;
                rowOne.CreateCell(4).SetCellValue(bfStatus);

                startRow = startRow + 1;
            }

            MemoryStream stream = new MemoryStream();
            wbOne.Write(stream);
            fileOne.Close();

            if (status == String.Empty)
            {
                status = "All";
            }
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", String.Format("TraineeBreakOff{0}_{1}_{2}.xls",
                                                      from.ToString("yyyyMMdd"), to.ToString("yyyyMMdd"), status));
        }

        /// <summary>
        /// Approve
        /// </summary>
        /// <param name="id"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult Approve(Guid id, DateTime from, DateTime to, string status)
        {
            MailEntity mail = new MailEntity();

            if (!mail.TraineeBreakOffApprove(id, User.Identity.Name.Split(',')[0]))
            {
                return HttpNotFound();
            }

            return PartialView("TraineeBreakOffTable", traineeBreakOffTable(from, to, status));
        }

        /// <summary>
        /// Cancel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult Cancel(Guid id, DateTime from, DateTime to, string status)
        {
            MailEntity mail = new MailEntity();

            if (!mail.TraineeBreakOffCancel(id, User.Identity.Name.Split(',')[0]))
            {
                return HttpNotFound();
            }

            return PartialView("TraineeBreakOffTable", traineeBreakOffTable(from, to, status));
        }

        /// <summary>
        /// Get BreakOffTable data
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static IEnumerable<TraineeBreakOffTable> traineeBreakOffTable(DateTime from, DateTime to, string status)
        {
            List<TraineeBreakOff> traineeBreakOff;
            List<TraineeBreakOffTable> traineeBreakOffManager = new List<TraineeBreakOffTable>();
            CaculateCost caculate = new CaculateCost();

            from = new DateTime(from.Year, from.Month, from.Day);
            to = to.AddDays(1);
            using (var db = new TeamManage_Entities())
            {
                if (status == String.Empty)
                {
                    traineeBreakOff = db.TraineeBreakOffs.Where(o => o.BreakOffFrom >= from && o.BreakOffTo <= to)
                                                        .OrderByDescending(o => o.BreakOffFrom).ToList();
                }
                else
                {
                    traineeBreakOff = db.TraineeBreakOffs.Where(o => o.BreakOffFrom >= from && o.BreakOffTo <= to)
                                                        .Where(o => o.Status == status)
                                                        .OrderByDescending(o => o.BreakOffFrom).ToList();
                }

                foreach (var item in traineeBreakOff)
                {
                    var user = db.Users.Where(o => o.UserID == item.UserID).FirstOrDefault();
                    TraineeBreakOffTable tbf = new TraineeBreakOffTable();
                    tbf.traineeBreakOff = item;
                    tbf.user = user;
                    tbf.time = caculate.Caculate(item.BreakOffFrom, item.BreakOffTo).ToString() + "H";
                    traineeBreakOffManager.Add(tbf);
                }
            }
            return traineeBreakOffManager;
        }

        /// <summary>
        /// get statuslist
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetStatusList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "Approve", Value = "Approve" });
            list.Add(new SelectListItem { Text = "Ask", Value = "Ask" });
            list.Add(new SelectListItem { Text = "Cancel", Value = "Cancel" });

            return list;
        }

        /// <summary>
        /// sort Method
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="sortDirection"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult SortTable(string sort, string sortDirection, DateTime from, DateTime to, string status)
        {

            if (string.IsNullOrEmpty(sort))
            {
                sort = "BreakOffFrom";
            }

            if (string.IsNullOrEmpty(sortDirection))
            {
                sortDirection = "ascending";
            }
            ViewBag.sort = sort;
            ViewBag.sortDirection = (sortDirection == "ascending") ? "descending" : "ascending";
            IEnumerable<TraineeBreakOffTable> traineeBreakOffManager = new List<TraineeBreakOffTable>();
            traineeBreakOffManager = traineeBreakOffTable(from, to, status);
            var query = from tr in traineeBreakOffManager
                        orderby Utils.GetPropertyValue(tr, tr.traineeBreakOff, sort) descending
                        select tr;
            if (sortDirection == "ascending")
            {
                query = from tr in traineeBreakOffManager
                        orderby Utils.GetPropertyValue(tr, tr.traineeBreakOff, sort) ascending
                        select tr;
            }
            if (sort == "UserName")
            {
                query = from tr in traineeBreakOffManager
                        orderby Utils.GetPropertyValue(tr, tr.user, sort) descending
                        select tr;
                if (sortDirection == "ascending")
                {
                    query = from tr in traineeBreakOffManager
                            orderby Utils.GetPropertyValue(tr, tr.user, sort) ascending
                            select tr;
                }
            }
            return PartialView("TraineeBreakOffTable", query);
        }

        /// <summary>
        /// dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}