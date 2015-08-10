using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Models.RiskInfo
{
    public class RiskMitigationViewModel
    {
        public Risk Risk { get; set; }
        public IEnumerable<RiskMitigation> ApprovedMitigations { get; set; }
        public IEnumerable<RiskMitigation> PlannedMitigations { get; set; }
    }
}