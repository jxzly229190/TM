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
    
    public partial class QA_Files
    {
        public QA_Files()
        {
            this.QA_Request = new HashSet<QA_Request>();
            this.QA_Test = new HashSet<QA_Test>();
            this.QA_TestIssue = new HashSet<QA_TestIssue>();
        }
    
        public System.Guid FileID { get; set; }
        public string Name { get; set; }
        public byte[] FileContent { get; set; }
        public string Type { get; set; }
    
        public virtual ICollection<QA_Request> QA_Request { get; set; }
        public virtual ICollection<QA_Test> QA_Test { get; set; }
        public virtual ICollection<QA_TestIssue> QA_TestIssue { get; set; }
    }
}
