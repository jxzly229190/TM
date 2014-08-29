using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamManager.DataProvider;

namespace TeamManager.Models
{
    public class OverTimeTable
    {
        public OverTime overtime { get; set; }
        public User user { get; set; }
    }
}