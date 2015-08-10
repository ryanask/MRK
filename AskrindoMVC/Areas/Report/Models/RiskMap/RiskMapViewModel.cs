using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AskrindoMVC.Areas.Report.Models.RiskMap
{
    public class RiskMapData
    {
        public int ProbLevelId { get; set; }
        public int ImpactLevelId { get; set; }
        public int RiskLevel { get; set; }
        public int Count { get; set; }
    }

    public class RiskMapParam
    {
        public int? PosId { get; set; }
        public SelectList PosList { get; set; }

        public int? BranchId { get; set; }
        public SelectList Branches { get; set; }

        public bool IsApproved { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime MapDate { get; set; }
    }

    public class RiskMapViewModel
    {
        public List<RiskMapData> RiskList { get; set; }
        public RiskMapParam Param { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }

        public void CalcMinMaxCount()
        {
            var minValue = int.MaxValue;
            int maxValue = int.MinValue;

            foreach (var item in RiskList)
            {
                if (item.Count < minValue)
                    minValue = item.Count;
                if (item.Count > maxValue)
                    maxValue = item.Count;
            }
            MinCount = minValue;
            MaxCount = maxValue;
        }

        public static int GetCount(int probLevelId, int impactLevelId, List<RiskMapData> list)
        {
            foreach (var item in list)
                if (item.ProbLevelId == probLevelId && item.ImpactLevelId == impactLevelId)
                    return item.Count;
            return 0;
        }
    }
}