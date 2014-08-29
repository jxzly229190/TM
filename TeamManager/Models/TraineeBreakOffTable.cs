using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamManager.DataProvider;

namespace TeamManager.Models
{
    public class TraineeBreakOffTable
    {
        public TraineeBreakOff traineeBreakOff { get; set; }
        public User user { get; set; }
        public string time { get; set; }
    }
}