using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TeamManager.DataProvider;

namespace TeamManager.Classes
{
    public partial class MailEntity
    {
        private TeamManage_Entities db = new TeamManage_Entities();

        /// <summary>
        /// BreakOffApplication
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="cutFrom"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BreakOffApplication(DateTime start, DateTime end, string cutFrom, string userID)
        {
            string cc = null;
            BreakOff breakOff = new BreakOff();

            breakOff.BreakOffGuid = Guid.NewGuid();
            breakOff.UserID = userID;
            breakOff.BreakOffFrom = start;
            breakOff.BreakOffTo = end;
            breakOff.CutFrom = cutFrom;
            breakOff.Status = "Ask";

            db.BreakOffs.Add(breakOff);

            var user = db.Users.FirstOrDefault(o => o.UserID == userID);
            var userRole = db.UserRoles.Where(o => o.UserID == userID).ToList();
            StringBuilder email = new StringBuilder();
            CaculateCost caculate = new CaculateCost();

            string time = caculate.Caculate(start, end).ToString() + "H";
            int timeCost = Convert.ToInt32(time.Substring(0, time.Length - 1));
            int days = timeCost / 8;
            timeCost = timeCost - days * 8;

            time = string.Empty;
            if (days > 0)
                time += " " + days.ToString() + (days > 1 ? " Days" : " Day");
            if (timeCost > 0)
                time += " " + timeCost.ToString() + (timeCost > 1 ? " Hours" : " Hour");

            if (end.Subtract(start).Days == 0)
                time = time + " off(" + cutFrom + ") on " + start.ToShortDateString() + ".";
            else
                time = time + " off(" + cutFrom + ") from " + start.ToShortDateString() + " to " + end.ToShortDateString() + ".";
            //email.Append("Hi Yemol, <br/><br/>");
            email.Append("I am going to take" + time);
            email.AppendFormat(" Could you please approve it?");//<br/><br/>Thanks,<br/><br/><br/>{0}", user.UserName);

            for (int i = 0; i < userRole.Count; i++)
            {
                string leaderID = null;
                string groupName = userRole[i].GroupName;
                var userLeader = db.UserRoles.Where(o => o.GroupName == groupName)
                    .Where(o => (o.RoleName == "Team Leader")).ToList();

                for (int j = 0; j < userLeader.Count; j++)
                {
                    if (leaderID == null || leaderID == String.Empty || userLeader[j].UserID == null)
                    {
                        leaderID += userLeader[j].UserID;
                    }
                    else
                    {
                        leaderID += ("," + userLeader[j].UserID);
                    }
                }

                if (cc == null || cc == String.Empty || leaderID == null)
                {
                    cc += leaderID;
                }
                else
                {
                    cc += ("," + leaderID);
                }
               
            }
            if (cc == null || cc == String.Empty)
            {
                cc += userID;
            }
            else
            {
                cc += ("," + userID);
            }

            t_AlertUser alert = new t_AlertUser();

            alert.ABID = 1;
            alert.UserID = "yemol";
            alert.CcUser = cc;
            alert.AUTitle = user.UserName + " BreakOff Application";
            alert.AUContent = email.ToString();
            alert.AUType = 0;
            alert.AUStatus = 1;
            alert.AUInterval = 0;
            alert.AUUnitNum = 0;
            alert.AUTime = DateTime.Now;
            alert.CreateBy = userID;
            alert.CreateDate = DateTime.Now;

            db.t_AlertUser.Add(alert);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// BreakOff Approved
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BreakOffApprove(Guid id, string userID)
        {
            //update BreakOff
            BreakOff breakoff = db.BreakOffs.Find(id);
            CaculateCost caculate = new CaculateCost();

            if (breakoff == null)
            {
                return false;
            }
            breakoff.Status = "Approve";

            //Add UserLeaveLog
            UserLeaveCutLog log = new UserLeaveCutLog();
            log.BreakOFFID = Guid.NewGuid();
            log.CutDateTime = DateTime.Now;
            log.UserID = breakoff.UserID;
            log.CutHour = caculate.Caculate(breakoff.BreakOffFrom, breakoff.BreakOffTo);
            StringBuilder content = new StringBuilder();
            content.Append("I have approved your BreakOff.");

            //update UserLeave
            var userLeave = db.UserLeaves.Where(o => o.UserID == breakoff.UserID).FirstOrDefault();
            if (breakoff.CutFrom == "Annual Leave")
            {
                content.Append("Your Annual Leave time has decreased "+ log.CutHour.ToString()+" hours,");
                content.Append(" it used to be " +userLeave.AnnualDaysRest.ToString()+" hours,");
                userLeave.AnnualDaysRest -= log.CutHour; // caculate.Caculate(breakoff.BreakOffFrom, breakoff.BreakOffTo);
                log.LeaveType = "Annual Leave";
                if (userLeave.AnnualDaysRest <= 0)
                    content.Append("now you don't have time");
                else
                    content.Append("now is " + userLeave.AnnualDaysRest.ToString() + " hours.");
                
            }
            else
            {
                string time;
                if (log.CutHour == 1)
                    time = " hour,";
                else
                    time = " hours,";
                content.Append("Your Change off time has decreased " + log.CutHour.ToString() + time);
                if(userLeave.OverTimeRest == 1)
                    time = " hour,";
                else
                    time = " hours,";
                content.Append(" it used to be " + userLeave.OverTimeRest.ToString() + time);
                userLeave.OverTimeRest -= log.CutHour;// caculate.Caculate(breakoff.BreakOffFrom, breakoff.BreakOffTo);
                log.LeaveType = "Change Off";
                if (userLeave.OverTimeRest <= 0)
                    content.Append("now you don't have time");
                else
                {
                    if (userLeave.OverTimeRest == 1)
                        time = " hour.";
                    else
                        time = " hours.";
                    content.Append("now is " + userLeave.OverTimeRest.ToString() + time);
                }
            }
            userLeave.UpdateTime = DateTime.Now;
            db.UserLeaveCutLogs.Add(log);

            //Add t_AlertUser
            t_AlertUser alert = new t_AlertUser();

            alert.ABID = 2;
            alert.UserID = breakoff.UserID;
            alert.AUTitle = "BreakOff Approved";
            alert.AUContent = content.ToString();
            alert.AUType = 0;
            alert.AUStatus = 1;
            alert.AUInterval = 0;
            alert.AUUnitNum = 0;
            alert.AUTime = DateTime.Now;
            alert.CreateBy = userID;
            alert.CreateDate = DateTime.Now;

            db.t_AlertUser.Add(alert);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// BreakOff Canceled
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool BreakOffCancel(Guid id, string userID)
        {
            BreakOff breakoff = db.BreakOffs.Find(id);

            if (breakoff == null)
            {
                return false;
            }
            breakoff.Status = "Cancel";

            t_AlertUser alert = new t_AlertUser();

            alert.ABID = 2;
            alert.UserID = breakoff.UserID;
            alert.AUTitle = "BreakOff Canceled";
            alert.AUContent = "I have Canceled your BreakOff.";
            alert.AUType = 0;
            alert.AUStatus = 1;
            alert.AUInterval = 0;
            alert.AUUnitNum = 0;
            alert.AUTime = DateTime.Now;
            alert.CreateBy = userID;
            alert.CreateDate = DateTime.Now;

            db.t_AlertUser.Add(alert);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// OverTimeApplication
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hours"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool OverTimeApplication(DateTime date, int hours, string userID)
        {
            OverTime overTime = new OverTime();
            overTime.OverTimeGuid = Guid.NewGuid();
            overTime.UserID = userID;
            overTime.OnDate = date;
            overTime.Hours = hours;
            overTime.Status = "Ask";

            db.OverTimes.Add(overTime);

            var user = db.Users.FirstOrDefault(o => o.UserID == userID);
            StringBuilder email = new StringBuilder();
            string hourstr = " hours";
            if (overTime.Hours == 1)
                hourstr = " hour";
            hourstr = overTime.Hours.ToString() + hourstr;
            //email.Append("Hi Yemol, <br/><br/>");
            email.AppendFormat("I am going to overtime {0} hours at {1}.", hourstr, overTime.OnDate.ToShortDateString());
            email.AppendFormat(" Could you please approve it?");//<br/><br/>Thanks,<br/><br/><br/>{0}", user.UserName);
            t_AlertUser alert = new t_AlertUser();

            alert.ABID = 1;
            alert.UserID = "yemol";
            alert.AUTitle = user.UserName + " OverTime Application";
            alert.AUContent = email.ToString();
            alert.AUType = 0;
            alert.AUStatus = 1;
            alert.AUInterval = 0;
            alert.AUUnitNum = 0;
            alert.AUTime = DateTime.Now;
            alert.CreateBy = userID;
            alert.CreateDate = DateTime.Now;

            db.t_AlertUser.Add(alert);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// OverTime Approved
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool OverTimeApprove(Guid id, string userID)
        {
            OverTime overtime = db.OverTimes.Find(id);

            if (overtime == null)
            {
                return false;
            }
            overtime.Status = "Approve";

            //update UserLeave
            var userLeave = db.UserLeaves.Where(o => o.UserID == overtime.UserID).FirstOrDefault();
            userLeave.OverTime += overtime.Hours;
            userLeave.OverTimeRest += overtime.Hours;
            userLeave.UpdateTime = DateTime.Now;


            //Add UserLeaveLog
            UserLeaveCutLog log = new UserLeaveCutLog();
            log.BreakOFFID = Guid.NewGuid();
            log.CutDateTime = DateTime.Now;
            log.UserID = overtime.UserID;
            log.CutHour = overtime.Hours;
            log.LeaveType = "OverTime";
            db.UserLeaveCutLogs.Add(log);

            //Add t_AlertUser
            t_AlertUser alert = new t_AlertUser();

            alert.ABID = 2;
            alert.UserID = overtime.UserID;
            alert.AUTitle = "OverTime Approved";
            alert.AUContent = "I have approved your OverTime.";
            alert.AUType = 0;
            alert.AUStatus = 1;
            alert.AUInterval = 0;
            alert.AUUnitNum = 0;
            alert.AUTime = DateTime.Now;
            alert.CreateBy = userID;
            alert.CreateDate = DateTime.Now;

            db.t_AlertUser.Add(alert);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// OverTime Canceled
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool OverTimeCancel(Guid id, string userID)
        {
            OverTime overtime = db.OverTimes.Find(id);

            if (overtime == null)
            {
                return false;
            }
            overtime.Status = "Cancel";

            t_AlertUser alert = new t_AlertUser();

            alert.ABID = 2;
            alert.UserID = overtime.UserID;
            alert.AUTitle = "OverTime Canceled";
            alert.AUContent = "I have Canceled your OverTime";
            alert.AUType = 0;
            alert.AUStatus = 1;
            alert.AUInterval = 0;
            alert.AUUnitNum = 0;
            alert.AUTime = DateTime.Now;
            alert.CreateBy = userID;
            alert.CreateDate = DateTime.Now;

            db.t_AlertUser.Add(alert);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// BreakOffApplication
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool TraineeBreakOffApplication(DateTime start, DateTime end, string userID)
        {
            TraineeBreakOff traineeBreakOff = new TraineeBreakOff();

            traineeBreakOff.BreakOffGuid = Guid.NewGuid();
            traineeBreakOff.UserID = userID;
            traineeBreakOff.BreakOffFrom = start;
            traineeBreakOff.BreakOffTo = end;
            traineeBreakOff.Status = "Ask";

            db.TraineeBreakOffs.Add(traineeBreakOff);

            var user = db.Users.FirstOrDefault(o => o.UserID == userID);
            StringBuilder email = new StringBuilder();
            CaculateCost caculate = new CaculateCost();

            string time = caculate.Caculate(start, end).ToString() + "H";
            int timeCost = Convert.ToInt32(time.Substring(0, time.Length - 1));
            int days = timeCost / 8;
            timeCost = timeCost - days * 8;

            time = string.Empty;
            if (days > 0)
                time += " " + days.ToString() + (days > 1 ? " Days" : " Day");
            if (timeCost > 0)
                time += " " + timeCost.ToString() + (timeCost > 1 ? " Hours" : " Hour");

            if (end.Subtract(start).Days == 0)
                time = time + start.ToShortDateString() + ".";
            else
                time = time + start.ToShortDateString() + " to " + end.ToShortDateString() + ".";
            //email.Append("Hi Yemol, <br/><br/>");
            email.Append("I am going to take" + time);
            email.AppendFormat(" Could you please approve it?");//<br/><br/>Thanks,<br/><br/><br/>{0}", user.UserName);

                    t_AlertUser alert = new t_AlertUser();

                    alert.ABID = 1;
                    alert.UserID = "yemol";
                    alert.AUTitle = user.UserName + " TraineeBreakOff Application";
                    alert.AUContent = email.ToString();
                    alert.AUType = 0;
                    alert.AUStatus = 1;
                    alert.AUInterval = 0;
                    alert.AUUnitNum = 0;
                    alert.AUTime = DateTime.Now;
                    alert.CreateBy = userID;
                    alert.CreateDate = DateTime.Now;

                    db.t_AlertUser.Add(alert);

            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// BreakOff Approved
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool TraineeBreakOffApprove(Guid id, string userID)
        {
            //update TrianeeBreakOff
            TraineeBreakOff traineebreakoff = db.TraineeBreakOffs.Find(id);
            CaculateCost caculate = new CaculateCost();

            if (traineebreakoff == null)
            {
                return false;
            }
            traineebreakoff.Status = "Approve";

            //Add t_AlertUser
            t_AlertUser alert = new t_AlertUser();

            alert.ABID = 2;
            alert.UserID = traineebreakoff.UserID;
            alert.AUTitle = "TraineeBreakOff Approved";
            alert.AUContent = "I have approved your BreakOff";
            alert.AUType = 0;
            alert.AUStatus = 1;
            alert.AUInterval = 0;
            alert.AUUnitNum = 0;
            alert.AUTime = DateTime.Now;
            alert.CreateBy = userID;
            alert.CreateDate = DateTime.Now;

            db.t_AlertUser.Add(alert);
            try
            {
                db.SaveChanges();
            }
            catch 
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// BreakOff Canceled
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool TraineeBreakOffCancel(Guid id, string userID)
        {
            TraineeBreakOff traineebreakoff = db.TraineeBreakOffs.Find(id);

            if (traineebreakoff == null)
            {
                return false;
            }
            traineebreakoff.Status = "Cancel";

            t_AlertUser alert = new t_AlertUser();

            alert.ABID = 2;
            alert.UserID = traineebreakoff.UserID;
            alert.AUTitle = "TraineeBreakOff Canceled";
            alert.AUContent = "I have Canceled your BreakOff.";
            alert.AUType = 0;
            alert.AUStatus = 1;
            alert.AUInterval = 0;
            alert.AUUnitNum = 0;
            alert.AUTime = DateTime.Now;
            alert.CreateBy = userID;
            alert.CreateDate = DateTime.Now;

            db.t_AlertUser.Add(alert);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}