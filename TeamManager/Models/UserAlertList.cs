using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamManager.DataProvider;
using TeamManager.Models;

namespace TeamManager.Models
{
    public class UserAlertList
    {
        public UserList userList { get; set; }
        public AlertList alertList { get; set; }

        public UserAlertList()
        {
            this.userList = new UserList();
            this.alertList = new AlertList();
        }
    }
    
    
    public class UserList
    {
        public t_AlertUser user { get; set; }
        public IEnumerable<t_AUAttach> userAttachs { get; set; }
    }
    public class AlertList
    {
        public t_AlertBase alert { get; set; }
        public IEnumerable<t_ABAttach> alertAttachs { get; set; }
    }
}