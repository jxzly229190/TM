using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamManager.Classes;
using TeamManager.DataProvider;
using MvcPaging;
using System.Data;
using TeamManager.Models;

namespace TeamManager.Controllers
{
    /*
     * todo: 
     * 1. Only edit self item and project
     * 2. Validate the request type(Ajax/Form).
     * 3. Exceed project vote count.
     */

    [Authorize]
    public class VoteController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private const int DefaultPageSize = 12;

        //
        // GET: /Vote/
        public ActionResult Index()
        {
            return Vote();
        }

        [HttpGet]
        [VoteAuth(AuthType.EqualsAndGreatThan,AuthRole.SE)]
        public ActionResult Vote(int pId = 0)
        {
            VoteProject voteProject = GetVoteProject(pId);

            if (voteProject == null)
            {
                ViewBag.Message = "Wowo~~, there is nothing to vote.";
                return View("Alert");
            }

            //Check the project if it is finished.
            if (voteProject.EndTime < DateTime.Now)
            {
                return GetVoteResultView(voteProject);
            }

            pId = voteProject.Id;
            ViewBag.projects = GetVoteProcessProjects();
            var voteModel = GetVoteModel(voteProject);

            return View("Vote", voteModel);
        }

        public JsonResult GetVoteModel(int pid)
        {
            if (pid == 0)
            {
                return Json(new ResponseModel() { state = -1, msg = "pid cannot be 0!" }, JsonRequestBehavior.AllowGet);
            }

            VoteProject voteProject = GetVoteProject(pid);

            if (voteProject == null)
            {
                return Json(new ResponseModel() { state = -1, msg = "Cannot find this project!" }, JsonRequestBehavior.AllowGet);
            }

            //Check the project if it is finished.
            if (voteProject.EndTime < DateTime.Now)
            {
                return Json(new ResponseModel() { state = -1, msg = "Vote has been finished!" }, JsonRequestBehavior.AllowGet);
            }

            var voteModel = GetVoteModel(voteProject);

            return Json(new ResponseModel() { state = 0, msg = "ok", data = voteModel }, JsonRequestBehavior.AllowGet);
        }

        private VoteModel GetVoteModel(VoteProject voteProject)
        {   
            //Get the VoteItems
            var voteItems = db.VoteItems.Where(i => i.PId == voteProject.Id && i.State == 0).ToList();
            var userId = User.Identity.Name.Split(',')[0];
            var voteDetails = db.VoteDetails.Where(d => d.State == 0 && d.PId == voteProject.Id && d.Voter == userId);

            var itemModels = new List<VoteItemModel>();

            foreach (var i in voteItems)
            {
                var itemModel = new VoteItemModel()
                {
                    Id = i.Id,
                    Comment = i.Comment,
                    PId = i.PId,
                    Name = i.Name,
                    Members = i.Nominees,
                    Nominator = i.Nominator,
                    State = 0
                };

                var detail = voteDetails.FirstOrDefault(d => d.IId == i.Id);

                itemModel.IsSelected = detail == null;//?false:true;
                itemModel.PreSelected = detail != null;//?false:true;

                itemModels.Add(itemModel);
            }

            var voteModel = new VoteModel() { Items = itemModels, Project = voteProject, CurrentUserVoteNum = voteDetails.ToList().Count };

            return voteModel;
        }

        private IList<DataProvider.VoteProject> GetVoteProcessProjects()
        {
            return db.VoteProjects.Where(p => p.State == 0 && (p.BeginTime < DateTime.Now && p.EndTime > DateTime.Now)).ToList();
        }

        [HttpPost]
        public JsonResult PostVote(int pId, string iIds)
        {
            if (pId <= 0)
            {
                return Json(new ResponseModel() { state = -1, msg = "pId is null" });
            }

            if (iIds == null)
            {
                return Json(new ResponseModel() { state = -1, msg = "iIds is null" });
            }

            var ids = iIds.Split(',');

            var voteProject=this.GetVoteProject(pId);

            if(voteProject==null){
                return Json(new ResponseModel() { state = -1, msg = "Sorry, we cannot find the project."});
            }

            if(voteProject.BeginTime > DateTime.Now){
                 return Json(new ResponseModel() { state = -1, msg = "Sorry, this project has not began to vote."});
            }

            if(voteProject.EndTime < DateTime.Now){
                 return Json(new ResponseModel() { state = -1, msg = "Sorry, this project has end to vote."});
            }

            if (ids.Count() > voteProject.VoteNum)
            {
                return Json(new ResponseModel() { state = -1, msg = "Sorry, the max vote number is " + voteProject.VoteNum });
            }

            try
            {
                var userId = User.Identity.Name.Split(',')[0];
                var voteDetails = db.VoteDetails.Where(d => d.PId == pId && d.Voter == userId && d.State == 0).ToList();

                foreach (var id in ids)
                {
                    int ID = Convert.ToInt32(id);
                    if (voteDetails.Where(d => d.State == 0 && d.PId == pId && d.IId == ID).Count() <= 0) //if not contains, then new one.
                    {
                        var newDetail = db.VoteDetails.Create();
                        newDetail.PId = pId;
                        newDetail.IId = ID;
                        newDetail.State = 0;
                        newDetail.Voter = User.Identity.Name.Split(',')[0];
                        newDetail.CreatedBy = User.Identity.Name.Split(',')[0];
                        newDetail.CreatedTime = DateTime.Now;

                        db.VoteDetails.Add(newDetail);
                    }
                    else //if contains, then remove the entry from the voteDetails.
                    {
                        voteDetails.Remove(voteDetails.FirstOrDefault(d => d.PId == pId && d.IId == ID));
                    }
                }

                foreach (var vd in voteDetails)
                {
                    vd.State = 1;
                    vd.ModifiedBy = User.Identity.Name.Split(',')[0];
                    vd.ModifiedTime = DateTime.Now;
                }

                db.SaveChanges();
                return Json(new ResponseModel() { state = 0, msg = "ok" });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel() { state = -1, msg = "Oops~, some error has been occurred on server side.", data = ex });
            }
        }

        public ActionResult Result(int pId = 0)
        {
            VoteProject voteProject = GetVoteProject(pId);

            return GetVoteResultView(voteProject);
        }

        #region Vote Item
        
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult ItemAdd(int id)
        {
            var voteProject = GetVoteProject(id);
            
            if (voteProject == null)
            {
                ViewBag.Message = "oops~~, we cannot find the vote project.";
                return View("Alert");
            }

            if (voteProject.BeginTime < DateTime.Now)
            {
                ViewBag.Message = "Sorry, this project has began to vote, cannot add item.";
                return View("Alert");
            }

            var voteItem = new VoteItemModel() { PId = voteProject.Id, ProjectName = voteProject.Name };

            ViewBag.users = db.Users.Where(u => u.UserName != null && u.UserName.Length > 0).OrderBy(i => i.UserName).ToList();

            return View("ItemAdd", voteItem);
        }

        private bool ValidateItemName(string name,Nullable<int> pId, Nullable<int> id)
        {
            var item = db.VoteItems.FirstOrDefault(p => p.State == 0 && p.PId == pId && p.Name == name && p.Id != id);

            if (item == null)
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult ItemAdd(VoteItemModel voteItem)
        {
            var voteProject = GetVoteProject(Convert.ToInt32(voteItem.PId));

            if (voteProject.BeginTime < DateTime.Now)
            {
                ViewBag.Message = "Sorry, this project has began to vote, cannot add item.";
                ViewBag.users = db.Users.Where(u => u.UserName != null && u.UserName.Length > 0).OrderBy(i => i.UserName).ToList();
                return View(voteItem);
            }           

            if (voteItem != null)
            {
                if (!ValidateItemName(voteItem.Name, voteItem.PId, voteItem.Id))
                {
                    ViewBag.Message = "Sorry, this item name has been used in this project.";
                    ViewBag.users = db.Users.Where(u => u.UserName != null && u.UserName.Length > 0).OrderBy(i => i.UserName).ToList();
                    return View(voteItem);
                }

                var userId = User.Identity.Name.Split(',')[0];
                var voteItemDB = new VoteItem
                {
                    PId = voteItem.PId,
                    Comment = voteItem.Comment,
                    CreatedBy = userId,
                    CreatedTime = DateTime.Now,
                    Name = voteItem.Name,
                    Nominator = userId,
                    Nominees = voteItem.Members,
                    State = 0
                };

                try
                {
                    db.VoteItems.Add(voteItemDB);
                    db.SaveChanges();

                    return RedirectToAction("ItemList", new { id = voteItem.PId });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "DatabaseError: "+ex.Message;
                    ViewBag.users = db.Users.Where(u => u.UserName != null && u.UserName.Length > 0).OrderBy(i => i.UserName).ToList();
                    return View(voteItem);
                    //return View("DatabaseError");
                    //throw;
                }
            }
            else
            {
                ViewBag.Message = "VoteItemModel is null.";
                ViewBag.users = db.Users.Where(u => u.UserName != null && u.UserName.Length > 0).OrderBy(i => i.UserName).ToList();
                return View(voteItem);
                //return View("Error");
            }
        }

        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult ItemEdit(int id,int pid)
        {
            var voteProject = GetVoteProject(pid);

            if (voteProject.BeginTime < DateTime.Now)
            {
                ViewBag.Message = "Sorry, this project has began to vote, cannot add item.";
                return View("Alert");
            }

            var voteItem = db.VoteItems.FirstOrDefault(i => i.State == 0 && i.Id == id);
            if (voteItem.Nominator != User.Identity.Name.Split(',')[0])
            {
                ViewBag.Message = "Sorry, this item was created by others, you cannot edit it.";
                return View("Alert");
            }

            var project = GetVoteProject(pid);

            if (project == null && voteItem == null)
            {
                ViewBag.Message = "Oops~~, the Vote Project is null or the Vote Item is null.";
                return View("Alert");
            }

            var model = new VoteItemModel() { PId = voteItem.PId, Members = voteItem.Nominees, Id = voteItem.Id, Nominator = voteItem.Nominator, Name = voteItem.Name, Comment = voteItem.Comment, ProjectName = project.Name };

            ViewBag.users = db.Users.Where(u => u.UserName != null && u.UserName.Length > 0).ToList();

            return View("ItemEdit", model);
        }

        [HttpPost]
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult ItemEdit(VoteItemModel voteItem)
        {
            var voteProject = GetVoteProject(Convert.ToInt32(voteItem.PId));

            if (voteProject.BeginTime < DateTime.Now)
            {
                ViewBag.Message = "Sorry, this project has began to vote, cannot add item.";
                return View(voteItem);
            }

            if (voteItem != null && voteItem.Id > 0)
            {
                if (voteItem.Nominator != User.Identity.Name.Split(',')[0])
                {
                    ViewBag.Message = "Sorry, this item was created by others, you cannot edit it.";
                    return View(voteItem);
                }

                if (!ValidateItemName(voteItem.Name, voteItem.PId, voteItem.Id))
                {
                    ViewBag.Message = "Sorry, this item name has been used in this project.";
                    return View(voteItem);
                }

                var voteItemDB = db.VoteItems.FirstOrDefault(i => i.Id == voteItem.Id);
                voteItemDB.Name = voteItem.Name;
                voteItemDB.Nominees = voteItem.Members;
                voteItemDB.Comment = voteItem.Comment;
                voteItemDB.ModifiedBy = User.Identity.Name.Split(',')[0];
                voteItemDB.ModifiedTime = DateTime.Now;
                try
                {
                    db.Entry(voteItemDB).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("ItemList", new { id = voteItem.PId });
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "DatabaseError: " + ex.Message;
                    return View(voteItem);
                    //return View("DatabaseError");
                    //throw;
                }
            }
            else
            {
                return View("VoteItemModel is null.");
            }
        }

        /// <summary>
        /// Item List 
        /// </summary>
        /// <param name="id">Project Id</param>
        /// <returns></returns>
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult ItemList(int id)
        {
            var voteProject = GetVoteProject(id);

            if (voteProject == null)
            {
                ViewBag.Message = "oops~~, we cannot find the vote.";
                return View("Alert");
            }

            id = voteProject.Id;

            var itemModels = GetVoteItems(id);

            var voteModel = new VoteModel() { Items = itemModels, Project = voteProject };

            return View("ItemList", voteModel);
        }

        [HttpPost]
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public JsonResult ItemDelete(int id)
        {
            try
            {
                var voteItem = db.VoteItems.FirstOrDefault(i => i.Id == id);
                if (voteItem.Nominator != User.Identity.Name.Split(',')[0])
                {
                    return Json(new ResponseModel() { state = -1, msg = "Sorry, this item was created by others, you cannot edit it.", data = new { id = id } });
                }

                if (voteItem != null)
                {
                    var voteProject = GetVoteProject(Convert.ToInt32(voteItem.PId));

                    if (voteProject.BeginTime < DateTime.Now)
                    {
                        return Json(new ResponseModel() { state = -1, msg = "Sorry, this project has began to vote, you cannot remove item.", data = new { id = id } });
                    }

                    voteItem.State = 255;
                    db.Entry(voteItem).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new ResponseModel() { state = 0, msg = "ok", data = new { id = id } });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel() { state = -1, msg = "error:" + ex.Message, data = ex });
            }
        }
                
        public bool CheckItemNameExists(string name,int pId,Nullable<int> id)
        {
            return ValidateItemName(name, pId, id);
        }
        #endregion

        #region Project

        /// <summary>
        /// Vote Project Index
        /// </summary>
        /// <returns></returns>
        public ActionResult VoteProjectIndex(int page = 1)
        {
            //If vote endtime< current date, then update project status=1
            var vplist = db.VoteProjects.Where(i => i.State == 0).ToList();
            foreach (VoteProject voteproject in vplist)
            {
                if (DateTime.Parse(voteproject.EndTime.ToString()) < DateTime.Now)
                {
                    voteproject.ModifiedTime = DateTime.Now;
                    voteproject.ModifiedBy = User.Identity.Name.Split(',')[0];
                    voteproject.State = 1;
                    db.Entry(voteproject).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            ViewData["UserRole"] = User.Identity.Name.Split(',')[1];
            //View all project
            int currentPageIndex = page < 0 ? 0 : page - 1;
            var voteProject = db.VoteProjects.OrderByDescending(o => o.CreatedTime).ToList();
            return View(voteProject.ToPagedList(currentPageIndex, DefaultPageSize, voteProject.Count));
        }

        /// <summary>
        /// Create new project view
        /// </summary>
        /// <returns></returns>
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult VoteProjectAdd()
        {
            return View();
        }

        /// <summary>
        /// Create new
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult VoteProjectAdd(VoteProject project)
        {
            if (ModelState.IsValid)
            {
                if (project.TermFrom >= project.TermTo)
                {
                    ViewBag.ErrorMsg = "Create failed! The TermFrom's value cannot greater than the TermTo's vlaue.";
                    return View(ConvertProjectToModel(project));  //Convert VoteProject to VoteProjectModel
                }
                if (project.BeginTime >= project.EndTime)
                {
                    ViewBag.ErrorMsg = "Create failed! The BeginTime's value cannot greater than the EndTime's vlaue.";
                    return View(ConvertProjectToModel(project));  //Convert VoteProject to VoteProjectModel
                }

                project.State = 0;
                project.CreatedTime = DateTime.Now;
                project.CreatedBy = User.Identity.Name.Split(',')[0];
                project.ModifiedTime = DateTime.Now;
                project.ModifiedBy = User.Identity.Name.Split(',')[0];
                db.VoteProjects.Add(project);
                db.SaveChanges();                

                return RedirectToAction("VoteProjectIndex");        
            }
            else
            { return View(project); }
        }

        /// <summary>
        /// Convert VoteProject to VoteProjectModel
        /// </summary>
        /// <param name="voteproject"></param>
        /// <returns></returns>
        private VoteProjectModel ConvertProjectToModel(VoteProject voteproject)
        {
            VoteProjectModel vpModel = new VoteProjectModel();
            vpModel.Id = voteproject.Id;
            vpModel.Name = voteproject.Name;
            vpModel.TermFrom = Convert.ToDateTime(voteproject.TermFrom);
            vpModel.TermTo = Convert.ToDateTime(voteproject.TermTo);
            vpModel.State = voteproject.State;
            vpModel.VoteNum = voteproject.VoteNum;
            vpModel.BeginTime = Convert.ToDateTime(voteproject.BeginTime);
            vpModel.EndTime = Convert.ToDateTime(voteproject.EndTime);
            return vpModel;
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult VoteProjectEdit(int id)
        {
            VoteProjectModel vpModel = new VoteProjectModel();
            VoteProject voteproject = db.VoteProjects.Find(id);
            if (vpModel == null)
            {
                return HttpNotFound();
            }
            //Convert VoteProject to VoteProjectModel
            vpModel = ConvertProjectToModel(voteproject);

            return View(vpModel);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="voteproject"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult VoteProjectEdit(VoteProject voteproject)
        {
            if (ModelState.IsValid)
            {
                if (voteproject.BeginTime <= DateTime.Now && GetVoteItems(voteproject.Id).Count() <= 0)
                {
                    ViewBag.ErrorMsg = "Please nominate before updating VoteBeginTime.";
                    return View(ConvertProjectToModel(voteproject));
                }
                if (voteproject.BeginTime >= voteproject.EndTime)
                {
                    ViewBag.ErrorMsg = "Create failed! The BeginTime's value cannot greater than the EndTime's vlaue.";
                    return View(ConvertProjectToModel(voteproject));  //Convert VoteProject to VoteProjectModel
                }
                voteproject.ModifiedTime = DateTime.Now;
                voteproject.ModifiedBy = User.Identity.Name.Split(',')[0];
                db.Entry(voteproject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("VoteProjectIndex");
            }
            return View(voteproject);
        }

        /// <summary>
        /// Delete view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult VoteProjectDelete(int id)
        {
            VoteProjectModel vpModel = new VoteProjectModel();
            VoteProject voteproject = db.VoteProjects.Find(id);
            if (vpModel == null)
            {
                return HttpNotFound();
            }
            //Convert VoteProject to VoteProjectModel
            vpModel = ConvertProjectToModel(voteproject);
            return View(vpModel);
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("VoteProjectDelete")]
        [ValidateAntiForgeryToken]
        [VoteAuth(AuthType.EqualsAndGreatThan, AuthRole.TL)]
        public ActionResult DeleteConfirmed(int id)
        {
            VoteProject vp = db.VoteProjects.Find(id);
            //db.VoteProjects.Remove(vp);
            vp.State = 255;
            vp.ModifiedTime = DateTime.Now;
            vp.ModifiedBy = User.Identity.Name.Split(',')[0];
            db.Entry(vp).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("VoteProjectIndex");
        }

        #endregion

        #region private methods

        //Get the project
        private VoteProject GetVoteProject(int pId)
        {
            VoteProject voteProject = null;
            if (pId > 0)
            {
                voteProject = db.VoteProjects.FirstOrDefault(p => p.Id == pId);
            }
            else
            {
                voteProject = db.VoteProjects.FirstOrDefault(p => p.State == 0 && (p.EndTime > DateTime.Now && p.BeginTime <= DateTime.Now));
            }

            if (voteProject == null)
            {
               var vps = db.VoteProjects.Where(p => (p.State == 0||p.State==1) && p.EndTime < DateTime.Now).OrderByDescending(p => p.EndTime).Take(1).ToList();
               if (vps != null && vps.Count() > 0)
               {
                   voteProject = vps[0];
               }
            }

            return voteProject;
        }

        private IList<VoteItemModel> GetVoteItems(int pid)
        {
            //Get the VoteItems
            var voteItems = db.VoteItems.Where(i => i.PId == pid && i.State == 0).ToList();
            var userId = User.Identity.Name.Split(',')[0];
            var voteDetails = db.VoteDetails.Where(d => d.State == 0 && d.PId == pid);

            var itemModels = new List<VoteItemModel>();

            foreach (var i in voteItems)
            {
                var itemModel = new VoteItemModel()
                {
                    Id = i.Id,
                    Comment = i.Comment,
                    PId = i.PId,
                    Name = i.Name,
                    Members = i.Nominees,
                    Nominator = i.Nominator,
                    State = i.State,
                    CreatedBy = i.CreatedBy,
                    CreatedTime = i.CreatedTime
                };

                itemModel.IsSelected = voteDetails.FirstOrDefault(d => d.IId == i.Id && d.Voter == userId) != null;
                itemModel.Count = voteDetails.Where(d => d.IId == i.Id).Count();

                itemModels.Add(itemModel);
            }

            return itemModels;
        }

        private ActionResult GetVoteResultView(VoteProject voteProject)
        {
            if (voteProject == null)
            {
                ViewBag.Message = "Wowo~~, we cannot find the vote.";
                return View("Alert");
            }

            var itemModels = GetVoteItems(voteProject.Id);

            var voteModel = new VoteModel() { Items = itemModels.OrderByDescending(i => i.Count).ToList(), Project = voteProject };

            return View("Result", voteModel);
        }
        #endregion
    }
}
