using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace TeamManager.Models
{
    public class ReportFiles
    {
        public List<ReportFile> Files { get; set; }

        public string Status { get; set; }
        public string Project { get; set; }

        public ReportFiles()
        {
            this.Files = new List<ReportFile>();
        }

        public string ToXML()
        {
            string result = "<Item>";
            result += "<Project><![CDATA[" + this.Project + "]]></Project>";
            result += "<Status><![CDATA[" + this.Status + "]]></Status>";
            result += "<Files>";
            foreach (ReportFile item in this.Files)
            {
                result += item.ToXML();
            }
            result += "</Files>";
            result += "</Item>";
            return result;
        }

        public void FromXML(string xml)
        {
            XmlDocument root = new XmlDocument();
            root.LoadXml(xml);

            XmlNode projectNode = root.DocumentElement.SelectSingleNode("Project");
            this.Project = projectNode.InnerText;
            XmlNode statusNode = root.DocumentElement.SelectSingleNode("Status");
            this.Status = statusNode.InnerText;
            XmlNode files = root.DocumentElement.SelectSingleNode("Files");
            foreach (XmlNode node in files.ChildNodes)
            {
                ReportFile item = new ReportFile();
                item.FromXML(node);
                this.Files.Add(item);
            }
        }

    //    public void AddFilesXML(string files)
    //    {
    //        XmlDocument root = new XmlDocument();
    //        root.LoadXml(files);
    //        XmlNode fileNode = root.DocumentElement;
    //        foreach (XmlNode node in fileNode.ChildNodes)
    //        {
    //            ReportFile item = new ReportFile();
    //            item.FromXML(node);
    //            this.Files.Add(item);
    //        }
    //    }

    //    public string GetFilesXML()
    //    {
    //        string result = "<Files>";
    //        foreach (ReportFile file in this.Files)
    //        {
    //            result += file.ToXML();
    //        }
    //        result += "</Files>";
    //        return result;
    //    }
    }
    public class ReportFile
    {
        public string Guid { get; set; }
        public string FileName { get; set; }

        public string ToXML()
        {
            string result = "<OneFile>";
            result += "<FileGuid><![CDATA[" + this.Guid + "]]></FileGuid>";
            result += "<FileName><![CDATA[" + this.FileName + "]]></FileName>";
            result += "</OneFile>";
            return result;
        }

        public void FromXML(XmlNode node)
        {
            XmlNode guidNode = node.SelectSingleNode("FileGuid");
            this.Guid = guidNode.InnerText;
            XmlNode nameNode = node.SelectSingleNode("FileName");
            this.FileName = nameNode.InnerText;
        }
    }
}