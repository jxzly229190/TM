using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QA_System.Models
{
    public class UserModels
    {
        private string _userID;

        public string UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }        
    }
}