using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;
using System.Web.Mvc;

namespace AskrindoMVC.Areas.RiskData.Models.RiskData
{
    public class RiskEditViewModel
    {
        public Risk Risk { get; set; }

        public SelectList CauseGroups { get; set; }
        public SelectList CauseTypes { get; set; }
        public SelectList Causes { get; set; }
        
        public SelectList EffectGroups { get; set; }
        public SelectList EffectTypes { get; set; }
        public SelectList Effects { get; set; }

        public SelectList RiskCats { get; set; }
        public SelectList RiskGroups { get; set; }
        public SelectList RiskTypes { get; set; }
    }
}