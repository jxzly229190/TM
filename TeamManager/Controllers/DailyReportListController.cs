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
using System.Globalization;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Util;

namespace TeamManager.Controllers
{
    public class DailyReportListController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private ReportFunction RF = new ReportFunction();

        /// <summary>
        /// get the data will be displayed in the table
        /// </summary>
        /// <param name="flag">a mark(0 means the result will be displayed in html;1 means the result will be displayed in excel)</param>
        /// <returns>a list will be passed to the view</returns>
        private List<Tuple<string, List<Tuple<string, string>>>> TableData(int flag)
        {
            DateTime dateFrom = DateTime.Now.Date;
            DateTime dateTo = DateTime.Now.Date.AddDays(1);

            var projects = db.UserRoles.Select(a => a.GroupName).Distinct().ToList();
            projects.Add("Other");


            var dailyReports = (from a in db.DailyReports
                                from u in db.Users
                                where a.UserID == u.UserID && a.AddDate >= dateFrom && a.AddDate <= dateTo
                                orderby u.UserName
                                select new { Name = u.UserName, Report = a.Report }).ToList();
            List<Tuple<string, List<Tuple<string, string>>>> projectTuples = new List<Tuple<string, List<Tuple<string, string>>>>();
            foreach (var project in projects)
            {
                string projectName = project;
                List<Tuple<string, string>> tuples = new List<Tuple<string, string>>();
                foreach (var item in dailyReports)
                {
                    string name = item.Name;

                    ReportItems reportItems = new ReportItems();
                    reportItems.FromXML(item.Report);

                    StringBuilder tempStr = new StringBuilder();
                    RF.ReportToGroup(projectName, reportItems, tempStr, flag);
                    if (tempStr.ToString() != "")
                    {
                        var tuple = new Tuple<string, string>(name, tempStr.ToString());
                        tuples.Add(tuple);
                    }
                }
                projectTuples.Add(new Tuple<string, List<Tuple<string, string>>>(projectName, tuples));
            }
            return projectTuples;
        }


        public ActionResult Index()
        {
            List<Tuple<string, List<Tuple<string, string>>>> projectTuples = TableData(0);

            return View(projectTuples);
        }


        public FileResult Export()
        {
            List<Tuple<string, List<Tuple<string, string>>>> list = TableData(1);
            MemoryStream stream = ExportDatasetToExcel(list);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", "spreadsheet1.xls");
        }

        public static MemoryStream ExportDatasetToExcel(List<Tuple<string, List<Tuple<string, string>>>> list)
        {
            try
            {
                //File stream object
                MemoryStream stream = new MemoryStream();

                //Open Excel object
                HSSFWorkbook workbook = new HSSFWorkbook();
                //Head Style
                CellStyle headstyle = workbook.CreateCellStyle();
                //Alignment
                headstyle.Alignment = HorizontalAlignment.CENTER;
                headstyle.VerticalAlignment = VerticalAlignment.CENTER;

                //Font
                Font headfont = workbook.CreateFont();
                headfont.FontHeightInPoints = 12;
                headfont.Boldweight = short.MaxValue;
                headstyle.SetFont(headfont);
                //Background color
                headstyle.FillForegroundColor = HSSFColor.GREY_25_PERCENT.index;
                headstyle.FillPattern = FillPatternType.SQUARES;
                headstyle.FillBackgroundColor = HSSFColor.GREY_25_PERCENT.index;

                //Border
                headstyle.BorderTop = CellBorderType.THIN;
                headstyle.BorderRight = CellBorderType.THIN;
                headstyle.BorderBottom = CellBorderType.THIN;
                headstyle.BorderLeft = CellBorderType.THIN;

                //Body Style
                CellStyle bodystyle = workbook.CreateCellStyle();
                //Border
                bodystyle.BorderTop = CellBorderType.THIN;
                bodystyle.BorderRight = CellBorderType.THIN;
                bodystyle.BorderBottom = CellBorderType.THIN;
                bodystyle.BorderLeft = CellBorderType.THIN;
                //Line Feed
                bodystyle.WrapText = true;
                //Sheet's object of Excel
                Sheet sheet = workbook.CreateSheet("sheet1");
                sheet.SetColumnWidth(0, 5000);
                sheet.SetColumnWidth(1, 100000);
                ////set date format
                //CellStyle cellStyleDate = workbook.CreateCellStyle();
                //DataFormat format = workbook.CreateDataFormat();
                //cellStyleDate.DataFormat = format.GetFormat("yyyy年m月d日");

                //Export to Excel
                Row row;
                Cell cell;
                //cell.SetCellType();
                int count = 0;
                foreach (var l in list)
                {
                    if (l.Item2.Count != 0)
                    {
                        row = sheet.CreateRow(count);
                        row.HeightInPoints = 20;
                        cell = row.CreateCell(0);
                        cell.CellStyle = headstyle;
                        cell.SetCellValue(l.Item1);
                        cell = row.CreateCell(1);
                        cell.CellStyle = headstyle;
                        CellRangeAddress range = new CellRangeAddress(count, count, 0, 1);
                        sheet.AddMergedRegion(range);
                        count++;
                        foreach (var item in l.Item2)
                        {
                            row = sheet.CreateRow(count);
                            cell = row.CreateCell(0);
                            cell.SetCellValue(item.Item1);
                            cell.CellStyle = bodystyle;
                            cell = row.CreateCell(1);
                            cell.SetCellValue(item.Item2);
                            cell.CellStyle = bodystyle;
                            count++;
                        }
                    }
                }


                //Save excel
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


        private List<Tuple<string, string, string>> Datas(string projectName)
        {
            DateTime dateFrom = DateTime.Now.Date;
            DateTime dateTo = DateTime.Now.Date.AddDays(1);

            //var projects = db.UserRoles.Select(a => a.GroupName).Distinct().ToList();
            //projects.Add("Other");


            var dailyReports = (from a in db.DailyReports
                                from u in db.Users
                                from r in db.UserRoles
                                where a.UserID == u.UserID && r.UserID == u.UserID && r.GroupName == projectName && a.AddDate >= dateFrom && a.AddDate <= dateTo
                                orderby u.UserName
                                select new { Name = u.UserName, Report = a.Report }).ToList();
            List<Tuple<string, string, string>> workTuples = new List<Tuple<string, string, string>>();

            foreach (var item in dailyReports)
            {
                string name = item.Name;

                ReportItems reportItems = new ReportItems();
                reportItems.FromXML(item.Report);

                string worked = RF.GetWorkedReport(reportItems);
                string working = RF.GetWorkingReport(reportItems);

                if (worked != "" || working != "")
                {
                    var tuple = new Tuple<string, string, string>(name, worked, working);
                    workTuples.Add(tuple);
                }
            }
            return workTuples;
        }

        public FileResult WorkExport()
        {
            List<Tuple<string, string, string>> list = Datas("Web Center");
            MemoryStream stream = ExportDataToExcel(list);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", "dailyreorts.xls");
        }

        public static MemoryStream ExportDataToExcel(List<Tuple<string, string, string>> list)
        {
            try
            {
                //File stream object
                MemoryStream stream = new MemoryStream();

                //Open Excel object
                HSSFWorkbook workbook = new HSSFWorkbook();

                //custom color
                HSSFPalette palette = workbook.GetCustomPalette();
                palette.SetColorAtIndex((short)15, (byte)(184), (byte)(134), (byte)(11));
                palette.SetColorAtIndex((short)16, (byte)(255), (byte)(240), (byte)(215));

                //Title Style
                CellStyle titlestyle = workbook.CreateCellStyle();
                //Alignment
                titlestyle.Alignment = HorizontalAlignment.CENTER;
                titlestyle.VerticalAlignment = VerticalAlignment.CENTER;
                //Font
                Font titlefont = workbook.CreateFont();
                titlefont.FontHeightInPoints = 16;
                titlefont.FontName = "Calibri";
                titlefont.Color = HSSFColor.WHITE.index;
                titlefont.Boldweight = (short)FontBoldWeight.BOLD;
                titlestyle.SetFont(titlefont);
                //Background color
                titlestyle.FillForegroundColor = (short)15;
                titlestyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                //Border
                titlestyle.BorderTop = CellBorderType.THIN;
                titlestyle.BorderRight = CellBorderType.THIN;
                titlestyle.BorderBottom = CellBorderType.THIN;
                titlestyle.BorderLeft = CellBorderType.THIN;


                //Head Style
                CellStyle headstyle = workbook.CreateCellStyle();
                //Alignment
                headstyle.Alignment = HorizontalAlignment.CENTER;
                headstyle.VerticalAlignment = VerticalAlignment.CENTER;
                //Font
                Font headfont = workbook.CreateFont();
                headfont.FontHeightInPoints = 14;
                headfont.FontName = "Calibri";
                headfont.Boldweight = (short)FontBoldWeight.BOLD;
                headstyle.SetFont(headfont);
                //Background color
                headstyle.FillForegroundColor = (short)16;
                headstyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                //Border
                headstyle.BorderTop = CellBorderType.THIN;
                headstyle.BorderRight = CellBorderType.THIN;
                headstyle.BorderBottom = CellBorderType.THIN;
                headstyle.BorderLeft = CellBorderType.THIN;


                //Name Style
                CellStyle namestyle = workbook.CreateCellStyle();
                //Alignment
                namestyle.Alignment = HorizontalAlignment.CENTER;
                namestyle.VerticalAlignment = VerticalAlignment.CENTER;
                //Font
                Font namefont = workbook.CreateFont();
                namefont.FontHeightInPoints = 11;
                namefont.FontName = "Calibri";
                namefont.Boldweight = (short)FontBoldWeight.BOLD;
                namestyle.SetFont(namefont);
                //Border
                namestyle.BorderTop = CellBorderType.THIN;
                namestyle.BorderRight = CellBorderType.THIN;
                namestyle.BorderBottom = CellBorderType.THIN;
                namestyle.BorderLeft = CellBorderType.THIN;

                //Body Style
                CellStyle bodystyle = workbook.CreateCellStyle();
                //Alignment
                bodystyle.VerticalAlignment = VerticalAlignment.CENTER;
                //Font
                Font bodyfont = workbook.CreateFont();
                bodyfont.FontHeightInPoints = 12;
                bodyfont.FontName = "Times New Roman";
                bodystyle.SetFont(bodyfont);
                //Border
                bodystyle.BorderTop = CellBorderType.THIN;
                bodystyle.BorderRight = CellBorderType.THIN;
                bodystyle.BorderBottom = CellBorderType.THIN;
                bodystyle.BorderLeft = CellBorderType.THIN;
                //Line Feed
                bodystyle.WrapText = true;
                //Sheet's object of Excel
                Sheet sheet = workbook.CreateSheet("sheet1");
                sheet.SetColumnWidth(0, (short)(35.7 * 160));
                sheet.SetColumnWidth(1, (short)(35.7 * 400));
                sheet.SetColumnWidth(2, (short)(35.7 * 600));


                //Export to Excel
                Row row;
                Cell cell;
                //cell.SetCellType();
                int count = 2;

                row = sheet.CreateRow(0);
                row.HeightInPoints = 20;
                cell = row.CreateCell(0);
                cell.CellStyle = titlestyle;
                cell.SetCellValue("Staff Report");
                cell = row.CreateCell(1);
                cell.CellStyle = titlestyle;
                cell = row.CreateCell(2);
                cell.CellStyle = titlestyle;
                CellRangeAddress range = new CellRangeAddress(0, 0, 0, 2);
                sheet.AddMergedRegion(range);

                row = sheet.CreateRow(1);
                row.HeightInPoints = 20;
                cell = row.CreateCell(0);
                cell.CellStyle = headstyle;
                cell = row.CreateCell(1);
                cell.CellStyle = headstyle;
                cell.SetCellValue(" What you worked on today?");
                cell = row.CreateCell(2);
                cell.CellStyle = headstyle;
                cell.SetCellValue(" What you will be working on tomorrow?");

                foreach (var item in list)
                {
                    row = sheet.CreateRow(count);

                    cell = row.CreateCell(0);
                    cell.SetCellValue(item.Item1);
                    cell.CellStyle = namestyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(item.Item2);
                    cell.CellStyle = bodystyle;
                    cell = row.CreateCell(2);
                    cell.SetCellValue(item.Item3);
                    cell.CellStyle = bodystyle;

                    count++;
                }

                //Save excel
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}