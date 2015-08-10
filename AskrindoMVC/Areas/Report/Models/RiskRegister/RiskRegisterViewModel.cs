using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AskrindoMVC.Areas.Report.Models.RiskRegister
{
    public class Param
    {
        public int? PosId { get; set; }
        public int? BranchId { get; set; }
        public int? RiskLevelId { get; set; }
        public bool IsApproved { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        public SelectList PosList { get; set; }
        public SelectList Branches { get; set; }
        public SelectList RiskLevels { get; set; }

        public bool ShowRiskCode { get; set; }
        public bool ShowRiskDate { get; set; }
        public bool ShowOrg { get; set; }
        public bool ShowRiskCat { get; set; }
        public bool ShowRiskCause { get; set; }
        public bool ShowRiskEffect { get; set; }
        public bool ShowRiskOwner { get; set; }
        public bool ShowProbLevel { get; set; }
        public bool ShowImpactLevel { get; set; }
        public bool ShowApprovedMitigations { get; set; }
        public bool ShowPlannedMitigations { get; set; }
    }

    public class RiskRecord
    {
        public Risk Risk { get; set; }
        public IEnumerable<RiskMitigation> ApprovedMitigations { get; set; }
        public IEnumerable<RiskMitigation> PlannedMitigations { get; set; }
    }

    public class RiskRegisterViewModel
    {
        public Param Param { get; set; }
        //public List<Risk> RiskList { get; set; }
        public List<RiskRecord> RiskList { get; set; }
    }
}