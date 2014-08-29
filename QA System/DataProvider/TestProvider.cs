using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using QA_System.Models;

namespace QA_System.DataProvider
{
    public class TestProvider : DataBase
    {
        public void CreateTest(Test test)
        {            
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_CreateTest";
                    
                    cmd.Parameters.AddWithValue("@TestReportID", test.TestReportID);
                    cmd.Parameters.AddWithValue("@RequestID", test.RequestID);
                    cmd.Parameters.AddWithValue("@Status", test.Status);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
            }
        }

        public List<RequestTestModels> GetRequestByUnAssign()
        {
            List<RequestTestModels> testRequestList = new List<RequestTestModels>();
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_GetRequestTestUnResolve";
                    
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
                        if (!reader.IsDBNull(7))
                        {
                            testRequest.UpdateTime = reader.GetDateTime(7);
                        }
                        //Get Request File
                        List<RequestTestFiles> requestTestFiles = GetRequestTestFile(testRequest.ReqestID);
                        if (requestTestFiles.Count > 0)
                        {
                            testRequest.RequestTestFiles.AddRange(requestTestFiles);
                        }

                        testRequestList.Add(testRequest);
                    }
                }
            }
            return testRequestList;
        }

        public void SaveAssign(Guid requestID, string assignTo)
        {
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_SaveTestAssign";

                    cmd.Parameters.AddWithValue("@RequestID", requestID);
                    cmd.Parameters.AddWithValue("@Tester", assignTo);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
                connection.Close();
            }
        }

        public void SaveTestStatus(Guid testReportID, QA_System.Models.Test.Types status)
        {
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_UpdateTestStatus";

                    cmd.Parameters.AddWithValue("@TestReportID", testReportID);
                    cmd.Parameters.AddWithValue("@Status", status);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
                connection.Close();
            }
        }

        public void SaveTestResult(Guid testReportID, QA_System.Models.Test.Results result)
        {
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_UpdateTestResult";

                    cmd.Parameters.AddWithValue("@TestReportID", testReportID);
                    cmd.Parameters.AddWithValue("@Result", result);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
                connection.Close();
            }
        }

        public void SaveTestCodeEvaluate(Guid testReportID, QA_System.Models.Test.CodeEvaluateTypes codeEvaluate)
        {
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_UpdateTestCodeEvaluate";

                    cmd.Parameters.AddWithValue("@TestReportID", testReportID);
                    cmd.Parameters.AddWithValue("@CodeEvaluate", codeEvaluate);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
                connection.Close();
            }
        }

        public void SaveTestComments(Guid testReportID, string commentXml)
        {
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_UpdateTestComments";

                    cmd.Parameters.AddWithValue("@TestReportID", testReportID);
                    cmd.Parameters.AddWithValue("@TestComments", commentXml);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
                connection.Close();
            }
        }

        public int GetTestUnResolveNum(string tester, int testResult)
        {
            int num = 0;
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_StaticTestUnResolve";

                    cmd.Parameters.AddWithValue("@tester", tester);
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

        #region Test Issue
        public void CreateTestIssue(TestIssue testIssue)
        {
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_CreateTestIssue";

                    cmd.Parameters.AddWithValue("@TestIssueID", testIssue.TestIssueID);
                    cmd.Parameters.AddWithValue("@TestReportID", testIssue.TestReportID);
                    cmd.Parameters.AddWithValue("@Description", testIssue.Description);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
                connection.Close();
            }
        }

        public void CreateTestIssueFile(TestIssue testIssue)
        {
            if (testIssue.TestIssueFiles.Count == 0) return;
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_CreateTestIssueFile";
                    cmd.Parameters.AddWithValue("@TestIssueID", testIssue.TestIssueID);
                    cmd.Parameters.Add("@FileID", SqlDbType.UniqueIdentifier);
                    foreach (TestIssueFiles testIssueFile in testIssue.TestIssueFiles)
                    {
                        cmd.Parameters["@FileID"].Value = testIssueFile.TestIssueFileID;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UpdateTestIssue(TestIssue testIssue)
        {
            using (SqlConnection connection = new SqlConnection(base.sqlConnString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "QA_UpdateTestIssue";

                    cmd.Parameters.AddWithValue("@TestIssueID", testIssue.TestIssueID);
                    cmd.Parameters.AddWithValue("@Description", testIssue.Description);

                    SqlDataReader reader = cmd.ExecuteReader();
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Get Test Issue
        /// </summary>
        /// <param name="testReportID"></param>
        /// <returns></returns>
        public TestIssue GetTestIssueByTestIssueID(Guid testIssueID)
        {
            TestIssue testIssue = new TestIssue();
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "QA_GetTestIssueByTestIssueID";
                cmd.Parameters.AddWithValue("@TestIssueID", testIssueID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    testIssue.TestReportID = reader.GetGuid(1);
                    testIssue.TestIssueID = reader.GetGuid(0);
                    testIssue.Description = reader[2].ToString();
                    testIssue.UpdateTime = reader.GetDateTime(3);
                }
            }
            return testIssue;
        }

        /// <summary>
        /// Get Test Issue File By TestIssueID
        /// </summary>
        /// <param name="testIssueID"></param>
        /// <returns></returns>
        public List<TestIssueFiles> GetTestIssueFilesByTestIssueD(Guid testIssueID)
        {
            List<TestIssueFiles> testIssueFileList = new List<TestIssueFiles>();
            using (SqlConnection connection = new SqlConnection(this.sqlConnString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "QA_GetTestIssueFileByTestIssueID";
                cmd.Parameters.AddWithValue("@TestIssueID", testIssueID);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TestIssueFiles testIssueFile = new TestIssueFiles();
                    testIssueFile.TestIssueFileID = reader.GetGuid(0);
                    testIssueFile.TestIssueFileName = reader[1].ToString();
                    testIssueFile.TestIssueFileExt = reader[2].ToString();
                    testIssueFileList.Add(testIssueFile);
                }
            }
            return testIssueFileList;
        }
        #endregion
    }
}