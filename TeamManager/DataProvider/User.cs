//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TeamManager.DataProvider
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.t_ABAttach = new HashSet<t_ABAttach>();
            this.t_ABAttach1 = new HashSet<t_ABAttach>();
            this.t_AlertBase = new HashSet<t_AlertBase>();
            this.t_AlertBase1 = new HashSet<t_AlertBase>();
            this.t_AlertUser = new HashSet<t_AlertUser>();
            this.t_AlertUser1 = new HashSet<t_AlertUser>();
            this.t_AUAttach = new HashSet<t_AUAttach>();
            this.t_AUAttach1 = new HashSet<t_AUAttach>();
        }
    
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string LoginPassword { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public Nullable<bool> IsTrainee { get; set; }
        public Nullable<bool> IsDeparted { get; set; }
        public string EmailAddress { get; set; }
        public string EMAccount { get; set; }
        public string EMPassword { get; set; }
        public string JIRAToken { get; set; }
    
        public virtual ICollection<t_ABAttach> t_ABAttach { get; set; }
        public virtual ICollection<t_ABAttach> t_ABAttach1 { get; set; }
        public virtual ICollection<t_AlertBase> t_AlertBase { get; set; }
        public virtual ICollection<t_AlertBase> t_AlertBase1 { get; set; }
        public virtual ICollection<t_AlertUser> t_AlertUser { get; set; }
        public virtual ICollection<t_AlertUser> t_AlertUser1 { get; set; }
        public virtual ICollection<t_AUAttach> t_AUAttach { get; set; }
        public virtual ICollection<t_AUAttach> t_AUAttach1 { get; set; }
    }
}
