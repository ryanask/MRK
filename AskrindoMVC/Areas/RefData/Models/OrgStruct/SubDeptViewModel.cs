using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.OrgStruct
{
    public class SubDeptViewModel
    {
        public SubDept SubDept { get; set; }
        public IEnumerable<SubDept> SubDepts { get; set; }
        public Dept Dept { get; set; }
    }
}