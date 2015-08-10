using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.RiskEffect
{
    public class EffectViewModel
    {
        public Effect Effect { get; set; }
        public IEnumerable<Effect> Effects { get; set; }
        public EffectType EffectType { get; set; }
    }
}