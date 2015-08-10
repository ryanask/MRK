using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.RiskCause
{
    public class CauseViewModel
    {
        public Cause Cause { get; set; }
        public CauseType CauseType { get; set; }
        public IEnumerable<Cause> Causes { get; set; }
    }
}