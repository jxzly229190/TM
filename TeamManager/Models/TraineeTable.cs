using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamManager.DataProvider;

namespace TeamManager.Models
{
    public class TraineeTable
    {
        public TraineeAttendance trainee { get; set; }
        public User user { get; set; }
        public string time { get; set; }
        public int id { get; set; }
    }
}