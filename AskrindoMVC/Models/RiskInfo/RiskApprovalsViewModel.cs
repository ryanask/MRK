using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Models.RiskInfo
{
    public class RiskApprovalsViewModel
    {
        public Risk Risk { get; set; }
        public IEnumerable<RiskApproval> RiskApprovals { get; set; }
    }
}