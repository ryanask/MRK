using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RiskData.Models.RiskData
{
    public class RiskAttachmentViewModel
    {
        public Risk Risk { get; set; }
        public RiskAttachment RiskAttachment { get; set; }
        public IEnumerable<RiskAttachment> RiskAttachments { get; set; }
    }
}