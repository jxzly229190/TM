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
using MvcPaging;

namespace TeamManager.Controllers
{
    public class OverTimeController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private const int DefaultPageSize = 12;

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int page = 1)
        {
            ViewData["List"] = GetList();
            //ViewData["Model"] = db.OverTimes.Where(o => o.UserID == User.Identity.Name).OrderByDescending(o => o.OnDate).ToList();
            int currentPageIndex = page < 0 ? 0 : page - 1;
            string userid = User.Identity.Name.Split(',')[0];
            var overtime = db.OverTimes.Where(o => o.UserID == userid).OrderByDescending(o => o.OnDate).ToList();
            var userLeave = db.UserLeaves.Where(o => o.UserID == userid).FirstOrDefault();
            if (userLeave == null)
            {
                return Content("<script >alert('You have not the right of Over Time!');window.history.back(-1);</script >", "text/html");
            }
            ViewBag.OverTime = userLeave.OverTime;
            ViewBag.OverTimeRest = userLeave.OverTimeRest;
            return View(overtime.ToPagedList(currentPageIndex, DefaultPageSize, overtime.Count));
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            MailEntity mail = new MailEntity();
            DateTime date = Convert.ToDateTime(collection.Get("OnDate"));
            int hours = Convert.ToInt32(collection.Get("Hours"));

            mail.OverTimeApplication(date, hours, User.Identity.Name.Split(',')[0]);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            OverTime overtime = db.OverTimes.Find(id);
            if (overtime == null)
            {
                return HttpNotFound();
            }
            ViewData["List"] = GetList();
           
            return View(overtime);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="overtime"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OverTime overtime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(overtime).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(overtime);
        }

        /// <summary>
        /// delete view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            OverTime overtime = db.OverTimes.Find(id);
            if (overtime == null)
            {
                return HttpNotFound();
            }
            ViewData["List"] = GetList();

            return View(overtime);
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
            OverTime overtime = db.OverTimes.Find(id);
            db.OverTimes.Remove(overtime);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// get list
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetList()
        {
            List<SelectListItem> last = new List<SelectListItem>();
            for (int i = 1; i < 9; i++)
            {
                last.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
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