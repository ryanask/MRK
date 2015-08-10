using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AskrindoMVC.Areas.Report.Models.TopTenRisk
{
    public class TopTenParam
    {
        public int? PosId { get; set; }
        public int? BranchId { get; set; }
        public bool IsApproved { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        public SelectList PosList { get; set; }
        public SelectList Branches { get; set; }

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

    public class TopTenData
    {
        public Risk Risk { get; set; }
        public IEnumerable<RiskMitigation> ApprovedMitigations { get; set; }
        public IEnumerable<RiskMitigation> PlannedMitigations { get; set; }
    }

    public class TopTenMap
    {
        public int ProbLevelId { get; set; }
        public int ImpactLevelId { get; set; }
        public int RiskLevel { get; set; }
        public int Count { get; set; }
    }

    public class TopTenViewModel
    {
        public TopTenParam Param { get; set; }
        public List<Risk> RiskList { get; set; }
        public List<TopTenData> TopTenList { get; set; }
        public List<TopTenMap> MapList { get; set; }
        public int MaxCount { get; set; }

        public static int GetMapCount(int probLevelId, int impactLevelId, List<TopTenMap> list)
        {
            foreach (var item in list)
                if (item.ProbLevelId == probLevelId && item.ImpactLevelId == impactLevelId)
                    return item.Count;
            return 0;
        }
    }
}