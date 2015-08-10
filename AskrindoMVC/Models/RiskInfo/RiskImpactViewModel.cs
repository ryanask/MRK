using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Models.RiskInfo
{
    public class RiskImpactViewModel
    {
        public RiskImpact RiskImpact { get; set; }
        public IEnumerable<RiskNonMoneyImpact> RiskNonMoneyImpacts { get; set; }
    }
}