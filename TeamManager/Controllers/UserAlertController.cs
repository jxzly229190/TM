using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using TeamManager.DataProvider;
using TeamManager.Models;
using TeamManager.Classes;
using System.Data;
using System.Text;

namespace TeamManager.Controllers
{
    public class UserAlertController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private const int DefaultPageSize = 10;

        private MultiSelectList GetUsers(string[] selectedValues)
        {

            List<SelectListItem> users = db.Users.Where(m => m.IsDeparted != true)
                .OrderBy(c => c.UserName)
                .Select(c => new SelectListItem
                {
                    Value = c.UserID,
                    Text = c.UserName
                }).ToList();

            return new MultiSelectList(users, "Value", "Text", selectedValues);

        }

        private MultiSelectList GetCcUsers(string[] selectedValues)
        {

            List<SelectListItem> ccUsers = db.Users.Where(m => m.IsDeparted != true)
                .OrderBy(c => c.UserName)
                .Select(c => new SelectListItem
                {
                    Value = c.UserID,
                    Text = c.UserName
                }).ToList();

            return new MultiSelectList(ccUsers, "Value", "Text", selectedValues);

        }

        private IEnumerable<SelectListItem> GetAlerts()
        {
            IEnumerable<SelectListItem> alerts = db.t_AlertBase
                .AsEnumerable()
                .Select(c => new SelectListItem
                {
                    Value = c.ABID.ToString(),
                    Text = string.Format("{0}({1})", c.ABName,c.ABRemark)
                });
            return alerts;
        }


        private static List<SelectListItem> GetInterval()
        {
            List<SelectListItem> interval = new List<SelectListItem>();
            interval.Add(new SelectListItem { Text = "Day", Value = "0"});
            interval.Add(new SelectListItem { Text = "Month", Value = "1" });
            interval.Add(new SelectListItem { Text = "Year", Value = "2" });
            interval.Add(new SelectListItem { Text = "OneTime", Value = "3" });
            return interval;
        }

        private static List<SelectListItem> SelectUnitNum()
        {
            List<SelectListItem> unitNum = new List<SelectListItem>();
            unitNum.Add(new SelectListItem { Text = "OneTime", Value = "0" });
            unitNum.Add(new SelectListItem { Text = "Not OneTime", Value = "1" });
            return unitNum;
        }
      
        public ActionResult Index(int page = 1, int unitNum = 1)
        {
            int currentPageIndex = page < 0 ? 0 : page - 1;
            List<UserList> usersAttachs = new List<UserList>();
            var users = new List<t_AlertUser>();
            if (unitNum == 0)
            {
                users = db.t_AlertUser.Where(c => c.AUUnitNum == 0).OrderByDescending(c => c.CreateDate).ToList();

            }
            else
            {
                users = db.t_AlertUser.Where(c => c.AUUnitNum != 0).OrderByDescending(c => c.CreateDate).ToList();
            }
            foreach (var item in users)
            {
                UserList userList = new UserList();

                if (item.UserID != null)
                {
                    string[] toUsers = item.UserID.Split(',');
                    StringBuilder temp = BuildUser(toUsers);
                    item.UserID = temp.ToString();
                }

                if (item.CcUser != null)
                {
                    string[] ccUsers = item.CcUser.Split(',');
                    StringBuilder temp = BuildUser(ccUsers);
                    item.CcUser = temp.ToString();
                }


                var attachs = db.t_AUAttach.Where(c => c.AUID == item.AUID).ToList();
                userList.user = item;
                userList.userAttachs = attachs;
                usersAttachs.Add(userList);
            }

            ViewData["SelectUnitNum"] = SelectUnitNum();
            ViewBag.unitNum = unitNum;
            return View(usersAttachs.ToPagedList(currentPageIndex, DefaultPageSize, usersAttachs.Count));
        }

        private static StringBuilder BuildUser(string[] users)
        {
            StringBuilder temp = new StringBuilder();
            for (int i = 0; i < users.Length; i++)
            {
                if ((i + 1) != users.Length)
                {
                    temp.Append(users[i] + "</br>");
                }
                else
                    temp.Append(users[i]);
            }
            return temp;
        }


        public ActionResult AlertIndex(int page = 1)
        {
            int currentPageIndex = page < 0 ? 0 : page - 1;
            List<AlertList> alertsAttachs = new List<AlertList>();
            var alerts = (from c in db.t_AlertBase
                          orderby c.ABID
                          select c).ToList();
            foreach (var item in alerts)
            {
                AlertList alertList = new AlertList();
                var attachs = db.t_ABAttach.Where(c => c.ABID == item.ABID).ToList();
                alertList.alert = item;
                alertList.alertAttachs = attachs;
                alertsAttachs.Add(alertList);
            }
            return View(alertsAttachs.ToPagedList(currentPageIndex, DefaultPageSize, alertsAttachs.Count));
        }


        public ActionResult Details(int id)
        {
            var user = db.t_AlertUser.FirstOrDefault(c => c.AUID == id);

            if (user.UserID != null)
            {
                string[] toUsers = user.UserID.Split(',');
                StringBuilder temp = BuildUser(toUsers);
                user.UserID = temp.ToString();
            }

            if (user.CcUser != null)
            {
                string[] ccUsers = user.CcUser.Split(',');
                StringBuilder temp = BuildUser(ccUsers);
                user.CcUser = temp.ToString();
            }

            var attachs = db.t_AUAttach.Where(c => c.AUID == id).ToList();
            UserList userList = new UserList();
            userList.user = user;
            userList.userAttachs = attachs;

            return View(userList);
        }


        public ActionResult AlertDetails(int id)
        {
            var alert = db.t_AlertBase.First(c => c.ABID == id);
            var attachs = db.t_ABAttach.Where(c => c.ABID == id).ToList();
            AlertList alertList = new AlertList();
            alertList.alert = alert;
            alertList.alertAttachs = attachs;

            return View(alertList);
        }
 
      
        public ActionResult Create()
        {
            ViewBag.UserList = GetUsers(null);
            ViewBag.CcUserList = GetCcUsers(null);
            ViewData["ABID"] = GetAlerts();
            ViewData["List"] = GetInterval();

            return View();
        }
        [HttpPost]
        public ActionResult Create(UserAlertList userAlertList)
        {
            if (ModelState.IsValid)
            {
                userAlertList.userList.user.UserID = Request.Form["ToUser"];
                userAlertList.userList.user.CcUser = Request.Form["CcUser"];

                userAlertList.userList.user.AUInterval = int.Parse(Request.Form["interval"]);
                if (Request.Form["interval"] == "3")
                {                   
                    userAlertList.userList.user.AUUnitNum = 0;                   
                }
                userAlertList.userList.user.AUType = 0;//For extension
                userAlertList.userList.user.CreateDate = DateTime.Now;
                userAlertList.userList.user.CreateBy = User.Identity.Name.Split(',')[0];
                db.t_AlertUser.Add(userAlertList.userList.user);
                db.SaveChanges();

                //UploadFiles(userAlertList);
                UploadFiles(userAlertList.userList);

                return RedirectToAction("Index", new { unitNum = userAlertList.userList.user.AUUnitNum });
            }
            else
                return View(userAlertList);
        }



        public ActionResult CreateAlert()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateAlert(AlertList alertList)
        {
            if (ModelState.IsValid)
            {
                alertList.alert.ABType = 0;//For extension
                alertList.alert.CreateDate = DateTime.Now;
                alertList.alert.CreateBy = User.Identity.Name.Split(',')[0];
                db.t_AlertBase.Add(alertList.alert);
                db.SaveChanges();

                UploadFiles(alertList);
                return RedirectToAction("Create");
            }
            else
            {return View(alertList);}
        }


        public ActionResult Edit(int id)
        {
            
            ViewData["ABID"] = GetAlerts();
            ViewBag.IntervalTime = GetInterval();

            UserList userList = new UserList();
            userList.user = db.t_AlertUser.FirstOrDefault(b => b.AUID == id);

            ViewBag.UserList = GetUsers(userList.user.UserID.Split(','));
            if (userList.user.CcUser!=null)
            {
                ViewBag.CcUserList = GetCcUsers(userList.user.CcUser.Split(','));
            }
            else
            {
                ViewBag.CcUserList = GetCcUsers(null);
            }

            userList.userAttachs = db.t_AUAttach.Where(b => b.AUID == id).ToList();
            if (userList.user == null)
            {
                return RedirectToAction("Index");
            }
            return View(userList);
        }
        [HttpPost]
        public ActionResult Edit(UserList newUserList)
        {
            try
            {
                UserList oldUserList = new UserList();

                t_AlertUser oldUser = db.t_AlertUser.First(b =>
                    b.AUID == newUserList.user.AUID);

                oldUser.UserID = Request.Form["ToUser"];
                oldUser.CcUser = Request.Form["CcUser"];
                oldUser.AUInterval = int.Parse(Request.Form["interval"]);

                if (Request.Form["interval"] == "3")
                {
                    oldUser.AUUnitNum = 0;
                }
                oldUser.ModifyDate = DateTime.Now;
                oldUser.ModifyBy = User.Identity.Name.Split(',')[0];

                List<t_AUAttach> oldAttachs = db.t_AUAttach.Where(b => b.AUID == newUserList.user.AUID).ToList();

                oldUserList.user = oldUser;
                oldUserList.userAttachs = oldAttachs;

                UpdateModel(oldUserList);
                db.SaveChanges();

                return RedirectToAction("Details", new { id = newUserList.user.AUID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Change failed,please read the detailed error information:" + ex.Message);
            }
            return View(newUserList);
        }



        public ActionResult EditAlert(int id)
        {
            AlertList alertList = new AlertList();
            alertList.alert = db.t_AlertBase.FirstOrDefault(b => b.ABID == id);
            alertList.alertAttachs = db.t_ABAttach.Where(b => b.ABID == id).ToList();
            if (alertList.alert == null)
            {
                return RedirectToAction("Index");
            }
            return View(alertList);
        }
        [HttpPost]
        public ActionResult EditAlert(AlertList newAlertList)
        {
            try
            {
                AlertList oldAlertList = new AlertList();
                t_AlertBase oldAlert = db.t_AlertBase.FirstOrDefault(b =>
                    b.ABID == newAlertList.alert.ABID);
                oldAlert.ModifyDate = DateTime.Now;
                oldAlert.ModifyBy = User.Identity.Name.Split(',')[0];

                List<t_ABAttach> oldAttachs = db.t_ABAttach.Where(b => b.ABID == newAlertList.alert.ABID).ToList();

                oldAlertList.alert = oldAlert;
                oldAlertList.alertAttachs = oldAttachs;
                UpdateModel(oldAlertList);

                db.SaveChanges();

                return RedirectToAction("AlertDetails", new { id = newAlertList.alert.ABID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Change failed,please read the detailed error information:" + ex.Message);
            }
            return View(newAlertList);
        }



        public ActionResult Delete(int id)
        {
            UserList userlist = new UserList();
            t_AlertUser user = db.t_AlertUser.First(b => b.AUID == id);
            if (user.UserID != null)
            {
                string[] toUsers = user.UserID.Split(',');
                StringBuilder temp = BuildUser(toUsers);
                user.UserID = temp.ToString();
            }

            if (user.CcUser != null)
            {
                string[] ccUsers = user.CcUser.Split(',');
                StringBuilder temp = BuildUser(ccUsers);
                user.CcUser = temp.ToString();
            }

            List<t_AUAttach> attachs = db.t_AUAttach.Where(a => a.AUID == id).ToList();
            userlist.user = user;
            userlist.userAttachs = attachs;
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            return View(userlist);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            List<t_AUAttach> attachs = db.t_AUAttach.Where(b => b.AUID == id).ToList();
            foreach (var item in attachs)
            {
                db.t_AUAttach.Remove(item);
                db.SaveChanges();
            }
            //int userId = attach.AUID;
            //db.t_AUAttach.Remove(attach);
            //db.SaveChanges();

            t_AlertUser user = db.t_AlertUser.First(b => b.AUID == id);
            db.t_AlertUser.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult AddAttach(int userid)
        {
            List<t_AUAttach> attachs = db.t_AUAttach.Where(a => a.AUID == userid).ToList();
            for (int index = 0; index < this.Request.Files.Count; index++)
            {
                HttpPostedFileBase file = Request.Files[index] as HttpPostedFileBase;
                if (file == null || file.ContentLength == 0) continue;

                t_AUAttach attach = new t_AUAttach();
                attach.AUID = userid;
                attach.AUARemark = Request.Form["attachRemark"];

                string mimeType = Request.Files[index].ContentType;
                Stream fileStream = Request.Files[index].InputStream;
                attach.AUAName = Path.GetFileName(Request.Files[index].FileName);

                int fileLength = Request.Files[index].ContentLength;
                byte[] fileData = new byte[fileLength];
                fileStream.Read(fileData, 0, fileLength);
                attach.AUAFile = fileData;
                fileStream.Close();

                attach.AUAType = 0;//For extension
                attach.AUAStatus = 1;
                attach.CreateDate = DateTime.Now;
                    attach.CreateBy = User.Identity.Name.Split(',')[0];

                attachs.Add(attach);

                db.t_AUAttach.Add(attach);
                db.SaveChanges();
            }
            return RedirectToAction("Edit", new { id = userid });
        }



        public ActionResult EditAttach(int id)
        {
            t_AUAttach attach = db.t_AUAttach.FirstOrDefault(c => c.AUAID == id);
            if (attach == null)
            {
                return RedirectToAction("Index");
            }
            return View(attach);
        }

        [HttpPost]
        public ActionResult EditAttach(t_AUAttach newAttach)
        {
            try
            {
                t_AUAttach oldAttach = db.t_AUAttach.FirstOrDefault(b =>
                    b.AUAID == newAttach.AUAID);
                oldAttach.ModifyDate = DateTime.Now;
                oldAttach.ModifyBy = User.Identity.Name.Split(',')[0];
                TryUpdateModel(oldAttach);
                db.SaveChanges();

                return RedirectToAction("Edit", new { id = oldAttach.AUID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Change failed,please read the detailed error information:" + ex.Message);
            }
            return View(newAttach);
        }



        public ActionResult EditAlertAttach(int id)
        {
            t_ABAttach attach = db.t_ABAttach.FirstOrDefault(c => c.ABAID == id);
            if (attach == null)
            {
                return RedirectToAction("Index");
            }
            return View(attach);
        }
        [HttpPost]
        public ActionResult EditAlertAttach(t_ABAttach newAttach)
        {
            try
            {
                t_ABAttach oldAttach = db.t_ABAttach.FirstOrDefault(b =>
                    b.ABAID == newAttach.ABAID);
                oldAttach.ModifyDate = DateTime.Now;
                oldAttach.ModifyBy = User.Identity.Name.Split(',')[0];
                UpdateModel(oldAttach);
                db.SaveChanges();

                return RedirectToAction("EditAlert", new { id = oldAttach.ABID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Change failed,please read the detailed error information:" + ex.Message);
            }
            return View(newAttach);
        }



        public ActionResult DeleteAttach(int id)
        {
            t_AUAttach attach = db.t_AUAttach.First(b => b.AUAID == id);
            if (attach == null)
            {
                return RedirectToAction("Edit", new { id = attach.AUID });
            }
            return View(attach);
        }
        [HttpPost]
        public ActionResult DeleteAttach(int id, FormCollection collection)
        {
            t_AUAttach attach = db.t_AUAttach.First(b => b.AUAID == id);
            int userId = attach.AUID;
            db.t_AUAttach.Remove(attach);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = userId });
        }



        [HttpPost]
        public ActionResult SendEmail(string[] names)
        { 
            for (int i = 0; i < names.Length; i++)
                {
                    //var name = names[i];
                    //var id = db.Users.Where(a => a.UserID == name).Select(a => a.UserID).First();
                    t_AlertUser alertUser = new t_AlertUser();
                    alertUser.UserID = names[i];
                    alertUser.ABID = db.t_AlertBase.Where(a => a.ABName.ToLower() == "standard").Select(b => b.ABID).FirstOrDefault();
                    alertUser.AUTitle = "DailyReport Alert";
                    alertUser.AUContent = "Please write your DailyReport!";
                    alertUser.AUType = 0;
                    alertUser.AUStatus = 1;
                    alertUser.AUInterval = 3;
                    alertUser.AUUnitNum = 0;
                    alertUser.AUTime = DateTime.Now.AddMinutes(2);
                    alertUser.CreateBy = User.Identity.Name.Split(',')[0];
                    alertUser.CreateDate = DateTime.Now;

                    db.t_AlertUser.Add(alertUser);
                    db.SaveChanges();
                }
            string result = "Send Successfully!";
            return Content(result);  
        }



        public ActionResult DownloadFiles(int id)
        {
            t_AUAttach attach=db.t_AUAttach.First(b=>b.AUAID==id);
            byte[] fileByte = attach.AUAFile.ToArray();

            Stream stream = new MemoryStream(fileByte);
            const long ChunkSize = 102400;
            byte[] buffer = new byte[ChunkSize];

            System.Web.HttpContext.Current.Response.Clear();
            long dataLengthToRead = stream.Length;
            System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(attach.AUAName));
            while (dataLengthToRead > 0 && System.Web.HttpContext.Current.Response.IsClientConnected)
            {
                int lengthRead = stream.Read(buffer, 0, Convert.ToInt32(ChunkSize));
                System.Web.HttpContext.Current.Response.OutputStream.Write(buffer, 0, lengthRead);
                System.Web.HttpContext.Current.Response.Flush();
                dataLengthToRead = dataLengthToRead - lengthRead;
            }
            stream.Close();
            Response.Close();

            return File(fileByte, "application/octet-stream");
        }



        private void UploadFiles(UserList userList)
        {
            List<t_AUAttach> attachs = new List<t_AUAttach>();
            for (int index = 0; index < this.Request.Files.Count; index++)
            {
                HttpPostedFileBase file = Request.Files[index] as HttpPostedFileBase;
                if (file == null || file.ContentLength == 0) continue;

                t_AUAttach attach = new t_AUAttach();
                attach.AUID = userList.user.AUID;

                string txtName = string.Format("txt{0}", index + 1);
                attach.AUARemark = Request.Form[txtName];

                string mimeType = Request.Files[index].ContentType;
                Stream fileStream = Request.Files[index].InputStream;
                attach.AUAName = Path.GetFileName(Request.Files[index].FileName);

                int fileLength = Request.Files[index].ContentLength;
                byte[] fileData = new byte[fileLength];
                fileStream.Read(fileData, 0, fileLength);
                attach.AUAFile = fileData;
                fileStream.Close();

                attach.AUAType = 0;//For extension
                attach.AUAStatus = 1;
                attach.CreateDate = DateTime.Now;
                attach.CreateBy = User.Identity.Name.Split(',')[0];

                attachs.Add(attach);

                db.t_AUAttach.Add(attach);
                db.SaveChanges();
            }
            userList.userAttachs = attachs;
        }



        private void UploadFiles(AlertList alertList)
        {
            List<t_ABAttach> attachs = new List<t_ABAttach>();
            for (int index = 0; index < this.Request.Files.Count; index++)
            {
                HttpPostedFileBase files = this.Request.Files[index] as HttpPostedFileBase;
                if (files == null || files.ContentLength == 0) continue;

                t_ABAttach attach = new t_ABAttach();
                attach.ABID = alertList.alert.ABID;
                attach.ABAStatus = 1;

                string txtName = string.Format("txt{0}", index + 1);
                attach.ABARemark = Request.Form[txtName];

                string mimeType = Request.Files[index].ContentType;
                Stream fileStream = Request.Files[index].InputStream;
                attach.ABAName = Path.GetFileName(Request.Files[index].FileName);

                int fileLength = Request.Files[index].ContentLength;
                byte[] fileData = new byte[fileLength];
                fileStream.Read(fileData, 0, fileLength);
                attach.ABAFile = fileData;
                fileStream.Close();

                attach.ABAType = 0;//For extension
                attach.CreateDate = DateTime.Now;
                attach.CreateBy = User.Identity.Name.Split(',')[0];

                attachs.Add(attach);

                db.t_ABAttach.Add(attach);
                db.SaveChanges();
            }
            alertList.alertAttachs = attachs;
        }

        public ActionResult FeedBackCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FeedBackCreate(UserAlertList userAlertList)
        {
            if (ModelState.IsValid)
            {
                userAlertList.userList.user.UserID = "ming.zhao";
                userAlertList.userList.user.ABID = 2;
                userAlertList.userList.user.AUStatus = 1;
                userAlertList.userList.user.AUInterval = 0;
                userAlertList.userList.user.AUType = 0;
                userAlertList.userList.user.AUTime = DateTime.Now;
                userAlertList.userList.user.AUUnitNum = 0;
                userAlertList.userList.user.AURemark = "System FeedBack";

                userAlertList.userList.user.CreateDate = DateTime.Now;
                userAlertList.userList.user.CreateBy = User.Identity.Name.Split(',')[0];
                db.t_AlertUser.Add(userAlertList.userList.user);
                db.SaveChanges();

                //UploadFiles(userAlertList);
                UploadFiles(userAlertList.userList);

                return RedirectToAction("Index");
            }
            else
                return View(userAlertList);
        }
    }
}

