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
    
    public partial class t_AlertUser
    {
        public t_AlertUser()
        {
            this.t_AUAttach = new HashSet<t_AUAttach>();
        }
    
        public int AUID { get; set; }
        public string UserID { get; set; }
        public string CcUser { get; set; }
        public int ABID { get; set; }
        public string AUTitle { get; set; }
        public string AUContent { get; set; }
        public Nullable<int> AUType { get; set; }
        public Nullable<int> AUStatus { get; set; }
        public int AUInterval { get; set; }
        public int AUUnitNum { get; set; }
        public System.DateTime AUTime { get; set; }
        public string AURemark { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
    
        public virtual t_AlertBase t_AlertBase { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<t_AUAttach> t_AUAttach { get; set; }
    }
}
