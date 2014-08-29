using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using QA_System.Models;

namespace QA_System.DataProvider
{
    public class DataBase
    {
        private string _sqlConnString = string.Empty;
        public string sqlConnString
        {
            get
            {
                SqlConnectionStringBuilder connectStringBuilder = new SqlConnectionStringBuilder();
                connectStringBuilder.DataSource = @"10.10.73.214";
                connectStringBuilder.InitialCatalog = "TeamManage";
                connectStringBuilder.UserID = "SA";
                connectStringBuilder.Password = "encompass-sh";
                connectStringBuilder.IntegratedSecurity = false;
                return connectStringBuilder.ConnectionString;
            }
        }


        #region Test Request
        public void CreateRequestTest(QA_System.Models.RequestTestModels requestTest)
        { 
            using(SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_CreateReqestTest";

                    cmd.Parameters.AddWithValue("@requestID", requestTest.ReqestID);
                    cmd.Parameters.AddWithValue("@project", requestTest.Project);
                    cmd.Parameters.AddWithValue("@RelatedJIRA", requestTest.RelatedJIRA);
                    cmd.Parameters.AddWithValue("@developer", requestTest.Developer);
                    cmd.Parameters.AddWithValue("@instruction", requestTest.Instruction);
                    cmd.Parameters.AddWithValue("@comments", requestTest.Comments);
                    //Other Fields
                    string otherFieldsXml = string.Empty;
                    if (requestTest.OtherFields != null)
                    {
                        otherFieldsXml = requestTest.OtherFields.GetXML();
                    }
                    cmd.Parameters.AddWithValue("@fields", otherFieldsXml);

                    cmd.ExecuteNonQuery();
                    
                }
            }
        }

        public void UpdateRequestTest(QA_System.Models.RequestTestModels requestTest)
        {
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_UpdateReqestTest";

                    cmd.Parameters.AddWithValue("@requestID", requestTest.ReqestID);
                    cmd.Parameters.AddWithValue("@project", requestTest.Project);
                    cmd.Parameters.AddWithValue("@RelatedJIRA", requestTest.RelatedJIRA);
                    cmd.Parameters.AddWithValue("@developer", requestTest.Developer);
                    cmd.Parameters.AddWithValue("@instruction", requestTest.Instruction);
                    cmd.Parameters.AddWithValue("@comments", requestTest.Comments);
                    //Other Fields
                    string otherFieldsXml = string.Empty;
                    if (requestTest.OtherFields != null)
                    {
                        otherFieldsXml = requestTest.OtherFields.GetXML();
                    }
                    cmd.Parameters.AddWithValue("@fields", otherFieldsXml);

                    cmd.ExecuteNonQuery();

                }
            }
        }

        public void CreateRequestTestFile(RequestTestModels requestTest)
        {
            if (requestTest.RequestTestFiles.Count == 0) return;
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_CreateTestReqestFile";
                    cmd.Parameters.AddWithValue("@RequestID", requestTest.ReqestID);
                    cmd.Parameters.Add("@FileID", SqlDbType.UniqueIdentifier);
                    foreach (RequestTestFiles requestTestFile in requestTest.RequestTestFiles)
                    {
                        cmd.Parameters["@FileID"].Value = requestTestFile.RequestTestFileID;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        

        public List<RequestTestModels> GetRequestTest(DateTime? fromDate, DateTime? toDate, string status, string developerID, string testerID)
        {
            List<RequestTestModels> testRequestList = new List<RequestTestModels>();
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_GetRequestTest";

                    cmd.Parameters.AddWithValue("@fromDate", fromDate);
                    cmd.Parameters.AddWithValue("@toDate", toDate);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@developer", developerID);
                    cmd.Parameters.AddWithValue("@tester", testerID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        RequestTestModels testRequest = new RequestTestModels();
                        testRequest.ReqestID = new Guid(reader[0].ToString());
                        testRequest.Project = reader[1].ToString();
                        testRequest.RelatedJIRA = reader[2].ToString();
                        testRequest.Developer = reader[3].ToString();
                        testRequest.Instruction = reader[4].ToString();
                        testRequest.Comments = reader[5].ToString();
                        if (!string.IsNullOrEmpty(reader[6].ToString()))
                        {
                            testRequest.OtherFields.InitReqestOtherFields(reader[6].ToString());
                        }
                        testRequest.InsertDateTime = reader.GetDateTime(7);
                        if (!reader.IsDBNull(8))
                        {
                            testRequest.UpdateTime = reader.GetDateTime(8);
                        }
                        testRequest.DeveloperName = reader[16].ToString();
                        //Test info
                        testRequest.TestModel.TestReportID = new Guid(reader[9].ToString());
                        if (!reader.IsDBNull(10))
                            testRequest.TestModel.Result = (Test.Results)reader.GetInt32(10);
                        if (!reader.IsDBNull(11))
                            testRequest.TestModel.Status = (Test.Types)reader.GetInt32(11);
                        if (!reader.IsDBNull(12))
                            testRequest.TestModel.CodeEvaluate = (Test.CodeEvaluateTypes)reader.GetInt32(12);
                        testRequest.TestModel.Tester = reader[13].ToString();
                        //Test Comments
                        //testRequest.TestModel.Comments = reader[14].ToString();
                        if (!reader.IsDBNull(14))
                        {
                            
                            testRequest.TestModel.InitTestCommentsFromXmlStr(reader[14].ToString());

                        }
                        testRequest.TestModel.UpdateTime = reader.GetDateTime(15);
                        testRequest.TestModel.TesterName = reader[17].ToString();
                        //Get Request File
                        List<RequestTestFiles> requestTestFiles = GetRequestTestFile(testRequest.ReqestID);
                        if (requestTestFiles.Count > 0)
                        {
                            testRequest.RequestTestFiles.AddRange(requestTestFiles);
                        }
                        //Get Test File

                        testRequestList.Add(testRequest);
                    }
                }
            }
            return testRequestList;
        }

        public RequestTestModels GetRequestTestByRequestID(Guid requestID)
        {
            RequestTestModels requestTest = new RequestTestModels();
            using (SqlConnection cnn = new SqlConnection(this.sqlConnString))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "QA_GetRequestTestByRequestID";
                cmd.Parameters.AddWithValue("@RequestID",requestID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    requestTest.ReqestID = new Guid(reader[0].ToString());
                    requestTest.Project = reader[1].ToString();
                    requestTest.RelatedJIRA = reader[2].ToString();
                    requestTest.Developer = reader[3].ToString();
                    requestTest.Instruction = reader[4].ToString();
                    requestTest.Comments = reader[5].ToString();
                    if (!string.IsNullOrEmpty(reader[6].ToString()))
                    {
                        requestTest.OtherFields.InitReqestOtherFields(reader[6].ToString());
                    }
                    requestTest.UpdateTime = reader.GetDateTime(7);
                    requestTest.DeveloperName = reader[16].ToString();
                    //Test
                    requestTest.TestModel.TestReportID = new Guid(reader[8].ToString());
                    requestTest.TestModel.RequestID = new Guid(reader[9].ToString());
                    if (!reader.IsDBNull(10))
                        requestTest.TestModel.Result = (Test.Results)reader.GetInt32(10);
                    if (!reader.IsDBNull(11))
                        requestTest.TestModel.Status = (Test.Types)reader.GetInt32(11);
                    if (!reader.IsDBNull(12))
                        requestTest.TestModel.CodeEvaluate = (Test.CodeEvaluateTypes)reader.GetInt32(12);
                    requestTest.TestModel.Tester = reader[13].ToString();
                    //Test Comments
                    //requestTest.TestModel.Comments = reader[14].ToString();
                    if (!reader.IsDBNull(14))
                    {
                        requestTest.TestModel.InitTestCommentsFromXmlStr(reader[14].ToString());
                    }
                    requestTest.TestModel.UpdateTime = reader.GetDateTime(15);
                    requestTest.TestModel.TesterName = reader[17].ToString();

                    //TestIssue
                    List<TestIssue> testIssueList = GetTestIssueByTestReportID(requestTest.TestModel.TestReportID);
                    if (testIssueList.Count > 0)
                    {
                        requestTest.TestModel.TestIssues.AddRange(testIssueList);
                    }                                      
                    
                    //Set file for Request and TestIssue
                    SetRequestAndTestIssueFile(ref requestTest);
                                                           
                }
            }
            return requestTest;
        }
        
        /// <summary>
        /// Get Test Issue
        /// </summary>
        /// <param name="testReportID"></param>
        /// <returns></returns>
        public List<TestIssue> GetTestIssueByTestReportID(Guid testReportID)
        {
            List<TestIssue> testIssueList = new List<TestIssue>();
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "QA_GetTestIssueByTestReportID";
                cmd.Parameters.AddWithValue("@TestReportID", testReportID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TestIssue testIssue = new TestIssue();
                    testIssue.TestReportID = testReportID;
                    testIssue.TestIssueID = reader.GetGuid(0);
                    testIssue.Description = reader[2].ToString();
                    testIssue.UpdateTime = reader.GetDateTime(3);
                    testIssueList.Add(testIssue);
                }
            }
            return testIssueList;
        }

        /// <summary>
        /// Get Test Issue File
        /// </summary>
        /// <param name="testReportID"></param>
        /// <returns></returns>
        public List<TestIssueFiles> GetTestIssueFilesByTestReportID(Guid testReportID)
        {
            List<TestIssueFiles> testIssueFileList = new List<TestIssueFiles>();
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "QA_GetTestIssueFileByTestReportID";
                cmd.Parameters.AddWithValue("@TestReportID", testReportID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TestIssueFiles testIssueFile = new TestIssueFiles();
                    testIssueFile.TestIssueID = reader.GetGuid(0);
                    testIssueFile.TestIssueFileID = reader.GetGuid(1);
                    testIssueFile.TestIssueFileName = reader[2].ToString();
                    testIssueFile.TestIssueFileExt = reader[3].ToString();
                    testIssueFileList.Add(testIssueFile);
                }
            }
            return testIssueFileList;
        }

        /// <summary>
        /// Get Request File
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public List<RequestTestFiles> GetRequestTestFile(Guid requestID)
        {
            List<RequestTestFiles> requestTestFilesList = new List<RequestTestFiles>();
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "QA_GetRequestFile";
                cmd.Parameters.AddWithValue("@RequestID", requestID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RequestTestFiles requestTestFile = new RequestTestFiles();
                    requestTestFile.RequestTestFileID = reader.GetGuid(0);
                    requestTestFile.RequestTestFileName = reader[1].ToString();
                    requestTestFile.RequestTestFileExt = reader[2].ToString();
                    requestTestFilesList.Add(requestTestFile);
                }
            }
            return requestTestFilesList;
        }

        /// <summary>
        /// Set Request File and TestIssue File
        /// </summary>
        /// <param name="requestTest"></param>
        public void SetRequestAndTestIssueFile(ref RequestTestModels requestTest)
        {

            List<RequestTestFiles> requestTestFilesList = new List<RequestTestFiles>();
            List<TestIssueFiles> testIssueFilesList = new List<TestIssueFiles>();

            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "QA_GetRequestAndTestIssueFile";
                cmd.Parameters.AddWithValue("@RequestID", requestTest.ReqestID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[3].ToString() == "Request")
                    {
                        RequestTestFiles requestTestFile = new RequestTestFiles();
                        requestTestFile.RequestTestFileID = reader.GetGuid(0);
                        requestTestFile.RequestTestFileName = reader[1].ToString();
                        requestTestFile.RequestTestFileExt = reader[2].ToString();
                        requestTest.RequestTestFiles.Add(requestTestFile);
                    }
                    else if (reader[3].ToString() == "TestIssue")
                    {
                        TestIssueFiles testIssueFile = new TestIssueFiles();
                        testIssueFile.TestIssueFileID = reader.GetGuid(0);
                        testIssueFile.TestIssueFileName = reader[1].ToString();
                        testIssueFile.TestIssueFileExt = reader[2].ToString();
                        //Test Issue File
                        if (!reader.IsDBNull(4))
                        {
                            testIssueFile.TestIssueID = reader.GetGuid(4);
                        }
                        testIssueFilesList.Add(testIssueFile);
                    }
                }
            }
            //Test Issue File
            if (testIssueFilesList.Count > 0)
            {
                foreach (TestIssueFiles files in testIssueFilesList)
                {
                    foreach (TestIssue testIssue in requestTest.TestModel.TestIssues)
                    {
                        if (testIssue.TestIssueID == files.TestIssueID)
                        {
                            testIssue.TestIssueFiles.Add(files);
                        }
                    }
                }
            }
        }

        public int GetRequestUnResolveNum(string developer, int testResult)
        {
            int num = 0;
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_StaticRequestUnResolve";

                    cmd.Parameters.AddWithValue("@developer", developer);
                    cmd.Parameters.AddWithValue("@testResult", testResult);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        num = reader.GetInt32(0);
                    }
                }
            }
            return num;
        }
       
        #endregion

        #region UploadFile
        public void createFile(Guid FileID, string filename, string fileExt)
        {
            using (SqlConnection connection = new SqlConnection(sqlConnString))
            {
                connection.Open();
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = connection;

                    comm.CommandText = "QA_CreateFile";
                    comm.CommandType = CommandType.StoredProcedure;

                    comm.Parameters.AddWithValue("@FileID", FileID);
                    comm.Parameters.AddWithValue("@Name", filename);
                    comm.Parameters.AddWithValue("@Ext", fileExt);

                    comm.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTestRequestFile(Guid fileID)
        {
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_DeleteFile";
                    cmd.Parameters.AddWithValue("@FileID", fileID);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
}