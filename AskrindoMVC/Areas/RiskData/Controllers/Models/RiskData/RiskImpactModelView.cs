using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RiskData.Models.RiskData
{
    public class RiskImpactModelView
    {
        public RiskImpact RiskImpact { get; set; }
        public IEnumerable<RiskNonMoneyImpact> RiskNonMoneyImpacts { get; set; }
    }
}