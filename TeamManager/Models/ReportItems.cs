using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using TeamManager.Models;

namespace TeamManager.Models
{
    public class ReportItems
    {
        public List<ReportItem> Finished { get; set; }
        public List<ReportItem> WIP { get; set; }
        public List<ReportItem> Planning { get; set; }
        public List<ReportItem> Blocking { get; set; }
        //public List<ReportFile> Files { get; set; }

        public ReportItems()
        {
            this.Finished = new List<ReportItem>();
            this.WIP = new List<ReportItem>();
            this.Planning = new List<ReportItem>();
            this.Blocking = new List<ReportItem>();
            //this.Files = new List<ReportFile>();
        }

        public string ToXML()
        {
            string result = "<Items>";

            result += "<Finished>";
            foreach (ReportItem item in this.Finished)
            {
                result += item.ToXML();
            }
            result += "</Finished>";

            result += "<WIP>";
            foreach (ReportItem item in this.WIP)
            {
                result += item.ToXML();
            }
            result += "</WIP>";

            result += "<Planning>";
            foreach (ReportItem item in this.Planning)
            {
                result += item.ToXML();
            }
            result += "</Planning>";

            result += "<Blocking>";
            foreach (ReportItem item in this.Blocking)
            {
                result += item.ToXML();
            }
            result += "</Blocking>";

            //result += "<Files>";
            //foreach (ReportFile item in this.Files)
            //{
            //    result += item.ToXML();
            //}
            //result += "</Files>";

            result += "</Items>";
            return result;
        }

        public void FromXML(string xml)
        {
            XmlDocument root = new XmlDocument();
            root.LoadXml(xml);
            XmlNode finished = root.DocumentElement.SelectSingleNode("Finished");
            foreach (XmlNode node in finished.ChildNodes)
            {
                ReportItem item = new ReportItem();
                item.FromXML(node);
                this.Finished.Add(item);
            }
            XmlNode wip = root.DocumentElement.SelectSingleNode("WIP");
            foreach (XmlNode node in wip.ChildNodes)
            {
                ReportItem item = new ReportItem();
                item.FromXML(node);
                this.WIP.Add(item);
            }
            XmlNode planning = root.DocumentElement.SelectSingleNode("Planning");
            if (planning != null)
            {
                foreach (XmlNode node in planning.ChildNodes)
                {
                    ReportItem item = new ReportItem();
                    item.FromXML(node);
                    this.Planning.Add(item);
                }
            }
            XmlNode blocking = root.DocumentElement.SelectSingleNode("Blocking");
            if (blocking != null)
            {
                foreach (XmlNode node in blocking.ChildNodes)
                {
                    ReportItem item = new ReportItem();
                    item.FromXML(node);
                    this.Blocking.Add(item);
                }
            }
            //XmlNode files = root.DocumentElement.SelectSingleNode("Files");
            //foreach (XmlNode node in files.ChildNodes)
            //{
            //    ReportFile item = new ReportFile();
            //    item.FromXML(node);
            //    this.Files.Add(item);
            //}
        }
    }
    public class ReportItem
    {
        public string Work { get; set; }
        public string Description { get; set; }
        public string Project { get; set; }

        public string ToXML()
        {
            string result = "<Item>";
            result += "<work><![CDATA[" + this.Work + "]]></work>";
            result += "<description><![CDATA[" + this.Description + "]]></description>";
            result += "<project><![CDATA[" + this.Project + "]]></project>";
            result += "</Item>";
            return result;
        }

        public void FromXML(XmlNode node)
        {
            XmlNode workNode = node.SelectSingleNode("work");
            this.Work = workNode.InnerText;
            XmlNode descNode = node.SelectSingleNode("description");
            this.Description = descNode.InnerText;
            XmlNode projectNode = node.SelectSingleNode("project");
            this.Project = projectNode.InnerText;
        }
    }
}