using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TeamManager.DataProvider;

namespace TeamManager.Models
{
    public class Users
    {
        [Required]
        [Display(Name = "User name")]
        public string UserID { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool IsValid(string userid, string password)
        {
            using (var db = new TeamManage_Entities())
            {                                   
                var user = db.Users.SingleOrDefault(u => u.UserID == userid && u.LoginPassword == password);                       
                    
                if (user != null)
                    return true;
                else
                    return false; 
            }
        }
    }
}