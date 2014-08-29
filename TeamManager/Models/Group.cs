using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManager.Models
{
    public class Group
    {
        public string GroupName { get; set; }
        public List<Monthly> Monthly { get; set; }
    }
}