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
    
    public partial class VoteProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> TermFrom { get; set; }
        public Nullable<System.DateTime> TermTo { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> VoteNum { get; set; }
        public Nullable<System.DateTime> BeginTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
    }
}
