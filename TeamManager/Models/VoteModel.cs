using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamManager.Models
{
    public class VoteModel
    {
        public DataProvider.VoteProject Project;

        public IEnumerable<Models.VoteItemModel> Items;
    }
}