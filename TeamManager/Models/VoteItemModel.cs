using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TeamManager.Models
{
    public class VoteItemModel
    {
        public int Id { get; set; }
        public Nullable<int> PId { get; set; }
        [Required]
        [StringLength(50,MinimumLength=5)]
        public string Name { get; set; }
        public string Nominees { get; set; }
        public string Nominator { get; set; }
        public string Comment { get; set; }
        public bool IsSelected { get; set; }
		public int Count { get;set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
    }
}