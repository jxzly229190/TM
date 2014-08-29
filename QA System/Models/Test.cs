using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace QA_System.Models
{
    public class Test
    {
        public enum Types
        {
            [Display(Name = "Not Started")]
            NotStarted = 0,
            [Display(Name = "In Progress")]
            InProgress = 1,
            [Display(Name = "Found Issue")]
            FoundIssue = 2,
            [Display(Name = "Tested")]
            Tested = 3
        }

        public enum CodeEvaluateTypes
        {
            [Display(Name = " ")]
            None,
            [Display(Name = "Terrible")]
            Terrible,
            [Display(Name = "Bad")]
            Bad,
            [Display(Name = "Normal")]
            Normal,
            [Display(Name = "Good")]
            Good,
            [Display(Name = "Very Good")]
            VeryGood
        }

        public enum Results
        {
            [Display(Name = " ")]
            None = 0,
            [Display(Name = "Pass")]
            Pass = 1,
            [Display(Name = "Not Pass")]
            NotPass = 2
        }

        #region property
        private Guid _testReportID;

        public Guid TestReportID
        {
            get { return _testReportID; }
            set { _testReportID = value; }
        }

        private Guid _requestID;

        public Guid RequestID
        {
            get { return _requestID; }
            set { _requestID = value; }
        }
        private Results _result;

        public  Results Result
        {
            get { return _result; }
            set { _result = value; }
        }
        private Types _status;

        public Types Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private CodeEvaluateTypes _codeEvaluate;

        public CodeEvaluateTypes CodeEvaluate
        {
            get { return _codeEvaluate; }
            set { _codeEvaluate = value; }
        }
        private string _tester;

        public string Tester
        {
            get { return _tester; }
            set { _tester = value; }
        }
        private string _testerName;

        public string TesterName
        {
            get { return _testerName; }
            set { _testerName = value; }
        }
        private List<TestComments> _comments = new List<TestComments>();

        public List<TestComments> Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }
        private DateTime _insertTime;

        public DateTime InsertTime
        {
            get { return _insertTime; }
            set { _insertTime = value; }
        }
        private DateTime _updateTime;

        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }

        private List<TestIssue> _testIssues = new List<TestIssue>();

        public List<TestIssue> TestIssues
        {
            get { return _testIssues; }
            set { _testIssues = value; }
        }

        public void InitTestCommentsFromXmlStr(string xmlStr)
        {
            
            XElement xElement = XElement.Parse(xmlStr);
            var testComments = from com in xElement.Descendants("comment")
                               select com;
            foreach (var comm in testComments)
            { 
                string date = comm.Attribute("date").Value;
                DateTime addDate = DateTime.MinValue;
                DateTime.TryParse(date, out addDate);
                string addUser = comm.Attribute("user").Value;
                string comment = comm.Value;
                TestComments tc = new TestComments(addDate, addUser, comment);
                this._comments.Add(tc);
            }
        }

        #endregion
    }

    // TestComments
    public class TestComments
    {
        private DateTime _addDate;

        public DateTime AddDate
        {
            get { return _addDate; }
            set { _addDate = value; }
        }

        private string _addUser;

        public string AddUser
        {
            get { return _addUser; }
            set { _addUser = value; }
        }

        private string _comments;

        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        public TestComments()
        { }

        public TestComments(DateTime addDate, string addUser, string comments)
        {
            this._addDate = addDate;
            this._addUser = addUser;
            this._comments = comments;
        }
    }
}