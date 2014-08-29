using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamManager.DataProvider;

namespace TeamManager.Models
{
    public class Reports
    {
        public DailyReport DailyReport { get; set; }
        public DailyProjectReport DailyProjectReort { get; set; }

        public Reports()
        {
            this.DailyReport = new DailyReport();
            this.DailyProjectReort = new DailyProjectReport();
        }
    }
}