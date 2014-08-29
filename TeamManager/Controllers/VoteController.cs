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
    public class VoteController : Controller
    {
        private TeamManage_Entities db = new TeamManage_Entities();
        private const int DefaultPageSize = 12;
        //
        // GET: /Vote/
        public ActionResult Index()
        {
            return null;
        }

        [HttpGet]
        public ActionResult Vote(int pId=0)
        {
            VoteProject voteProject =null;
            if (pId > 0)
            {
                voteProject = db.VoteProjects.FirstOrDefault(p => p.Id == pId);
            }
            else
            {
                voteProject = db.VoteProjects.FirstOrDefault(p => p.State == 0 && (p.EndTime > DateTime.Now&&p.BeginTime<=DateTime.Now));
            }

            if (voteProject == null)
            {
                ViewBag.Message = "Wowo~~, there is nothing to vote.";
                return View("Alert");
            }

            pId = voteProject.Id;
            //Read the VoteItems
            var voteItems = db.VoteItems.Where(i => i.PId == pId && i.State == 0).ToList();

            var voteDetails = db.VoteDetails.Where(d => d.State == 0 && d.PId == pId && d.Voter == User.Identity.Name.Split(',')[0]);
                        
            var itemModels = from i in voteItems select new VoteItemModel() { Id = i.Id, Comment = i.Comment, PId = i.PId, IsSelected = voteDetails.FirstOrDefault(d => d.IId == i.Id) != null, Name = i.Name, Nominees = i.Nominees, Nominator = i.Nominator, State = 0 };

            var voteModel = new VoteModel() { Items = itemModels, Project = voteProject };

            return View("Vote", voteModel);
        }

        public JsonResult GetItems()
        {
            var list = new List<VoteItemModel>() { 
                new VoteItemModel(){
                Id=1,
                Name="Jimmy",
                Nominees="ABC,BCD",
                IsSelected=false
            },new VoteItemModel(){
                Id=2,
                Name="Peter",
                Nominees="ABC,BCD",
                IsSelected=true
            }};

            return Json(list,JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public JsonResult PostVote(int pId, int[] iIds)
        {
            try
            {
                var voteDetails = db.VoteDetails.Where(d => d.State == 0 && d.PId == pId && d.Voter == User.Identity.Name.Split(',')[0]).ToList();
                foreach (var id in iIds)
                {
                    if (voteDetails.Where(d => d.Id == id).Count() <= 0) //if not contains, then new one.
                    {
                        var newDetail = db.VoteDetails.Create();
                        newDetail.PId = pId;
                        newDetail.IId = id;
                        newDetail.State = 0;
                        newDetail.Voter = User.Identity.Name.Split(',')[0];
                        newDetail.CreatedBy = User.Identity.Name.Split(',')[0];
                        newDetail.CreatedTime = DateTime.Now;
                    }
                    else //if contains, then remove the entry from the voteDetails.
                    {
                        voteDetails.Remove(voteDetails.FirstOrDefault(d => d.Id == id));
                    }
                }

                foreach (var vd in voteDetails)
                {
                    vd.State = 1;                    
                }

                db.SaveChanges();
                return Json(new ResponseModel() { state = 0, msg = "ok" });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel() { state = -1, msg = "oops~, some error has been occurred on server side.", data = ex });
            }
        }

        public ActionResult ItemAdd()
        {
            return View("View2");
        }

        [HttpPost]
        public ActionResult ItemAdd(VoteItemModel voteItem)
        {
            return RedirectToAction("Index");
        }


        #region Project
        /// <summary>
        /// Vote Project Detail Index
        /// </summary>
        /// <returns></returns>
        public ActionResult VoteProjectDetailIndex(int page = 1)
        {
            //ViewData["List"] = GetList();
            //int currentPageIndex = page < 0 ? 0 : page - 1;
            //string userid = User.Identity.Name.Split(',')[0];
            //var voteProject = db.VoteProjects.OrderByDescending(o => o.CreatedTime).ToList();
            //return View(voteProject.ToPagedList(currentPageIndex, DefaultPageSize, voteProject.Count));
            return View();
        }

        /// <summary>
        /// Vote Project Index
        /// </summary>
        /// <returns></returns>
        public ActionResult VoteProjectIndex(int page = 1)
        {
            ViewData["List"] = GetList();
            int currentPageIndex = page < 0 ? 0 : page - 1;
            string userid = User.Identity.Name.Split(',')[0];
            var voteProject = db.VoteProjects.OrderByDescending(o => o.CreatedTime).ToList();
            return View(voteProject.ToPagedList(currentPageIndex, DefaultPageSize, voteProject.Count));
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
        /// Create new project view
        /// </summary>
        /// <returns></returns>
        public ActionResult VoteProjectAdd()
        {
            return View();
        }

        /// <summary>
        /// Create new
        /// </summary>
        /// <param name="vpList"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VoteProjectAdd(VoteProject vpList)
        {
            if (ModelState.IsValid)
            {
                vpList.State = 0;//For extension
                vpList.CreatedTime = DateTime.Now;
                vpList.CreatedBy = User.Identity.Name.Split(',')[0];
                vpList.ModifiedTime = DateTime.Now;
                vpList.ModifiedBy = User.Identity.Name.Split(',')[0];
                db.VoteProjects.Add(vpList);
                db.SaveChanges();

                return RedirectToAction("VoteProjectIndex");
            }
            else
            { return View(vpList); }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult VoteProjectEdit(int id)
        {
            VoteProjectModel vpModel = new VoteProjectModel();
            vpModel.voteproject = db.VoteProjects.Find(id);
            if (vpModel == null)
            {
                return HttpNotFound();
            }
            ViewData["List"] = GetList();
            return View(vpModel);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="voteproject"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VoteProjectEdit(VoteProject voteproject)
        {
            if (ModelState.IsValid)
            {
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
        public ActionResult VoteProjectDelete(int id)
        {
            VoteProjectModel vpModel = new VoteProjectModel();
            vpModel.voteproject = db.VoteProjects.Find(id);
            if (vpModel == null)
            {
                return HttpNotFound();
            }
            ViewData["List"] = GetList();

            return View(vpModel);
        }
        /// <summary>
        /// delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("VoteProjectDelete")]
        [ValidateAntiForgeryToken]
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
    }
}
