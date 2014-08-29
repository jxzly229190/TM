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
using MvcPaging;

namespace TeamManager.Controllers
{
    public class TraineeBreakOffController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private const int DefaultPageSize = 12;

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int page = 1)
        {
            string userid = User.Identity.Name.Split(',')[0];
            int currentPageIndex = page < 0 ? 0 : page - 1;
            var traineebreakoff = db.TraineeBreakOffs.Where(o => o.UserID == userid).OrderByDescending(o => o.BreakOffFrom).ToList();
            List<TraineeBreakOffTable> offList = new List<TraineeBreakOffTable>();
            CaculateCost caculate = new CaculateCost();
            foreach (var item in traineebreakoff)
            {
                TraineeBreakOffTable tbf = new TraineeBreakOffTable();
                tbf.traineeBreakOff = item;
                tbf.time = caculate.Caculate(item.BreakOffFrom, item.BreakOffTo).ToString() + "H";
                offList.Add(tbf);
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

            DateTime start = Convert.ToDateTime(collection.Get("BreakOffFrom"));
            DateTime end = Convert.ToDateTime(collection.Get("BreakOffTo"));

            mail.TraineeBreakOffApplication(start, end, User.Identity.Name.Split(',')[0]);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            TraineeBreakOff traineeBreakOff = db.TraineeBreakOffs.Find(id);
            if (traineeBreakOff == null)
            {
                return HttpNotFound();
            }

            return View(traineeBreakOff);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="traineeBreakOff"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TraineeBreakOff traineeBreakOff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(traineeBreakOff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(traineeBreakOff);
        }

        /// <summary>
        /// delete view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            TraineeBreakOff traineeBreakOff = db.TraineeBreakOffs.Find(id);
            if (traineeBreakOff == null)
            {
                return HttpNotFound();
            }
            return View(traineeBreakOff);
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
            TraineeBreakOff traineeBreakOff = db.TraineeBreakOffs.Find(id);
            db.TraineeBreakOffs.Remove(traineeBreakOff);
            db.SaveChanges();
            return RedirectToAction("Index");
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