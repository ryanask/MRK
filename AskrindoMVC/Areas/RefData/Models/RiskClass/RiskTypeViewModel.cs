using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.RiskClass
{
    public class RiskTypeViewModel
    {
        public RiskType RiskType { get; set; }
        public IEnumerable<RiskType> RiskTypes { get; set; }
        public RiskGroup RiskGroup { get; set; }
    }
}