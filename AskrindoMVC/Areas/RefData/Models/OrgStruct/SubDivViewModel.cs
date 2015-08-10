using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.OrgStruct
{
    public class SubDivViewModel
    {
        public Division Division { get; set; }
        public SubDiv SubDiv { get; set; }
        public IEnumerable<SubDiv> SubDivs { get; set; }
    }
}