using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamManager.DataProvider;
using TeamManager.Models;
using MvcPaging;
using TeamManager.Classes;

namespace TeamManager.Controllers
{
    public class TimeManagerController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private const int DefaultPageSize = 10;

        //
        // GET: /TimeManager/

        public ActionResult Index(int page = 1)
        {
            ViewData["List"] = GetList();
            int currentPageIndex = page < 0 ? 0 : page - 1;
            //ViewData["Model"] = timeManager();
            return View(timeManager().ToPagedList(currentPageIndex, DefaultPageSize, timeManager().Count()));
        }

        [HttpPost]
        public ActionResult Create(UserLeave userleave)
        {
            userleave.InitDateTime = DateTime.Now;
            userleave.AnnualDaysRest = userleave.AnnualDays;
            userleave.OverTimeRest = userleave.OverTime;
            userleave.AnnualDaysERP = 0;
            if (ModelState.IsValid)
            {
                db.UserLeaves.Add(userleave);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userleave);
        }

        //
        // GET: /TimeManager/Edit/5

        public ActionResult Edit(int id)
        {
            UserLeave userleave = db.UserLeaves.Find(id);
            if (userleave == null)
            {
                return HttpNotFound();
            }
            TimeTable time = new TimeTable();
            time.userLeave = userleave;
            time.user = db.Users.Find(userleave.UserID);
            return View(time);
        }

        //
        // POST: /TimeManager/Edit/5

        [HttpPost]
        public ActionResult Edit(TimeTable time)
        {
            UserLeave userLeave = time.userLeave;
            userLeave.UpdateTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(userLeave).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(time);
        }

        public IEnumerable<TimeTable> timeManager()
        {
            List<TimeTable> timeTable = new List<TimeTable>();
            //var userLeave = db.UserLeaves.ToList();
            var userLeave = db.UserLeaves.Where(o=>(o.AnnualDays!=0)||(o.OverTime!=0)).ToList();
            foreach (var item in userLeave)
            {
                var user = db.Users.Where(o => o.UserID == item.UserID).FirstOrDefault();
                if (user.IsDeparted == false)
                {
                    TimeTable time = new TimeTable();
                    time.userLeave = item;
                    time.user = user;
                    timeTable.Add(time);
                }
            }
            return timeTable.OrderBy(o=>o.user.UserName);
        }

        public List<SelectListItem> GetList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            var user = db.Users.Where(o => o.IsDeparted == false).ToList();
            foreach (var item in user)
            {
                var userLeave = db.UserLeaves.Where(o => o.UserID == item.UserID);
                if (userLeave.Count() == 0)
                {
                    list.Add(new SelectListItem { Text = item.UserName.ToString(), Value = item.UserID.ToString() });
                }
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
        /// <param name="status"></param>
        /// <returns></returns>
        //public ActionResult SortTable(string sortDirection,int page=1)
        //{

        //    int currentPageIndex = page < 0 ? 0 : page - 1;
        //    if (string.IsNullOrEmpty(sortDirection))
        //    {
        //        sortDirection = "ascending";
        //    }
        //    ViewBag.sortDirection = (sortDirection == "ascending") ? "descending" : "ascending";
        //    IEnumerable<TimeTable> timeTable = new List<TimeTable>();
        //    timeTable = timeManager();
        //    var query = from tr in timeTable
        //                orderby Utils.GetPropertyValue(tr, tr.user, "UserName") descending
        //                select tr;
        //    if (sortDirection == "ascending")
        //    {
        //        query = from tr in timeTable
        //                orderby Utils.GetPropertyValue(tr, tr.user, "UserName") ascending
        //                select tr;
        //    }
        //    ViewData["List"] = GetList();
        //    return View("Index", query.ToPagedList(currentPageIndex, DefaultPageSize, query.Count()));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}