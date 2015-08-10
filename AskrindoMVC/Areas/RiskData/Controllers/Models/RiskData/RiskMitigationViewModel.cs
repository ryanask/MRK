using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;
using System.Web.Mvc;

namespace AskrindoMVC.Areas.RiskData.Models.RiskData
{
    public class RiskMitigationViewModel
    {       
        public Risk Risk { get; set; }
        public RiskMitigation RiskMitigation { get; set; }
        public IEnumerable<RiskMitigation> RiskMitigations { get; set; }
        public SelectList MitigationCats { get; set; }
        public SelectList MitigationTypes { get; set; }
        public SelectList ProbLevels { get; set; }
        public SelectList ImpactLevels { get; set; }

        public IEnumerable<MitigationUnit> MitigationUnit{ get; set;}
        public MitigationUnit MitigationUnits { get; set; }

        public Division division { get; set; }
        public IEnumerable<divisionTable> DivisionTable { get; set; }
        public SelectList divisions { get; set; }
        public IEnumerable<Division> divisionName { get; set; }

        public Boolean IsMitigation{ get; set; }

        public IEnumerable<MitigationApproval> MitigationApprovals { get; set; }
    }

    public class divisionTable
    {
        public int DivisionId { get; set; }
        public String DivisionName { get; set; }
        public int MitigationUnitId { get; set; }
        public int MitigationId { get; set; }
    }
}