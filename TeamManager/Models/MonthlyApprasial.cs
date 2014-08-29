using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManager.Models
{
    public class MonthlyApprasial
    {
        public Nullable<decimal> WorkDone { get; set; }
        public Nullable<decimal> BugCreated { get; set; }
        public Nullable<decimal> WorkQuality { get; set; }
        public Nullable<decimal> DailyPerformance { get; set; } 
    }
}