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
    
    public partial class t_AlertBase
    {
        public t_AlertBase()
        {
            this.t_AlertUser = new HashSet<t_AlertUser>();
        }
    
        public int ABID { get; set; }
        public string ABName { get; set; }
        public Nullable<int> ABType { get; set; }
        public Nullable<int> ABStatus { get; set; }
        public Nullable<int> ABOrder { get; set; }
        public string ABRemark { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual ICollection<t_AlertUser> t_AlertUser { get; set; }
    }
}