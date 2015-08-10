using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.OrgStruct
{
    public class SubBranchViewModel
    {
        public Branch Branch { get; set; }
        public SubBranch SubBranch { get; set; }
        public IEnumerable<SubBranch> SubBranches { get; set; }
    }
}