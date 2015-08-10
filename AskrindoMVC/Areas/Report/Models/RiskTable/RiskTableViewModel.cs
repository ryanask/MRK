using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.Report.Models.RiskTable
{
    public class RiskTableParam
    {
        public int? PosId { get; set; }
        public int? BranchId { get; set; }
        //public int? RiskLevelId { get; set; }
        public bool IsApproved { get; set; }
        public int DataTypeId { get; set; }
        public int RowGroupId { get; set; }
        public int ColGroupId { get; set; }

        public int RowCount { get; set; }
        public int ColCount { get; set; }
        public string RowHeader { get; set; }
        public string ColHeader { get; set; }
        public List<int> RowIds { get; set; }
        public List<int> ColIds { get; set; }
        public List<string> RowLabels { get; set; }
        public List<string> ColLabels { get; set; }
        public List<decimal> RowTotals { get; set; }
        public List<decimal> ColTotals { get; set; }
        public decimal Total { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        public SelectList PosList { get; set; }
        public SelectList Branches { get; set; }
        public SelectList RiskLevels { get; set; }
        public SelectList DataTypes { get; set; }
        public SelectList RowGroups { get; set; }
        public SelectList ColGroups { get; set; }
    }

    public class CellData
    {
        public int RowId { get; set; }
        public int ColId { get; set; }
        public int Count { get; set; }
        public decimal Values { get; set; }
    }

    public class RiskTableViewModel
    {
        public RiskTableParam Param { get; set; }
        public List<Risk> RiskList { get; set; }
        public List<CellData> CellList { get; set; }

        public static int GetCellDataIndex(int rowId,int colId, List<CellData> cellList)
        {
            for (var i = 0; i < cellList.Count; i++)
            {
                var cell = cellList[i];
                if (cell.RowId == rowId && cell.ColId == colId)
                    return i;
            }
            return -1;
        }
    }
}