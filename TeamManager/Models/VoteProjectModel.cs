using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TeamManager.DataProvider;

namespace TeamManager.Models
{
    public class VoteProjectModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public DateTime TermFrom { get; set; }
        [Required]
        public DateTime TermTo { get; set; }
        public Nullable<int> State { get; set; }
        [Required]
        public Nullable<int> VoteNum { get; set; }
        [Required]
        public DateTime BeginTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
    }
}