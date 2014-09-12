using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TeamManager.Models
{
    public class VoteItemModel
    {
        public int Id { get; set; }
        public Nullable<int> PId { get; set; }
        [Required]
        public string ProjectName { set; get; }
        [Required]
        [StringLength(50,MinimumLength=5)]
        //[Remote("CheckItemNameExists","Vote",AdditionalFields="PId,Id", ErrorMessage="Name has been used in this project.")] 
        public string Name { get; set; }
        [Required]
        public string Members { get; set; }
        public string Nominator { get; set; }
        [Required]
        [MinLength(10)]
        public string Comment { get; set; }
        public bool IsSelected { get; set; }
        public bool PreSelected { get; set; }
        
        // 投票数量
        public int Count { get; set; }
        public Nullable<int> State { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
    }
}