using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;

namespace QA_System.Models
{
    public class RequestTestModels
    {
        # region attribute
        private Guid _reqestID;
        public Guid ReqestID
        {
            get { return _reqestID; }
            set { _reqestID = value; }
        }

        private string _project;
        public string Project
        {
            get { return _project; }
            set { _project = value; }
        }

        private string _relatedJIRA;
        public string RelatedJIRA
        {
            get { return _relatedJIRA; }
            set { _relatedJIRA = value; }
        }

        private string _developer;
        public string Developer
        {
            get { return _developer; }
            set { _developer = value; }
        }
        private string _developerName;
        public string DeveloperName
        {
            get { return _developerName; }
            set { _developerName = value; }
        }

        private string _instruction;
        public string Instruction
        {
            get { return _instruction; }
            set { _instruction = value; }
        }

        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        private DateTime _insertDateTime;
        public DateTime InsertDateTime
        {
            get { return _insertDateTime; }
            set { _insertDateTime = value; }
        }

        private DateTime _updateTime = DateTime.MinValue;
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }

        private List<RequestTestFiles> _requestTestFiles = new List<RequestTestFiles>();

        public List<RequestTestFiles> RequestTestFiles
        {
            get { return _requestTestFiles; }
            set { _requestTestFiles = value; }
        }

        private TestRequestOtherFields _otherFields = new TestRequestOtherFields();

        public TestRequestOtherFields OtherFields
        {
            get { return _otherFields; }
            set { _otherFields = value; }
        }

        private Test _testModel = new Test();

        public Test TestModel
        {
            get { return _testModel; }
            set { _testModel = value; }
        }

        #endregion

        public RequestTestModels()
        { }
    }

    public class RequestTestFiles
    {
        private Guid _requestTestFileID;

        public Guid RequestTestFileID
        {
            get { return _requestTestFileID; }
            set { _requestTestFileID = value; }
        }
        private string _requestTestFileName;

        public string RequestTestFileName
        {
            get { return _requestTestFileName; }
            set { _requestTestFileName = value; }
        }
        private string _requestTestFileExt;

        public string RequestTestFileExt
        {
            get { return _requestTestFileExt; }
            set { _requestTestFileExt = value; }
        }         
    }

    public class TestRequestOtherFields
    {
        # region attribute
        private string _environment;

        public string Environment
        {
            get { return _environment; }
            set { _environment = value; }
        }

        #endregion

        public string GetXML()
        {
            StringBuilder xmlStringBuilder = new StringBuilder("<Fields>");

            xmlStringBuilder.Append("<Field name=\"Environment\"><![CDATA[");
            xmlStringBuilder.Append(this.Environment);
            xmlStringBuilder.Append("]]></Field>");

            xmlStringBuilder.Append("</Fields>");
            return xmlStringBuilder.ToString();
        }

        public bool InitReqestOtherFields(string xmlString)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
                XmlNode rootNode = xmlDocument.SelectSingleNode(@"/Fields");

                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    string name = node.Attributes["name"].Value;
                    switch (name)
                    {
                        case "Environment":
                            this._environment = node.InnerText;
                            break;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}