using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace QA_System.Classes
{
    public static class HtmlPager
    {
        /// <summary>  
        /// pagination display 
        /// </summary>   
        /// <param name="html"></param>  
        /// <param name="currentPageStr">QueryStringKey</param>   
        /// <param name="rows">size of page</param>  
        /// <param name="totalCount">total count</param>  
        /// <returns></returns> 
        public static MvcHtmlString Pager(this HtmlHelper html, int rows, object totalCount)
        {
            string currentPageStr = "pageIndex";
            var queryString = html.ViewContext.HttpContext.Request.QueryString;
            int currentPage = 1;  
            var totalPages = Math.Max(((int)totalCount + rows - 1) / rows, 1); 
            var dict = new System.Web.Routing.RouteValueDictionary(html.ViewContext.RouteData.Values);
            var output = new StringBuilder();
            if (!string.IsNullOrEmpty(queryString[currentPageStr]))
            {
                //QueryString 
                foreach (string key in queryString.Keys)
                    if (queryString[key] != null && !string.IsNullOrEmpty(key))
                        dict[key] = queryString[key];
                int.TryParse(queryString[currentPageStr], out currentPage);
            }
            else
            {
                // ～/Page/{page number} 
                if (dict.ContainsKey(currentPageStr))
                    int.TryParse(dict[currentPageStr].ToString(), out currentPage);
                
            }

            //save search value to next page
            foreach (string key in queryString.Keys)
                dict[key] = queryString[key];            
            //var formValue = html.ViewContext.HttpContext.Request.Form;
            //foreach (string key in formValue.Keys)
            //    if (formValue[key] != null && !string.IsNullOrEmpty(key))
            //        dict[key] = formValue[key]; 
 
            if (currentPage <= 0) currentPage = 1;
            if (totalPages > 1)
            {
                if (currentPage != 1)
                {
                    //First link  
                    dict[currentPageStr] = 1;
                    output.AppendFormat("{0} ", html.RouteLink("<< First", dict));
                }
                if (currentPage > 1)
                {
                    //Preview link
                    dict[currentPageStr] = currentPage - 1;
                    output.Append(html.RouteLink("< Prev", dict));
                }                
                output.Append(" ");
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                        if (currint == i)
                        {
                            output.Append(string.Format("[{0}]", currentPage));
                        }
                        else
                        {
                            dict[currentPageStr] = currentPage + i - currint;
                            output.Append(html.RouteLink((currentPage + i - currint).ToString(), dict));
                        }
                    output.Append(" ");
                }
                if (currentPage < totalPages)
                {
                    dict[currentPageStr] = currentPage + 1;
                    output.Append(html.RouteLink("Next >", dict));
                }                
                output.Append(" ");
                if (currentPage != totalPages)
                {
                    dict[currentPageStr] = totalPages;
                    output.Append(html.RouteLink("Last >>", dict));
                }
                output.Append(" ");
            }
            output.AppendFormat("<b> Page {0} of {1} Total {2} </b>", currentPage, totalPages, totalCount);
            return new MvcHtmlString(output.ToString());
        }
    }

    public class PagerInfo
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int PageTotalCount { get; set; }
        public string PageSortExpress { get; set; }
        public string PageSortDirection { get; set; }
    }

    public class PageQuery<TPager, TEntityList>
    {
        public TPager Pager { get; set; }
        public TEntityList EntityList { get; set; }

        public PageQuery(TPager pager, TEntityList entityList)
        {
            this.Pager = pager;
            this.EntityList = entityList;
        }       
    }
}
