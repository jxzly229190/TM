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
    
    public partial class VoteDetail
    {
        public int Id { get; set; }
        public Nullable<int> PId { get; set; }
        public Nullable<int> IId { get; set; }
        public string Voter { get; set; }
        public Nullable<int> State { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
    }
}