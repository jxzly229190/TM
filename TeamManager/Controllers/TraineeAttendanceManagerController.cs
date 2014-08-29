using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamManager.Classes;
using TeamManager.DataProvider;
using TeamManager.Models;

namespace TeamManager.Controllers
{
    public class TraineeAttendanceManagerController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();

        //
        // GET: /TraineeAttendanceManager/

        public ActionResult Index()
        {
            DateTime from = DateTime.Now.AddDays(1 - Convert.ToInt32(DateTime.Now.DayOfWeek.ToString("d")));
            DateTime to = DateTime.Now;

            ViewData["dropdown"] = GetDropDownList();

            ViewBag.fromTime = from.ToString("MM'/'dd'/'yyyy");
            ViewBag.toTime = to.ToString("MM'/'dd'/'yyyy");
            ViewBag.flag = false;

            return View(traineeTable(from, to, ""));
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult Search(DateTime from, DateTime to, string who)
        {
            return PartialView("TraineeTable", traineeTable(from, to, who));
        }

        /// <summary>
        /// Get BreakOffTable data
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public static IEnumerable<TraineeTable> traineeTable(DateTime from, DateTime to, string who)
        {
            List<TraineeAttendance> traineeAttendance;
            List<TraineeTable> TraineeManager = new List<TraineeTable>();
            CaculateCost caculate = new CaculateCost();

            var count = 0;

            from = new DateTime(from.Year, from.Month, from.Day);
            to = to.AddDays(1); 
            using (var db = new TeamManage_Entities())
            {
                if (who == string.Empty)
                {
                    traineeAttendance = db.TraineeAttendances.Where(o => o.AttendanceFrom >= from && o.AttendanceTo <= to)
                                                        .OrderByDescending(o => o.AttendanceFrom).ToList();
                }
                else
                {
                    traineeAttendance = db.TraineeAttendances.Where(o => o.AttendanceFrom >= from && o.AttendanceTo <= to)
                                                        .Where(o => o.UserID == who)
                                                        .OrderByDescending(o => o.AttendanceFrom).ToList();
                }

                foreach (var item in traineeAttendance)
                {
                    count++;
                    var user = db.Users.Where(o => o.UserID == item.UserID).FirstOrDefault();
                    TraineeTable tt = new TraineeTable();
                    tt.id = count;
                    tt.trainee = item;
                    tt.user = user;
                    tt.time = caculate.Caculate(item.AttendanceFrom, item.AttendanceTo).ToString() + "H";

                    TraineeManager.Add(tt);
                }
            }
            return TraineeManager;
        }

        /// <summary>
        /// get statuslist
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "All", Value = "" });
            var user = db.Users.Where(o => o.IsDeparted == false).Where(o => o.IsTrainee == true).ToList();
            foreach (var item in user)
            {
                list.Add(new SelectListItem { Text = item.UserName.ToString(), Value =item.UserID.ToString() });
            }

            return list;
        }

        /// <summary>
        /// sort Method
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="sortDirection"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public ActionResult SortTable(string sort, string sortDirection, DateTime from, DateTime to, string who)
        {

            if (string.IsNullOrEmpty(sort))
            {
                sort = "AttendanceFrom";
            }

            if (string.IsNullOrEmpty(sortDirection))
            {
                sortDirection = "ascending";
            }
            ViewBag.sort = sort;
            ViewBag.sortDirection = (sortDirection == "ascending") ? "descending" : "ascending";
            IEnumerable<TraineeTable> TraineeManager = new List<TraineeTable>();
            TraineeManager = traineeTable(from, to, who);
            var query = from tr in TraineeManager
                        orderby Utils.GetPropertyValue(tr, tr.trainee, sort) descending
                        select tr;
            if (sortDirection == "ascending")
            {
                query = from tr in TraineeManager
                        orderby Utils.GetPropertyValue(tr, tr.trainee, sort) ascending
                        select tr;
            }
            if (sort == "UserName")
            {
                query = from tr in TraineeManager
                        orderby Utils.GetPropertyValue(tr, tr.user, sort) descending
                        select tr;
                if (sortDirection == "ascending")
                {
                    query = from tr in TraineeManager
                            orderby Utils.GetPropertyValue(tr, tr.user, sort) ascending
                            select tr;
                }
            }
            return PartialView("TraineeTable", query);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}