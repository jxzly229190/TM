using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using QA_System.Models;
using QA_System.Classes;
using QA_System.DataProvider;


namespace QA_System.Controllers.Request
{
    public class RequestController : BasePageController
    {

        #region View of RequestTestList, RequestTest, RequestDetail
        [SessionExpireFilter]
        public ActionResult RequestTest(string requestID, string pageIndex, string sort)
        {
            ViewBag.Message = "Send a test request.";
            ViewBag.pageIndex = pageIndex;
            ViewBag.sort = sort;

            RequestTestModels requestTest = new RequestTestModels();
            if (!string.IsNullOrEmpty(requestID))
            {
                requestTest = GetRequestTestByID(requestID);
            }
            
            return View(requestTest);
        }

        [SessionExpireFilter]
        public ActionResult RequestTestList(int? pageIndex, string sort, string sortDirection, string user)
        {
            ViewBag.sort = sort;
            if (!pageIndex.HasValue)
            {
                pageIndex = 1;
            }
            if (string.IsNullOrEmpty(sort))
            {
                sort = "UpdateTime";
            }
            if (string.IsNullOrEmpty(sortDirection))
            {
                sortDirection = "descending";
            }
            ViewBag.sortDirection = (sortDirection == "ascending") ? "descending" : "ascending";
            if (string.IsNullOrEmpty(user))
            {
                user = Session[SESSION_USERID].ToString();
                ViewBag.showAllUser = false;
            }
            else if (user == "All")
            {
                user = ""; // show all requests
                ViewBag.showAllUser = true;
            }

            DataBase db = new DataBase();
            List<RequestTestModels> TestRequestList = new List<RequestTestModels>();
            TestRequestList = db.GetRequestTest(null, null, "", user, "");
            PagerInfo pageInfo = new PagerInfo();
            pageInfo.PageSize = PAGESIZE;
            pageInfo.PageIndex = pageIndex.Value;
            pageInfo.PageTotalCount = TestRequestList.Count;

            var query = from tr in TestRequestList
                        orderby Utils.GetPropertyValue(tr, tr.TestModel, sort) descending
                        select tr;
            if (sortDirection == "ascending")
            {
                query = from tr in TestRequestList
                        orderby Utils.GetPropertyValue(tr, tr.TestModel, sort) ascending
                        select tr;
            }

            IEnumerable<RequestTestModels> data = query.Skip(pageInfo.PageSize * (pageIndex.Value - 1)).Take(pageInfo.PageSize);
            PageQuery<PagerInfo, IEnumerable<RequestTestModels>> requestTestQuery = new PageQuery<PagerInfo, IEnumerable<RequestTestModels>>(pageInfo, data);
            return View(requestTestQuery);
        }

        [SessionExpireFilter]
        public ActionResult RequestTestDetail(string requestID)
        {
            RequestTestModels requestTest = GetRequestTestByID(requestID);

            return View(requestTest);
        }
        #endregion

        [HttpPost]
        [SessionExpireFilter]
        public ActionResult SaveRequestTest(FormCollection formValues)
        {
            bool isSuccess = true;
            string errorMsg = string.Empty;
            string url = "../Request/RequestTestList";
            try
            {
                DataBase db = new DataBase();
                RequestTestModels requestTest = new RequestTestModels();
                
                requestTest.Project = formValues["selproject"].ToString();
                requestTest.RelatedJIRA = formValues["txtrelatedJIRA"].ToString();
                requestTest.Developer = Session[SESSION_USERID].ToString();
                requestTest.Instruction = formValues["txtInstruction"].ToString();
                requestTest.Comments = formValues["txtComments"].ToString();
                if (formValues["txtEnvironment"] != null)
                {
                    requestTest.OtherFields.Environment = formValues["txtEnvironment"].ToString();
                }

                //deal with request attachment
                HttpFileCollectionBase fc = HttpContext.Request.Files;
                List<RequestTestFiles> attachmentlist = SaveUploadFiles(fc);
                requestTest.RequestTestFiles.AddRange(attachmentlist);
                if (formValues["txtRequestID"] != null && !string.IsNullOrEmpty(formValues["txtRequestID"].ToString()) && formValues["txtRequestID"].ToString() != new Guid().ToString())
                {
                    requestTest.ReqestID = new Guid(formValues["txtRequestID"].ToString());
                    db.UpdateRequestTest(requestTest);                    
                    if (formValues["pageIndex"] != null && !string.IsNullOrEmpty(formValues["pageIndex"].ToString().Trim()))
                    {
                        url += "?pageIndex=" + formValues["pageIndex"];
                    }
                }
                else
                {
                    requestTest.ReqestID = Guid.NewGuid();
                    db.CreateRequestTest(requestTest);
                    //Create Test
                    QA_System.Models.Test test = new QA_System.Models.Test();
                    TestProvider testProvider = new TestProvider();
                    test.TestReportID = Guid.NewGuid();
                    test.RequestID = requestTest.ReqestID;
                    test.Status = Models.Test.Types.NotStarted;
                    testProvider.CreateTest(test);
                }
                
                if (requestTest.RequestTestFiles.Count > 0)
                {
                    db.CreateRequestTestFile(requestTest);
                }
            
            }
            catch(Exception ex)
            {
                isSuccess = false;
                errorMsg = ex.ToString();
            }
            
            return Json(new { IsSuccess = isSuccess, href = url, errorMsg = errorMsg }, "text/html", JsonRequestBehavior.AllowGet);           
        }

        // Save Upload Files
        public List<RequestTestFiles> SaveUploadFiles(HttpFileCollectionBase files)
        {
            List<RequestTestFiles> attachmentlist = new List<RequestTestFiles>();
            UploadFiles uploadFiles = new UploadFiles();
            string fileExt = string.Empty;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase hpfb = files[i];
                Guid fileID;
                if (hpfb.ContentLength > 0)
                {                    
                    uploadFiles.SaveUploadFiles(hpfb.InputStream, hpfb.FileName, out fileExt, out fileID);
                    RequestTestFiles requestTestFile = new RequestTestFiles();
                    requestTestFile.RequestTestFileID = fileID;
                    requestTestFile.RequestTestFileName = hpfb.FileName;
                    requestTestFile.RequestTestFileExt = fileExt;
                    attachmentlist.Add(requestTestFile);
                }
            }
            return attachmentlist;
        }

        /// <summary>
        /// Delete Request file by fileID
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public void RequestTestFileDelete(string fileID)
        {
            DataBase db = new DataBase();
            try
            {
                db.DeleteTestRequestFile(new Guid(fileID));
            }
            catch{}
        }

        public RequestTestModels GetRequestTestByID(string requestID)
        {
            RequestTestModels requestTest = new RequestTestModels();
            if (!string.IsNullOrEmpty(requestID))
            {
                DataBase db = new DataBase();
                requestTest = db.GetRequestTestByRequestID(new Guid(requestID));
            }
            ViewBag.request = requestTest;
            return requestTest;
        }
    }
}
