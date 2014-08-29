using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TeamManager.DataProvider;
using TeamManager.Models;
using TeamManager.Classes;

namespace TeamManager.Controllers
{
    public class BreakOffManagerController : Controller
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

            return View(breakOffTable(from, to, ""));
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
            return PartialView("BreakOffTable", breakOffTable(from, to, status));
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
            var list = breakOffTable(from, to, status);//.GroupBy(o=>o.user.UserName);

            //open excel template file  
            FileStream fileOne = new FileStream(Server.MapPath("/Content/Template.xls"), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wbOne = new HSSFWorkbook(fileOne);

            //get first sheet  
            HSSFSheet sheet = (HSSFSheet)wbOne.GetSheetAt(1);

            //Insert data from second row    
            int startRow = 1;

            //Insert data 
            foreach (var item in list)
            {
                HSSFRow rowOne = (HSSFRow)sheet.CreateRow(startRow);
                String userName = item.user.UserName;
                rowOne.CreateCell(0).SetCellValue(userName);

                String breakOffFrom = item.breakoff.BreakOffFrom.ToString();
                rowOne.CreateCell(1).SetCellValue(breakOffFrom);

                String breakOffTo = item.breakoff.BreakOffTo.ToString();
                rowOne.CreateCell(2).SetCellValue(breakOffTo);

                String cutFrom = item.breakoff.CutFrom;
                rowOne.CreateCell(3).SetCellValue(cutFrom);

                String time = item.time;
                rowOne.CreateCell(4).SetCellValue(time);

                String bfStatus = item.breakoff.Status;
                rowOne.CreateCell(5).SetCellValue(bfStatus);

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
            return File(stream, "application/vnd.ms-excel", String.Format("BreakOff{0}_{1}_{2}.xls",
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

            if (!mail.BreakOffApprove(id, User.Identity.Name.Split(',')[0]))
            {
                return HttpNotFound();
            }

            return PartialView("BreakOffTable", breakOffTable(from, to, status));
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

            if (!mail.BreakOffCancel(id, User.Identity.Name.Split(',')[0]))
            {
                return HttpNotFound();
            }

            return PartialView("BreakOffTable", breakOffTable(from, to, status));
        }

        //public ActionResult Email(Guid id)
        //{
        //    StringBuilder email = new StringBuilder();

        //    var who = db.BreakOffs.Where(o => o.BreakOffGuid == id).FirstOrDefault();
        //    var whoName = db.Users.Where(o => o.UserID == who.UserID).FirstOrDefault();
        //    if (who.UserID == "yemol")
        //    {
        //        ViewBag.emailTo = "Ronstar.Luan@beyondsoft.com";
        //        ViewBag.emailCc = "";
        //        email.Append("Hi Ronstar, \r\n\r\n");
        //    }
        //    else
        //    {
        //        ViewBag.emailTo = "wenzhi.hao@beyondsoft.com";
        //        ViewBag.emailCc = "yvone.zhang@beyondsoft.com";
        //        email.Append("Hi Wen, \r\n\r\n");
        //    }
        //    ViewBag.subject = "Over Time";
        //    string hourstr = " hours";
        //    if (who.Hours == 1)
        //        hourstr = " hour";
        //    hourstr = who.Hours.ToString() + hourstr;

        //    email.Append(whoName.UserName + " work overtime " + hourstr + " on " + who.OnDate.ToString("MM'/'dd'/'yyyy") + ".");
        //    email.Append("\r\n\r\nThanks,\r\n\r\n\r\nYemol");

        //    ViewBag.content = email.ToString();
        //    ViewBag.flag = true;

        //    return PartialView("SendEmail");
        //}

        /// <summary>
        /// Get BreakOffTable data
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static IEnumerable<BreakOffTable> breakOffTable(DateTime from, DateTime to, string status)
        {
            List<BreakOff> breakOff;
            List<BreakOffTable> breakOffManager = new List<BreakOffTable>();
            CaculateCost caculate = new CaculateCost();

            from = new DateTime(from.Year, from.Month, from.Day);            
            to = to.AddDays(1);
            using (var db = new TeamManage_Entities())
            {
                if (status == String.Empty)
                {
                    breakOff = db.BreakOffs.Where(o => o.BreakOffFrom >= from && o.BreakOffTo <= to)
                                                        .OrderByDescending(o => o.BreakOffFrom).ToList();
                }
                else
                {
                    breakOff = db.BreakOffs.Where(o => o.BreakOffFrom >= from && o.BreakOffTo <= to)
                                                        .Where(o => o.Status == status)
                                                        .OrderByDescending(o => o.BreakOffFrom).ToList();
                }

                foreach (var item in breakOff)
                {
                    var user = db.Users.Where(o => o.UserID == item.UserID).FirstOrDefault();
                    BreakOffTable bf = new BreakOffTable();
                    bf.breakoff = item;
                    bf.user = user;
                    bf.time = caculate.Caculate(item.BreakOffFrom, item.BreakOffTo).ToString() + "H";
                    breakOffManager.Add(bf);
                }
            }
            return breakOffManager;
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

        //public static List<SelectListItem> GetMonthList()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();

        //    for (int i = 1; i < 13; i++)
        //    {
        //        list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //    }

        //    return list;
        //}

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
            IEnumerable<BreakOffTable> breakOffManager = new List<BreakOffTable>();
            breakOffManager = breakOffTable(from, to, status);
            var query = from tr in breakOffManager
                        orderby Utils.GetPropertyValue(tr, tr.breakoff, sort) descending
                        select tr;
            if (sortDirection == "ascending")
            {
                query = from tr in breakOffManager
                        orderby Utils.GetPropertyValue(tr, tr.breakoff, sort) ascending
                        select tr;
            }
            if (sort == "UserName")
            {
                query = from tr in breakOffManager
                        orderby Utils.GetPropertyValue(tr, tr.user, sort) descending
                        select tr;
                if (sortDirection == "ascending")
                {
                    query = from tr in breakOffManager
                            orderby Utils.GetPropertyValue(tr, tr.user, sort) ascending
                            select tr;
                }
            }
            return PartialView("BreakOffTable", query);
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