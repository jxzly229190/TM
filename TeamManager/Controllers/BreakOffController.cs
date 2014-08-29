using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TeamManager.Classes;
using TeamManager.DataProvider;
using MvcPaging;
using TeamManager.Models;


namespace TeamManager.Controllers
{
    public class BreakOffController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private const int DefaultPageSize = 12;

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>

        //[HandleError(ExceptionType = typeof(System.Data.DataException), View = "DatabaseError")]
        public ActionResult Index(int page = 1)
        {
            int currentPageIndex = page < 0 ? 0 : page - 1;
            string userid = User.Identity.Name.Split(',')[0];
            var breakoff = db.BreakOffs.Where(o => o.UserID == userid).OrderByDescending(o => o.BreakOffFrom).ToList();
            var userLeave = db.UserLeaves.Where(o => o.UserID == userid).FirstOrDefault();

            if (userLeave == null)
            {
                return Content("<script >alert('You have not the Annual Days or Over Time!');window.history.back(-1);</script >", "text/html");
            }

            ViewBag.AnnualDays = userLeave.AnnualDays;
            ViewBag.AnnualDaysRest = userLeave.AnnualDaysRest;
            ViewBag.OverTime = userLeave.OverTime;
            ViewBag.OverTimeRest = userLeave.OverTimeRest;

            ViewData["List"] = GetList();

            List<BreakOffTable> offList = new List<BreakOffTable>();
            CaculateCost caculate = new CaculateCost();
            foreach (var item in breakoff)
            {
                BreakOffTable bf = new BreakOffTable();
                bf.breakoff = item;
                bf.time = caculate.Caculate(item.BreakOffFrom, item.BreakOffTo).ToString() + "H";
                offList.Add(bf);
            }
            //ViewData["Model"] = offList;
            return View(offList.ToPagedList(currentPageIndex, DefaultPageSize, offList.Count));
      }       

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            MailEntity mail = new MailEntity();
            CaculateCost caculate = new CaculateCost();
            
            DateTime start = Convert.ToDateTime(collection.Get("BreakOffFrom"));
            DateTime end = Convert.ToDateTime(collection.Get("BreakOffTo"));
            string cutFrom= collection.Get("CutFrom");

            string userid = User.Identity.Name.Split(',')[0];
            var userLeave = db.UserLeaves.Where(o => o.UserID == userid).FirstOrDefault();

            int cutTime = caculate.Caculate(start, end);
            if (cutFrom == "Annual Leave")
            {
                if (cutTime > userLeave.AnnualDaysRest)
                {
                    return Content("<script >alert('You do not have enough time for Annual Leave！');window.history.back(-1);</script >", "text/html");
                }
                if (cutTime % 4 != 0)
                {
                    return Content("<script >alert('Leave time must be in multiples of four！');window.history.back(-1);</script >", "text/html");
                }
            }
            else
            {
                if (cutTime > userLeave.OverTimeRest)
                {
                    return Content("<script >alert('You do not have enough time for Change Off！');window.history.back(-1);</script >", "text/html");
                }
            }

            mail.BreakOffApplication(start, end, cutFrom, userid);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            BreakOff breakoff = db.BreakOffs.Find(id);
            if (breakoff == null)
            {
                return HttpNotFound();
            }
            string userid = User.Identity.Name.Split(',')[0];
            var userLeave = db.UserLeaves.Where(o => o.UserID == userid).FirstOrDefault();
            ViewBag.AnnualDaysRest = userLeave.AnnualDaysRest;
            ViewBag.OverTimeRest = userLeave.OverTimeRest;
            ViewData["List"] = GetList();
           
            return View(breakoff);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="breakoff"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BreakOff breakoff)
        {
            CaculateCost caculate = new CaculateCost();
            string userid = User.Identity.Name.Split(',')[0];
            var userLeave = db.UserLeaves.Where(o => o.UserID == userid).FirstOrDefault();
            ViewBag.AnnualDaysRest = userLeave.AnnualDaysRest;
            ViewBag.OverTimeRest = userLeave.OverTimeRest;

            int cutTime = caculate.Caculate(breakoff.BreakOffFrom, breakoff.BreakOffTo);
            if (breakoff.CutFrom == "Annual Leave")
            {
                if (cutTime > ViewBag.AnnualDaysRest)
                {
                    ModelState.AddModelError("BreakOffTo", "You do not have enough time for Annual Leave！");
                }
                if (cutTime % 4 != 0)
                {
                    ModelState.AddModelError("BreakOffTo", "Leave time must be in multiples of four！");
                }
            }
            else
            {
                if (cutTime > ViewBag.OverTimeRest)
                {
                    ModelState.AddModelError("BreakOffTo", "You do not have enough time for Change Off！");
                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(breakoff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewData["List"] = GetList();
            return View(breakoff);
        }

        /// <summary>
        /// delete view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            BreakOff breakoff = db.BreakOffs.Find(id);
            if (breakoff == null)
            {
                return HttpNotFound();
            }
            ViewData["List"] = GetList();
            return View(breakoff);
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            BreakOff breakoff = db.BreakOffs.Find(id);
            db.BreakOffs.Remove(breakoff);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// get list of cutfrom
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetList()
        {
            List<SelectListItem> last = new List<SelectListItem>();
            if (ViewBag.AnnualDaysRest < 4 && ViewBag.OverTimeRest <= 0)
            {
                last = null;
            }
            else if (ViewBag.AnnualDaysRest < 4)
            {
                last.Add(new SelectListItem { Text = "Change Off", Value = "Change Off" });
            }
            else if (ViewBag.OverTimeRest <= 0)
            {
                last.Add(new SelectListItem { Text = "Annual Leave", Value = "Annual Leave" });
            }
            else
            {
                last.Add(new SelectListItem { Text = "Annual Leave", Value = "Annual Leave" });
                last.Add(new SelectListItem { Text = "Change Off", Value = "Change Off" });
            }
            return last;
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