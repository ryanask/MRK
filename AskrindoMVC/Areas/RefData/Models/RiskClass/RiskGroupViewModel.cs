using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.RiskClass
{
    public class RiskGroupViewModel
    {
        public RiskGroup RiskGroup { get; set; }
        public IEnumerable<RiskGroup> RiskGroups { get; set; }
        public RiskCat RiskCat { get; set; }
    }
}