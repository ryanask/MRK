using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.NonMoneyImpact
{
    public class ImpactTypeViewModel
    {
        public ImpactType ImpactType { get; set; }
        public IEnumerable<ImpactType> ImpactTypes { get; set; }
        public ImpactCat ImpactCat { get; set; }
    }
}