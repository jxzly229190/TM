using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QA_System.Models
{
    public class TestIssue
    {       
        #region property
        private Guid _testIssueID;

        public Guid TestIssueID
        {
            get { return _testIssueID; }
            set { _testIssueID = value; }
        }

        private Guid _testReportID;

        public Guid TestReportID
        {
            get { return _testReportID; }
            set { _testReportID = value; }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private DateTime _updateTime;

        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }

        private List<TestIssueFiles> _testIssueFiles = new List<TestIssueFiles>();

        public List<TestIssueFiles> TestIssueFiles
        {
            get { return _testIssueFiles; }
            set { _testIssueFiles = value; }
        }

       
        #endregion

    }

    public class TestIssueFiles
    {
        private Guid _testIssueID;

        public Guid TestIssueID
        {
            get { return _testIssueID; }
            set { _testIssueID = value; }
        }

        private Guid _testIssueFileID;

        public Guid TestIssueFileID
        {
            get { return _testIssueFileID; }
            set { _testIssueFileID = value; }
        }

        private string _testIssueFileName;

        public string TestIssueFileName
        {
            get { return _testIssueFileName; }
            set { _testIssueFileName = value; }
        }

        private string _testIssueFileExt;

        public string TestIssueFileExt
        {
            get { return _testIssueFileExt; }
            set { _testIssueFileExt = value; }
        }

       
    }
}