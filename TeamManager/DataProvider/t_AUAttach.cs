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
    
    public partial class t_AUAttach
    {
        public int AUAID { get; set; }
        public int AUID { get; set; }
        public string AUAName { get; set; }
        public byte[] AUAFile { get; set; }
        public Nullable<int> AUAType { get; set; }
        public Nullable<int> AUAStatus { get; set; }
        public string AUARemark { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifyBy { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
    
        public virtual t_AlertUser t_AlertUser { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
