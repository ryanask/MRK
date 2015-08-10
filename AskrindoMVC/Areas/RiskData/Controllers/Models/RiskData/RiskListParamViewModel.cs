﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RiskData.Models.RiskData
{
    public class RiskListParamViewModel
    {
        public int? PosId { get; set; }
        public SelectList PosList { get; set; }

        public int? BranchId { get; set; }
        public SelectList Branches { get; set; }

        public int? StateId { get; set; } // 1: normal, 2: read only, 3: approved, 4: closed
        public SelectList States { get; set; }

        public IEnumerable<Risk> Risks { get; set; }
    }
}