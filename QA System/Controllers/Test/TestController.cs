using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QA_System.Models;
using QA_System.Classes;
using QA_System.DataProvider;

namespace QA_System.Controllers.Test
{
    public class TestController : BasePageController
    {

        //
        // GET: /Test/
        public ActionResult Index()
        {
            return View();
        }

        #region View Test Assign
        [SessionExpireFilter]
        public ActionResult TestAssign(string requestID, string pageIndex, string sort)
        {
            RequestTestModels requestTest = new RequestTestModels();
            if (!string.IsNullOrEmpty(requestID))
            {
                requestTest = new  QA_System.Controllers.Request.RequestController().GetRequestTestByID(requestID);
            }

            List<UserModels> userlist = new UserProvider().GetAllUser();
            List<SelectListItem> selectlistTester = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Value = "";
            item.Text = "";
            selectlistTester.Add(item);
            item.Selected = true;
            foreach (UserModels user in userlist)
            {
                item = new SelectListItem();
                item.Text = user.UserName;
                item.Value = user.UserID;
                selectlistTester.Add(item);
                if (requestTest.TestModel.Tester == item.Value)
                {
                    item.Selected = true;
                }
            }
            ViewBag.Tester = selectlistTester;
            ViewBag.pageIndex = pageIndex;
            ViewBag.sort = sort;

            return View(requestTest);
        }

        [SessionExpireFilter]
        public ActionResult TestAssignList(int? pageIndex, string sort, string sortDirection)
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

            TestProvider testProvider = new TestProvider();
            DataBase db = new DataBase();
            List<RequestTestModels> TestRequestList = new List<RequestTestModels>();
            //TestRequestList = testProvider.GetRequestByUnAssign();
            TestRequestList = db.GetRequestTest(null, null, "", "", "");
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
            //IEnumerable<RequestTestModels> data = TestRequestList.OrderByDescending(aa => aa.UpdateTime).Skip(pageInfo.PageSize * (pageIndex.Value - 1)).Take(pageInfo.PageSize);
            IEnumerable<RequestTestModels> data = query.Skip(pageInfo.PageSize * (pageIndex.Value - 1)).Take(pageInfo.PageSize);
            PageQuery<PagerInfo, IEnumerable<RequestTestModels>> requestTestQuery = new PageQuery<PagerInfo, IEnumerable<RequestTestModels>>(pageInfo, data);
            return View(requestTestQuery);
        }

        [SessionExpireFilter]
        public ActionResult SaveTestAssign(string requestID, string assignTo)
        {
            TestProvider testProvider = new TestProvider();
            testProvider.SaveAssign(new Guid(requestID), assignTo);
            
            return Json(new { IsSuccess = true }, "text/html", JsonRequestBehavior.AllowGet);           
        }
        #endregion

        # region View TestList and Test
        [SessionExpireFilter]
        public ActionResult TestList(int? pageIndex, string sort, string sortDirection)
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

            DataBase db = new DataBase();
            List<RequestTestModels> TestRequestList = new List<RequestTestModels>();
            TestRequestList = db.GetRequestTest(null, null, "", "", Session[SESSION_USERID].ToString());
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
            //IEnumerable<RequestTestModels> data = TestRequestList.OrderByDescending(aa => aa.InsertDateTime).Skip(pageInfo.PageSize * (pageIndex.Value - 1)).Take(pageInfo.PageSize);
            IEnumerable<RequestTestModels> data = query.Skip(pageInfo.PageSize * (pageIndex.Value - 1)).Take(pageInfo.PageSize);
            PageQuery<PagerInfo, IEnumerable<RequestTestModels>> requestTestQuery = new PageQuery<PagerInfo, IEnumerable<RequestTestModels>>(pageInfo, data);
            return View(requestTestQuery);
        }

        [SessionExpireFilter]
        public ActionResult Test(string requestID)
        {
            RequestTestModels requestTest = new RequestTestModels();
            if (!string.IsNullOrEmpty(requestID))
            {
                DataBase db = new DataBase();
                requestTest = db.GetRequestTestByRequestID(new Guid(requestID));
            }
            return View(requestTest);
        }
        #endregion

        #region Functions on Test View(save Test Status, save Test Result, save Test comment)
        /// <summary>
        /// Save Test Status
        /// </summary>
        /// <param name="testReportID"></param>
        /// <param name="statusValue"></param>
        /// <returns></returns>
        public JsonResult SaveTestStatus(string testReportID, int statusValue)
        {
            bool isSuccess = true;
            string errorMsg = string.Empty;
            string statusType = Models.Test.Types.NotStarted.ToString();
            string statusTypeDisplayName = string.Empty;
            try
            {
                TestProvider testProvider = new TestProvider();
                testProvider.SaveTestStatus(new Guid(testReportID), (Models.Test.Types)statusValue);
                statusType = Enum.GetName(typeof(Models.Test.Types), (Models.Test.Types)statusValue);
                statusTypeDisplayName = Utils.DisplayName((Models.Test.Types)statusValue);
            }
            catch(Exception ex)
            {
                isSuccess = false;
                errorMsg = ex.ToString();
            }

            return Json(new { IsSuccess = isSuccess, TestStatus = statusType, TestStatusDispalyName = statusTypeDisplayName, msg = errorMsg }, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Test Result
        /// </summary>
        /// <param name="testReportID"></param>
        /// <param name="resultValue"></param>
        /// <returns></returns>
        public JsonResult SaveTestResult(string testReportID, int resultValue)
        {
            bool isSuccess = true;
            string errorMsg = string.Empty;
            string resultType = Models.Test.Results.NotPass.ToString();
            string resultTypeDisplayName = string.Empty;
            try
            {
                TestProvider testProvider = new TestProvider();
                testProvider.SaveTestResult(new Guid(testReportID), (Models.Test.Results)resultValue);
                resultTypeDisplayName = Utils.DisplayName((Models.Test.Results)resultValue);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMsg = ex.ToString();
            }

            return Json(new { IsSuccess = isSuccess, TestResult = resultType, TestResultDisplayName = resultTypeDisplayName, msg = errorMsg }, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save CodeEvaluate
        /// </summary>
        /// <param name="testReportID"></param>
        /// <param name="codeValue"></param>
        /// <returns></returns>
        public JsonResult SaveTestCodeEvaluate(string testReportID, int codeValue)
        {
            bool isSuccess = true;
            string errorMsg = string.Empty;
            try
            {
                TestProvider testProvider = new TestProvider();
                testProvider.SaveTestCodeEvaluate(new Guid(testReportID), (Models.Test.CodeEvaluateTypes)codeValue);                
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMsg = ex.ToString();
            }

            return Json(new { IsSuccess = isSuccess, msg = errorMsg }, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Test Comments
        /// </summary>
        /// <param name="testReportID"></param>
        /// <param name="testComments"></param>
        /// <returns></returns>
        public JsonResult SaveTestComments(string testReportID, string testComments)
        {
            bool isSuccess = true;
            string errorMsg = string.Empty;
            try
            {
                TestProvider testProvider = new TestProvider();
                string commentXml = Utils.FormatCommentXml(DateTime.Now, Session[SESSION_USERNAME].ToString(), testComments);
                testProvider.SaveTestComments(new Guid(testReportID), commentXml);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMsg = ex.ToString();
            }

            return Json(new { IsSuccess = isSuccess, msg = errorMsg }, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Functions on Test View for Test Issue(save Test Issue, get Test Issue lists)
        [HttpPost]
        [SessionExpireFilter]
        public JsonResult SaveTestIssue(FormCollection formValues)
        {
            bool isSuccess = true;
            string errorMsg = string.Empty;
            try
            {
                TestProvider testProvider = new TestProvider();
                TestIssue testIssue = new TestIssue();
                testIssue.Description = formValues["txtIssueDes"].ToString();
                if (formValues["testIssueID"] != null  && !string.IsNullOrEmpty(formValues["testIssueID"].ToString()))
                {
                    //Update Test Issue
                    testIssue.TestIssueID = new Guid(formValues["testIssueID"].ToString());
                    testProvider.UpdateTestIssue(testIssue);
                }
                else
                {
                    //Create New Test Issue
                    testIssue.TestReportID = new Guid(formValues["testReportID"].ToString());
                    testIssue.TestIssueID = Guid.NewGuid();
                    testProvider.CreateTestIssue(testIssue);
                }
                
                //deal with attachment
                HttpFileCollectionBase fc = HttpContext.Request.Files;
                List<TestIssueFiles> attachmentlist = SaveUploadFiles(fc);
                testIssue.TestIssueFiles.AddRange(attachmentlist);                

                if(testIssue.TestIssueFiles.Count >0)
                {
                    testProvider.CreateTestIssueFile(testIssue);
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMsg = ex.ToString();
            }

            return Json(new { IsSuccess = isSuccess, errorMsg = errorMsg }, "text/html", JsonRequestBehavior.AllowGet);
        }

        // Save Upload Files
        [SessionExpireFilter]
        public List<TestIssueFiles> SaveUploadFiles(HttpFileCollectionBase files)
        {
            List<TestIssueFiles> attachmentlist = new List<TestIssueFiles>();
            UploadFiles uploadFiles = new UploadFiles();
            string fileExt = string.Empty;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase hpfb = files[i];
                Guid fileID;
                if (hpfb.ContentLength > 0)
                {
                    uploadFiles.SaveUploadFiles(hpfb.InputStream, hpfb.FileName, out fileExt, out fileID);
                    TestIssueFiles testIssueFile = new TestIssueFiles();
                    testIssueFile.TestIssueFileID = fileID;
                    testIssueFile.TestIssueFileName = hpfb.FileName;
                    testIssueFile.TestIssueFileExt = fileExt;
                    attachmentlist.Add(testIssueFile);
                }
            }
            return attachmentlist;
        }

        //Edit TestIssue 
        public JsonResult GetTestIssue(string testIssueID)
        {
            string errorMsg = string.Empty;
            TestIssue testIssue = new TestIssue();
            try
            {
                TestProvider testProvider = new TestProvider();               
                testIssue = testProvider.GetTestIssueByTestIssueID(new Guid(testIssueID));
                List<TestIssueFiles> testIssueFiles = testProvider.GetTestIssueFilesByTestIssueD(new Guid(testIssueID));
                if (testIssueFiles.Count > 0)
                {
                    testIssue.TestIssueFiles.AddRange(testIssueFiles);
                }

                
            }
            catch (Exception ex)
            {
                errorMsg = ex.ToString();
            }

            return Json(testIssue, JsonRequestBehavior.AllowGet);
        }
        #endregion

        
    }
}
