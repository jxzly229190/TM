using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamManager.DataProvider;
using TeamManager.Models;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Net;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.Util;

namespace TeamManager.Controllers
{
    public class AppraisalController : Controller
    {

        TeamManage_Entities db = new TeamManage_Entities();

        private List<SelectListItem> GetGroupName()
        {
            var name = User.Identity.Name.Split(',')[0];

            var groupName = from g in db.UserRoles
                            where g.UserID == name
                            select g;
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (var gn in groupName)
            {
                listItem.Add(new SelectListItem { Text = gn.GroupName.ToString(), Value = gn.GroupName.ToString() });
            }
            return listItem;
        }

        private static DateTime GetFriday()
        {
            DateTime dt = DateTime.Now;
            int dayOfWeek = Convert.ToInt32(dt.DayOfWeek.ToString("d"));
            DateTime startWeek = dt.AddDays(1 - ((dayOfWeek == 0) ? 7 : dayOfWeek));
            DateTime friTime = startWeek.AddDays(4).Date;
            return friTime;
        }

        //
        // GET: /Appraisal/

        public ActionResult Create()
        {
            List<SelectListItem> listItem = GetGroupName();

            ViewData["ListItem"] = new SelectList(listItem, "Value", "Text", "");

            return View();


        }

        [HttpPost]
        public ActionResult Create(FormCollection form)
        {

            try
            {
                DateTime friTime = GetFriday();

                string groupName = form["GroupName"];
                var query = (from a in db.UserRoles
                             join b in db.Users on a.UserID equals b.UserID
                             where b.IsDeparted == false && a.RoleID != "PM" && a.GroupName == groupName
                             orderby b.UserName
                             select new { UserID = a.UserID, UserName = b.UserName }).ToList();

                foreach (var member in query)
                {
                    UserAppraisal appraisal = new UserAppraisal()
                    {
                        UserID = member.UserID,
                        UserName = member.UserName,
                        WorkDone = string.IsNullOrEmpty(form[member.UserID + "WorkDone"]) ? 0 : Convert.ToDecimal(form[member.UserID + "WorkDone"]),
                        BugCreated = string.IsNullOrEmpty(form[member.UserID + "BugCreated"]) ? 0 : Convert.ToDecimal(form[member.UserID + "BugCreated"]),
                        WorkQuality = string.IsNullOrEmpty(form[member.UserID + "WorkQuality"]) ? 0 : Convert.ToDecimal(form[member.UserID + "WorkQuality"]),
                        DailyPerformance = string.IsNullOrEmpty(form[member.UserID + "DailyPerformance"]) ? 0 : Convert.ToDecimal(form[member.UserID + "DailyPerformance"]),
                        GroupName = groupName,
                        DateTime = friTime
                    };

                    if (appraisal.WorkDone!=0||appraisal.BugCreated!=0||appraisal.WorkQuality!=0||appraisal.DailyPerformance!=0)
                    {
                        db.UserAppraisals.Add(appraisal);
                        db.SaveChanges();
                    }          
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                Response.Write(dbEx);
            }


            return RedirectToAction("Edit");
        }

       

        public ActionResult Member(string groupName)
        {
            DateTime friTime = GetFriday();

            var query = (from a in db.UserRoles
                         join b in db.Users on a.UserID equals b.UserID
                         where b.IsDeparted == false && a.GroupName == groupName && a.RoleID != "PM"
                         orderby b.UserName
                         select new { UserID = a.UserID, UserName = b.UserName }).Except(
                         from c in db.UserAppraisals
                         where c.GroupName == groupName && c.DateTime == friTime
                         select new { UserID = c.UserID, UserName = c.UserName });
                        
            var members = query.ToList().Select(c => new UserAppraisal { UserID = c.UserID, UserName = c.UserName }).ToList();
            return PartialView("Member", members);
        }

        public ActionResult Edit()
        {
            List<SelectListItem> listItem = GetGroupName();
            ViewData["ListItem"] = new SelectList(listItem, "Value", "Text", "");

            DateTime dt = DateTime.Now;
            int dayOfWeek = Convert.ToInt32(dt.DayOfWeek.ToString("d"));
            DateTime startWeek = dt.AddDays(1 - ((dayOfWeek == 0) ? 7 : dayOfWeek));
            ViewData["DefaultTime"] = startWeek.AddDays(4).ToString("MM'/'dd'/'yyyy");

            return View();
        }
       
        public ActionResult SubmitEdit(FormCollection form)
        {
            try
            {
                string groupName = form["groupName"];
                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.FullDateTimePattern = "yyyy/MM/dd H:mm:ss";
                string dateTime = form["dateTime"];
                DateTime dt = string.IsNullOrEmpty(dateTime) ? Convert.ToDateTime("1753/01/01", dtFormat) : Convert.ToDateTime(dateTime, dtFormat);
                var query = (from a in db.UserAppraisals
                             where a.GroupName == groupName && a.DateTime == dt
                             select a).ToList();
                foreach (var member in query)
                {
                    member.WorkDone = string.IsNullOrEmpty(form[member.UserID + "WorkDone"]) ? 0 : Convert.ToDecimal(form[member.UserID + "WorkDone"]);
                    member.BugCreated = string.IsNullOrEmpty(form[member.UserID + "BugCreated"]) ? 0 : Convert.ToDecimal(form[member.UserID + "BugCreated"]);
                    member.WorkQuality = string.IsNullOrEmpty(form[member.UserID + "WorkQuality"]) ? 0 : Convert.ToDecimal(form[member.UserID + "WorkQuality"]);
                    member.DailyPerformance = string.IsNullOrEmpty(form[member.UserID + "DailyPerformance"]) ? 0 : Convert.ToDecimal(form[member.UserID + "DailyPerformance"]);
                    
                    if (member.WorkDone==0&&member.BugCreated==0&&member.WorkQuality==0&&member.DailyPerformance==0)
                    {
                        db.UserAppraisals.Remove(member);
                    }
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                Response.Write(dbEx);
            }

            return null;
        }

        public ActionResult MemberEdit(string groupName, string dateTime)
        {
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.FullDateTimePattern = "yyyy/MM/dd H:mm:ss";
            DateTime dt = string.IsNullOrEmpty(dateTime) ? Convert.ToDateTime("1753/01/01", dtFormat) : Convert.ToDateTime(dateTime, dtFormat);
            var query = (from a in db.UserAppraisals
                         where a.GroupName == groupName && a.DateTime == dt
                         orderby a.UserName
                         select a).ToList();
            var members = query.Select(c => new UserAppraisal { UserID = c.UserID, UserName = c.UserName, WorkDone = c.WorkDone, BugCreated = c.BugCreated, WorkQuality = c.WorkQuality, DailyPerformance = c.DailyPerformance }).ToList();
            return PartialView("MemberEdit", members);
        }

        public ActionResult MonthlyAppraisal()
        {
            List<SelectListItem> listItem = GetGroupName();
            ViewData["ListItem"] = new SelectList(listItem, "Value", "Text", "");

            //DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime startDate = DateTime.Now.AddDays(1 - DateTime.Now.Day);
            DateTime endDate = DateTime.Today;

            ViewData["startDate"] = startDate.ToString("MM'/'dd'/'yyyy");
            ViewData["endDate"] = endDate.ToString("MM'/'dd'/'yyyy");

            return View();
        }

        public ActionResult List(string groupName, string startDate, string endDate)
        {
            List<Monthly> list = new List<Monthly>();
            List<string> date = new List<string>();

            list = GroupRegion(groupName, startDate, endDate);
            date = DateRegion(groupName, startDate, endDate);

            ViewBag.date = date;
           
            return PartialView("ListPartial",list);
        }
        public List<Monthly> GroupRegion(string groupName, string startDate, string endDate)
        {
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.FullDateTimePattern = "yyyy/MM/dd";
            DateTime dateFrom = Convert.ToDateTime(startDate, dtFormat);
            DateTime dateTo = Convert.ToDateTime(endDate, dtFormat);
            var query = (from a in db.UserAppraisals
                         join b in db.Users on a.UserID equals b.UserID
                         where a.GroupName == groupName && a.DateTime >= dateFrom && a.DateTime <= dateTo&&b.IsDeparted==false
                         orderby a.DateTime
                         group a by a.UserName into m
                         select m).ToList();
            var datetime = (from b in db.UserAppraisals
                            where b.GroupName == groupName&&b.DateTime >= dateFrom && b.DateTime <= dateTo
                            select b.DateTime).Distinct();
                var monthly = new List<Monthly>();
                foreach (var app in query)
                {
                    //var isDeparted = (from a in db.Users where a.UserName == app.Key && a.IsDeparted == true select a.UserName).ToList();

                    //if (isDeparted.Count != 0)
                    //{
                    //    continue;
                    //}
                    var mothlyAppraisal = new List<MonthlyApprasial>();
                    foreach (var dt in datetime)
                    {
                        var l = app.FirstOrDefault(item => item.DateTime == dt);

                        if (l != null)
                        {
                            mothlyAppraisal.Add(new MonthlyApprasial { WorkDone = l.WorkDone, BugCreated = l.BugCreated, WorkQuality = l.WorkQuality, DailyPerformance = l.DailyPerformance });
                        }
                        else
                        {
                            mothlyAppraisal.Add(new MonthlyApprasial { WorkDone = 0, BugCreated = 0, WorkQuality = 0, DailyPerformance = 0 });
                        }
                    }
                    monthly.Add(new Monthly { UserName = app.Key, MonthlyAppraisal = mothlyAppraisal });
                }
            return monthly;
        }
        public List<string> DateRegion(string groupName, string startDate, string endDate)
        {
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.FullDateTimePattern = "yyyy/MM/dd";
            DateTime dateFrom = Convert.ToDateTime(startDate, dtFormat);
            DateTime dateTo = Convert.ToDateTime(endDate, dtFormat);
            var datetime = (from b in db.UserAppraisals
                            where b.GroupName==groupName&&b.DateTime >= dateFrom && b.DateTime <= dateTo
                            select b.DateTime).Distinct();
            var dateTime = new List<string>();
            foreach (var item in datetime)
            {
                dateTime.Add(item.ToString("yyyy/MM/dd"));
            }
            return dateTime;
        }

        public List<Group> Group(string startDate, string endDate)
        {
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.FullDateTimePattern = "yyyy/MM/dd";
            DateTime dateFrom = Convert.ToDateTime(startDate, dtFormat);
            DateTime dateTo = Convert.ToDateTime(endDate, dtFormat);
            var query = (from a in db.UserAppraisals
                         join b in db.Users on a.UserID equals b.UserID
                         where a.DateTime >= dateFrom && a.DateTime <= dateTo&&b.IsDeparted==false
                         orderby a.DateTime
                         group a by a.GroupName into m
                         select m).ToList();
            var datetime = (from b in db.UserAppraisals
                            where b.DateTime >= dateFrom && b.DateTime <= dateTo
                            select b.DateTime).Distinct();
            var group = new List<Group>();
            foreach (var project in query)
            {
                var appraisal = (from c in project
                                 orderby c.DateTime
                                 group c by c.UserName into n
                                 select n).ToList();
                var monthly = new List<Monthly>();
                foreach (var app in appraisal)
                {
                    var mothlyAppraisal = new List<MonthlyApprasial>();
                    foreach (var dt in datetime)
                    {
                        var l = app.FirstOrDefault(item => item.DateTime == dt);
                        if (l != null)
                        {
                            mothlyAppraisal.Add(new MonthlyApprasial { WorkDone = l.WorkDone, BugCreated = l.BugCreated, WorkQuality = l.WorkQuality, DailyPerformance = l.DailyPerformance });
                        }
                        else
                        {
                            mothlyAppraisal.Add(new MonthlyApprasial { WorkDone = 0, BugCreated = 0, WorkQuality = 0, DailyPerformance = 0 });
                        }
                    }
                    monthly.Add(new Monthly { UserName = app.Key, MonthlyAppraisal = mothlyAppraisal });
                }
                group.Add(new Group { GroupName = project.Key, Monthly = monthly });
            }

            return group;
        }
        public List<string> Date(string startDate, string endDate)
        {
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.FullDateTimePattern = "yyyy/MM/dd";
            DateTime dateFrom = Convert.ToDateTime(startDate, dtFormat);
            DateTime dateTo = Convert.ToDateTime(endDate, dtFormat);
            var datetime = (from b in db.UserAppraisals
                            where b.DateTime >= dateFrom && b.DateTime <= dateTo
                            select b.DateTime).Distinct();
            var dateTime = new List<string>();
            foreach (var item in datetime)
            {
                dateTime.Add(item.ToString("yyyy/MM/dd"));
            }
            return dateTime;
        }
        public static MemoryStream ExportToExcel(List<Group> list, List<string> date)
        {
            try
            {
                //文件流对像
                MemoryStream stream = new MemoryStream();

                //打开Excel对象
                HSSFWorkbook workbook = new HSSFWorkbook();

                Font aFont = workbook.CreateFont();
                aFont.FontName = "Calibri";
                aFont.FontHeightInPoints = 14;
                
                //aFont.Boldweight = short.MaxValue;

                CellStyle projectStyle = workbook.CreateCellStyle();
                //对齐方式
                projectStyle.Alignment = HorizontalAlignment.CENTER;
                projectStyle.VerticalAlignment = VerticalAlignment.BOTTOM;
                ////边框
                projectStyle.BorderTop = CellBorderType.MEDIUM;
                projectStyle.BorderRight = CellBorderType.MEDIUM;
                projectStyle.BorderBottom = CellBorderType.THIN;
                projectStyle.BorderLeft = CellBorderType.MEDIUM;
                //字体
                projectStyle.SetFont(aFont);

                //背景颜色
                HSSFPalette palette = workbook.GetCustomPalette();
                palette.SetColorAtIndex((short)9, (byte)(240), (byte)(240), (byte)(240));
                palette.SetColorAtIndex((short)10, (byte)(220), (byte)(220), (byte)(220));

                projectStyle.FillForegroundColor=(short)9;
                projectStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                //projectStyle.FillBackgroundColor = (short)9;

                CellStyle headStyle = workbook.CreateCellStyle();
                headStyle.Alignment = HorizontalAlignment.CENTER;
                headStyle.VerticalAlignment = VerticalAlignment.BOTTOM;
                headStyle.BorderTop = CellBorderType.MEDIUM;
                headStyle.BorderRight = CellBorderType.THIN;

                headStyle.SetFont(aFont);

                //headStyle.FillForegroundColor = HSSFColor.GREY_25_PERCENT.index;
                //headStyle.FillPattern = FillPatternType.SQUARES;
                //headStyle.FillBackgroundColor = HSSFColor.GREY_25_PERCENT.index;
                headStyle.FillForegroundColor = (short)9;
                headStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;

                CellStyle dateStyle = workbook.CreateCellStyle();
                dateStyle.Alignment = HorizontalAlignment.CENTER;
                dateStyle.VerticalAlignment = VerticalAlignment.BOTTOM;
                dateStyle.BorderTop = CellBorderType.MEDIUM;
                dateStyle.BorderRight = CellBorderType.MEDIUM;
                dateStyle.BorderLeft = CellBorderType.MEDIUM;

                Font bFont = workbook.CreateFont();
                bFont.FontName = "Calibri";
                bFont.FontHeightInPoints = 12;
                dateStyle.SetFont(bFont);

                CellStyle projectstyle = workbook.CreateCellStyle();
                //对齐方式
                projectstyle.Alignment = HorizontalAlignment.CENTER;
                projectstyle.VerticalAlignment = VerticalAlignment.CENTER;
                
                ////边框
                projectstyle.BorderRight = CellBorderType.MEDIUM;
                projectstyle.BorderBottom = CellBorderType.MEDIUM;
                //projectstyle.BorderLeft = CellBorderType.MEDIUM;

                projectstyle.SetFont(bFont);

                CellStyle nameStyle = workbook.CreateCellStyle();
                nameStyle.SetFont(bFont);
                //对齐方式
                nameStyle.Alignment = HorizontalAlignment.CENTER;
                nameStyle.VerticalAlignment = VerticalAlignment.CENTER;
                ////边框
                nameStyle.BorderTop = CellBorderType.THIN;
                nameStyle.BorderRight = CellBorderType.THIN;
                nameStyle.BorderBottom = CellBorderType.THIN;
                nameStyle.BorderLeft = CellBorderType.MEDIUM;

                CellStyle itemStyle = workbook.CreateCellStyle();
                nameStyle.SetFont(bFont);
                //对齐方式
                //itemStyle.Alignment = HorizontalAlignment.CENTER;
                itemStyle.VerticalAlignment = VerticalAlignment.BOTTOM;
                ////边框
                itemStyle.BorderTop = CellBorderType.THIN;
                itemStyle.BorderRight = CellBorderType.THIN;
                itemStyle.BorderBottom = CellBorderType.THIN;
                itemStyle.BorderLeft = CellBorderType.THIN;

                CellStyle datestyle = workbook.CreateCellStyle();
                nameStyle.SetFont(bFont);
                //对齐方式
                datestyle.Alignment = HorizontalAlignment.RIGHT;
                datestyle.VerticalAlignment = VerticalAlignment.BOTTOM;
                ////边框
                datestyle.BorderTop = CellBorderType.THIN;
                datestyle.BorderRight = CellBorderType.MEDIUM;
                datestyle.BorderBottom = CellBorderType.THIN;
                datestyle.BorderLeft = CellBorderType.MEDIUM;

                CellStyle emptyStyle = workbook.CreateCellStyle();
                //对齐方式
                emptyStyle.Alignment = HorizontalAlignment.CENTER;
                emptyStyle.VerticalAlignment = VerticalAlignment.BOTTOM;

                //背景颜色               
                emptyStyle.FillForegroundColor = (short)10;
                emptyStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                ////边框
                emptyStyle.BorderTop = CellBorderType.THIN;
                emptyStyle.BorderRight = CellBorderType.MEDIUM;
                emptyStyle.BorderBottom = CellBorderType.THIN;
                emptyStyle.BorderLeft = CellBorderType.MEDIUM;
               
                //emptyStyle.FillBackgroundColor = (short)8;

                CellStyle lastLineStyle = workbook.CreateCellStyle();
                lastLineStyle.BorderTop = CellBorderType.MEDIUM;

                //Excel的Sheet对象
                Sheet sheet = workbook.CreateSheet("sheet1");
                sheet.SetColumnWidth(0, 22 * 256);
                sheet.SetColumnWidth(1, 20 * 256);
                sheet.SetColumnWidth(2, 24 * 256);
                sheet.DefaultRowHeightInPoints = 20;
                sheet.CreateFreezePane(3, 0);
                //sheet.CreateFreezePane(3, 0, 0, 0);

                ////set date format
                //CellStyle cellStyleDate = workbook.CreateCellStyle();
                //DataFormat format = workbook.CreateDataFormat();
                //cellStyleDate.DataFormat = format.GetFormat("yyyy年m月d日");

                //将数据导入到excel表中
                Row row;
                Cell cell;
                CellRangeAddress range;
                int count = 1;
                foreach (var project in list)
                {
                    int column = 0;
                    row = sheet.CreateRow(count);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("Project");
                    cell.CellStyle = projectStyle;
                    cell = row.CreateCell(1);
                    cell.SetCellValue("Name");
                    cell.CellStyle = headStyle;
                    cell = row.CreateCell(2);
                    cell.SetCellValue("Check Item");
                    cell.CellStyle = headStyle;
                    foreach (var dateitem in date)
                    {
                        sheet.SetColumnWidth(column + 3, 12 * 256);
                        cell = row.CreateCell(column + 3);
                        cell.SetCellValue(dateitem);
                        cell.CellStyle = dateStyle;
                        column++;
                    }
                    count++;
                    int startCount = count;
                    foreach (var user in project.Monthly)
                    {
                        Row row1 = sheet.CreateRow(count);
                        Row row2 = sheet.CreateRow(count + 1);
                        Row row3 = sheet.CreateRow(count + 2);
                        Row row4 = sheet.CreateRow(count + 3);
                        cell = row1.CreateCell(0);
                        cell.SetCellValue(project.GroupName);
                        cell.CellStyle = projectstyle;
                        //cell = row2.CreateCell(0);
                        //cell.CellStyle = projectstyle;
                        //cell = row3.CreateCell(0);
                        //cell.CellStyle = projectstyle;
                        //cell = row4.CreateCell(0);
                        //cell.CellStyle = projectstyle;
                        cell = row1.CreateCell(1);
                        cell.SetCellValue(user.UserName);
                        cell.CellStyle = nameStyle;
                        cell = row2.CreateCell(1);
                        cell.CellStyle = nameStyle;
                        cell = row3.CreateCell(1);
                        cell.CellStyle = nameStyle;
                        cell = row4.CreateCell(1);
                        cell.CellStyle = nameStyle;
                        cell = row1.CreateCell(2);
                        cell.SetCellValue("Work Done");
                        cell.CellStyle = itemStyle;
                        cell = row2.CreateCell(2);
                        cell.SetCellValue("Bug Created");
                        cell.CellStyle = itemStyle;
                        cell = row3.CreateCell(2);
                        cell.SetCellValue("Work Quality");
                        cell.CellStyle = itemStyle;
                        cell = row4.CreateCell(2);
                        cell.SetCellValue("Daily Performance");
                        cell.CellStyle = itemStyle;
                        range = new CellRangeAddress(count, count + 3, 1, 1);
                        sheet.AddMergedRegion(range);
                        int num = 0;
                        foreach (var item in user.MonthlyAppraisal)
                        {

                            cell = row1.CreateCell(num + 3);
                            if (item.WorkDone != 0)
                            {
                                cell.SetCellValue((Double)item.WorkDone);
                                cell.CellStyle = datestyle;
                            }
                            else
                            {
                                cell.CellStyle = emptyStyle;
                            }
                            cell = row2.CreateCell(num + 3);
                            if (item.BugCreated != 0)
                            {
                                cell.SetCellValue((Double)item.BugCreated);
                                cell.CellStyle = datestyle;
                            }
                            else
                            {
                                cell.CellStyle = emptyStyle;
                            }
                            cell = row3.CreateCell(num + 3);
                            if (item.WorkQuality != 0)
                            {
                                cell.SetCellValue((Double)item.WorkQuality);
                                cell.CellStyle = datestyle;
                            }
                            else
                            {
                                cell.CellStyle = emptyStyle;
                            }
                            cell = row4.CreateCell(num + 3);
                            if (item.DailyPerformance != 0)
                            {
                                cell.SetCellValue((Double)item.DailyPerformance);
                                cell.CellStyle = datestyle;
                            }
                            else
                            {
                                cell.CellStyle = emptyStyle;
                            }
                            num++;
                        }
                        count += 4;
                        range = new CellRangeAddress(startCount, count - 1, 0, 0);
                        sheet.AddMergedRegion(range);
                    }
                    row = sheet.CreateRow(count);
                    int m = date.Count;
                    for (int i = 0; i < m + 3; i++)
                    {
                        cell = row.CreateCell(i);
                        cell.CellStyle = lastLineStyle;
                    }
                }

                //保存excel文档
                sheet.ForceFormulaRecalculation = true;

                workbook.Write(stream);
                workbook.Dispose();

                return stream;
            }
            catch
            {
                return new MemoryStream();
            }
        }

        public FileResult ExportAppraisal(string startDate, string endDate)
        {
            List<Group> list = Group(startDate, endDate);
            List<string> datelist = Date(startDate, endDate);
            MemoryStream stream = ExportToExcel(list, datelist);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", "spreadsheet1.xls");
        }

        public ActionResult Email(string groupName, string table)
        {
            string result = string.Empty;

            var query = (from a in db.UserAppraisals
                         join b in db.Users on a.UserID equals b.UserID
                         where a.GroupName == groupName&&b.IsDeparted==false
                         select b.EmailAddress).Distinct();
            foreach (var item in query)
            {
                if (SendMail(item, "Appraisal", Server.UrlDecode(table), true))
                {
                    result="Send Successfully!";
                }
                else
                {
                    result="Failure to send!";
                }
            }
            return Content(result);
        }

        public static bool SendMail(string to, string subject, string body, bool IsHtml)
        {

            //邮件发送类  
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("elliemaesh@gmail.com", "encompass1234");
            try
            {
                mail = new MailMessage("elliemaesh@gmail.com", to, subject, body);
                mail.IsBodyHtml = IsHtml;
                //mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtp.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                mail.Dispose();
            }
        }
 
    }
}

       
     
