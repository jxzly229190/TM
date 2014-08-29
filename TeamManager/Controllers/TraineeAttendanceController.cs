using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamManager.Classes;
using TeamManager.DataProvider;

namespace TeamManager.Controllers
{
    public class TraineeAttendanceController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();

        //
        // GET: /TraineeAttendance/

        public ActionResult Index()
        {
            string userid = User.Identity.Name.Split(',')[0];
            DateTime from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime to = from.AddMonths(1);
            var traineeAttendance = db.TraineeAttendances.OrderByDescending(o => o.AttendanceFrom)
               .Where(o => o.UserID == userid).Where(o => (o.AttendanceFrom >= from) && (o.AttendanceFrom <= to)).ToList();
            return View(traineeAttendance);
        }

        //
        // POST: /TraineeAttendance/Create

        [HttpPost]
        public ActionResult Create(TraineeAttendance traineeattendance)
        {
            if (ModelState.IsValid)
            {
                traineeattendance.AttendanceGuid = Guid.NewGuid();
                traineeattendance.UserID = User.Identity.Name.Split(',')[0];
                traineeattendance.CreateTime = DateTime.Now;
                db.TraineeAttendances.Add(traineeattendance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(traineeattendance);
        }

        //
        // GET: /TraineeAttendance/Edit/5

        public ActionResult Edit(Guid id)
        {
            TraineeAttendance traineeattendance = db.TraineeAttendances.Find(id);
            if (traineeattendance == null)
            {
                return HttpNotFound();
            }
            return View(traineeattendance);
        }

        //
        // POST: /TraineeAttendance/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TraineeAttendance traineeattendance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(traineeattendance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(traineeattendance);
        }

        //
        // GET: /TraineeAttendance/Delete/5

        public ActionResult Delete(Guid id)
        {
            TraineeAttendance traineeattendance = db.TraineeAttendances.Find(id);
            if (traineeattendance == null)
            {
                return HttpNotFound();
            }
            return View(traineeattendance);
        }

        //
        // POST: /TraineeAttendance/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            TraineeAttendance traineeattendance = db.TraineeAttendances.Find(id);
            db.TraineeAttendances.Remove(traineeattendance);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}