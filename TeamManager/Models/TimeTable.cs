using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamManager.DataProvider;

namespace TeamManager.Models
{
    public class TimeTable
    {
        public User user { get; set; }
        public UserLeave userLeave { get; set; }
    }
}