using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TeamManager.Models;

namespace TeamManager.Classes
{
    public class ReportFunction
    {
        /// <summary>
        /// Classify one report with the groupname
        /// </summary>
        /// <param name="projectName">one of groupnames</param>
        /// <param name="items">an object correspond to a report </param>
        /// <param name="tempStr">a string</param>
        public void ReportToGroup(string projectName, ReportItems items, StringBuilder tempStr, int flag)
        {
            StringBuilder temp = new StringBuilder();
            int count = 1;
            int marches = 0;
            if (items.Finished.Count > 0)
            {
                temp.Append("Finished:<br/>");
                foreach (ReportItem finished in items.Finished)
                {
                    if (finished.Project == projectName)
                    {
                        temp.Append("&nbsp;&nbsp;" + count++ + ". " + finished.Work + "<br/>");
                        if (finished.Description != "")
                        {
                            string str = finished.Description.Replace("\n", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                            temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + str + "<br/>");
                        }
                        marches++;
                    }
                }
            }
            if (marches > 0)
            {
                tempStr.Append(temp);
            }

            count = 1;
            marches = 0;
            temp = new StringBuilder();
            if (items.WIP.Count > 0)
            {
                temp.Append("WIP:<br/>");
                foreach (ReportItem wip in items.WIP)
                {
                    if (wip.Project == projectName)
                    {
                        temp.Append("&nbsp;&nbsp;" + count++ + ". " + wip.Work + "<br/>");
                        if (wip.Description != "")
                        {
                            string str = wip.Description.Replace("\n", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                            temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + str + "<br/>");
                        }
                        marches++;
                    }
                }
            }
            if (marches > 0)
            {
                tempStr.Append(temp);
            }
            //build Planning
            count = 1;
            marches = 0;
            temp = new StringBuilder();
            if (items.Planning.Count > 0)
            {
                temp.Append("Planning:<br/>");
                foreach (ReportItem planning in items.Planning)
                {
                    if (planning.Project == projectName)
                    {
                        temp.Append("&nbsp;&nbsp;" + count++ + ". " + planning.Work + "<br/>");
                        if (planning.Description != "")
                        {
                            string str = planning.Description.Replace("\n", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                            temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + str + "<br/>");
                        }
                        marches++;
                    }
                }
            }
            if (marches > 0)
            {
                tempStr.Append(temp);
            }
            //build Blocking
            count = 1;
            marches = 0;
            temp = new StringBuilder();
            if (items.Blocking.Count > 0)
            {
                temp.Append("Blocking:<br/>");
                foreach (ReportItem blocking in items.Blocking)
                {
                    if (blocking.Project == projectName)
                    {
                        temp.Append("&nbsp;&nbsp;" + count++ + ". " + blocking.Work + "<br/>");
                        if (blocking.Description != "")
                        {
                            string str = blocking.Description.Replace("\n", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                            temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + str + "<br/>");
                        }
                        marches++;
                    }
                }
            }
            if (marches > 0)
            {
                tempStr.Append(temp);
            }
            if (flag != 0)
            {
                tempStr.Replace("<br/>", "\n");
                tempStr.Replace("&nbsp;", " ");
            }
        }

        /// <summary>
        /// build Finished and WIP
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public string GetWorkedReport(ReportItems items)
        {
            StringBuilder tempStr = new StringBuilder();
            StringBuilder temp = new StringBuilder();
            int marches = 0;
            if (items.Finished.Count > 0)
            {
                foreach (ReportItem item in items.Finished)
                {
                    if (marches == items.Finished.Count - 1 && items.WIP.Count==0)
                    {
                        temp.Append(item.Work);
                    }
                    else
                        temp.Append(item.Work + "\n"); 
                    
                    marches++;
                }
            }
            if (marches > 0)
            {
                tempStr.Append(temp);
            }

            marches = 0;
            temp = new StringBuilder();
            if (items.WIP.Count > 0)
            {
                foreach (ReportItem item in items.WIP)
                {
                    if (marches == items.WIP.Count - 1)
                    {
                        temp.Append(item.Work);
                    }
                    else
                        temp.Append(item.Work + "\n");

                    marches++;           
                }
            }
            if (marches > 0)
            {
                tempStr.Append(temp);
            }
            return tempStr.ToString();
        }


        public string GetWorkingReport(ReportItems items)
        {
            StringBuilder temp = new StringBuilder();
            temp = new StringBuilder();
            int marches = 0;
            if (items.Planning.Count > 0)
            {
                foreach (ReportItem item in items.Planning)
                {
                    if (marches == items.Planning.Count - 1)
                    {
                        temp.Append(item.Work);
                    }
                    else
                        temp.Append(item.Work + "\n");

                    marches++;     
                }
            }
            return temp.ToString();
        }


        /// <summary>
        /// build Finished
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public string GetFinishedReport(ReportItems items)
        {
            StringBuilder temp = new StringBuilder();
            int count = 1;
            int marches = 0;
            if (items.Finished.Count > 0)
            {
                //temp.Append("Finished:<br/>");
                foreach (ReportItem item in items.Finished)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Work + "<br/>");
                    if (item.Description != "")
                    {
                        temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Description + "<br/>");
                    }
                    //temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Project + "<br/>");
                    marches++;
                }
            }
            return temp.ToString();
        }

        /// <summary>
        /// build WIP
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public string GetWIPReport(ReportItems items)
        {
            StringBuilder temp = new StringBuilder();
            int count = 1;
            int marches = 0;
            if (items.WIP.Count > 0)
            {
                //temp.Append("WIP:<br/>");
                foreach (ReportItem item in items.WIP)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Work + "<br/>");
                    if (item.Description != "")
                    {
                        temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Description + "<br/>");
                    }
                    //temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Project + "<br/>");
                    marches++;
                }
            }
            return temp.ToString();
        }

        /// <summary>
        /// build Planning
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public string GetPlanningReport(ReportItems items)
        {
            StringBuilder temp = new StringBuilder();
            int count = 1;
            int marches = 0;
            if (items.Planning.Count > 0)
            {
                //temp.Append("Planning:<br/>");
                foreach (ReportItem item in items.Planning)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Work + "<br/>");
                    if (item.Description != "")
                    {
                        temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Description + "<br/>");
                    }
                    temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Project + "<br/>");
                    marches++;
                }
            }
            return temp.ToString();
        }

        /// <summary>
        /// build Blocking
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public string GetBlocking(ReportItems items)
        {
            StringBuilder temp = new StringBuilder();
            int count = 1;
            int marches = 0;
            if (items.Blocking.Count > 0)
            {
                //temp.Append("Blocking:<br/>");
                foreach (ReportItem item in items.Blocking)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Work + "<br/>");
                    if (item.Description != "")
                    {
                        temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Description + "<br/>");
                    }
                    temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Project + "<br/>");
                    marches++;
                }
            }
            return temp.ToString();
        }

        public string GetReport(ReportItems items)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder temp = new StringBuilder();
            //build Finished
            int count = 1;
            int marches = 0;
            if (items.Finished.Count > 0)
            {
                temp.Append("Finished:<br/>");
                foreach (ReportItem item in items.Finished)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Work + "<br/>");
                    if (item.Description != "")
                    {
                        string str = item.Description.Replace("\n", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                        temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + str + "<br/>");
                    }
                    temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Project + "<br/>");
                    marches++;
                }
            }
            if (marches > 0)
            {
                builder.Append(temp);
            }
            //build WIP
            count = 1;
            marches = 0;
            temp = new StringBuilder();
            if (items.WIP.Count > 0)
            {
                temp.Append("WIP:<br/>");
                foreach (ReportItem item in items.WIP)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Work + "<br/>");
                    if (item.Description != "")
                    {
                        string str = item.Description.Replace("\n", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                        temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + str + "<br/>");
                    }
                    temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Project + "<br/>");
                    marches++;
                }
            }
            if (marches > 0)
            {
                builder.Append(temp);
            }
            //build Planning
            count = 1;
            marches = 0;
            temp = new StringBuilder();
            if (items.Planning.Count > 0)
            {
                temp.Append("Planning:<br/>");
                foreach (ReportItem item in items.Planning)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Work + "<br/>");
                    if (item.Description != "")
                    {
                        string str = item.Description.Replace("\n", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                        temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + str + "<br/>");
                    }
                    temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Project + "<br/>");
                    marches++;
                }
            }
            if (marches > 0)
            {
                builder.Append(temp);
            }
            //build Blocking
            count = 1;
            marches = 0;
            temp = new StringBuilder();
            if (items.Blocking.Count > 0)
            {
                temp.Append("Blocking:<br/>");
                foreach (ReportItem item in items.Blocking)
                {
                    temp.Append("&nbsp;&nbsp" + count++ + ". " + item.Work + "<br/>");
                    if (item.Description != "")
                    {
                        string str = item.Description.Replace("\n", "<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                        temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + str + "<br/>");
                    }
                    temp.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + item.Project + "<br/>");
                    marches++;
                }
            }
            if (marches > 0)
            {
                builder.Append(temp);
            }
            return builder.ToString();
        }

        //internal string getReport(ReportItems reportItems)
        //{
        //    throw new NotImplementedException();
        //}
    }
}