using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManager.Models
{
    public class Monthly
    {
        public string UserName { get; set; }
        public List<MonthlyApprasial> MonthlyAppraisal { get; set; }
    }
}