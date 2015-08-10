using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using AskrindoMVC.Helpers;
using AskrindoMVC.Areas.Report.Models.RiskTable;
using System.IO;

namespace AskrindoMVC.Areas.Report.Controllers
{
    public class RiskTableController : Controller
    {
        private AskrindoMVCEntities db = new AskrindoMVCEntities();

        public const int GROUP_NONE = 1;
        public const int GROUP_CAUSE = 2;
        public const int GROUP_EFFECT = 3;
        public const int GROUP_RISKCLASS = 4;
        public const int GROUP_HQ_BRANCH = 5;
        public const int GROUP_CLASSBRANCH = 6;
        public const int GROUP_BRANCH_CLASS1 = 7;
        public const int GROUP_BRANCH_CLASS2 = 8;
        public const int GROUP_BRANCH_CLASS3 = 9;
        public const int GROUP_PROBLEVEL = 10;
        public const int GROUP_IMPACTLEVEL = 11;
        public const int GROUP_RISKLEVEL = 12;
        public const int GROUP_RANGERISKLEVEL = 13;

        public const int DATATYPE_COUNT = 1;
        public const int DATATYPE_PROBLEVEL = 2;
        public const int DATATYPE_IMPACTLEVEL = 3;
        public const int DATATYPE_RISKLEVEL = 4;

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            RiskTableViewModel vm = new RiskTableViewModel();
            vm.RiskList = new List<Risk>();
            vm.Param = new RiskTableParam();
            vm.CellList = new List<CellData>();
            vm.Param.PosId = 1;
            vm.Param.IsApproved = true;
            vm.Param.ReportDate = DateTime.Now;
            vm.Param.DataTypeId = DATATYPE_COUNT;
            vm.Param.RowGroupId = GROUP_NONE;
            vm.Param.ColGroupId = GROUP_NONE;
            UpdateParam(vm);
            PrepareRowsAndCols(vm);
            CalcRisk(vm);
            CalcCellValues(vm);
            ViewBag.DataTypeText = GetDataTypeText(vm.Param.DataTypeId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(RiskTableViewModel vm)
        {
            vm.RiskList = new List<Risk>();
            vm.CellList = new List<CellData>();
            UpdateParam(vm);
            PrepareRowsAndCols(vm);
            CalcRisk(vm);
            CalcCellValues(vm);
            ViewBag.DataTypeText = GetDataTypeText(vm.Param.DataTypeId);
            return View(vm);
        }

        private void UpdateParam(RiskTableViewModel vm)
        {
            Dictionary<int, string> posList = new Dictionary<int, string>();
            posList.Add(1, "Kantor Pusat");
            posList.Add(2, "Cabang");
            vm.Param.PosList = new SelectList(posList, "Key", "Value", vm.Param.PosId);

            Dictionary<int, string> branchList = new Dictionary<int, string>();
            foreach (var branch in db.Branches.OrderBy(m => m.ClassId).ThenBy(m => m.BranchName))
                branchList.Add(branch.BranchId, branch.BranchName + " (Kelas " + branch.BranchClass.ClassName + ")");
            vm.Param.Branches = new SelectList(branchList, "Key", "Value", vm.Param.BranchId);

            Dictionary<int, string> dataList = new Dictionary<int, string>();
            dataList.Add(DATATYPE_COUNT, "Jumlah Data");
            dataList.Add(DATATYPE_PROBLEVEL, "Tingkat Probabilitas");
            dataList.Add(DATATYPE_IMPACTLEVEL, "Tingkat Dampak");
            dataList.Add(DATATYPE_RISKLEVEL, "Tingkat Risiko");
            vm.Param.DataTypes = new SelectList(dataList, "Key", "Value", vm.Param.DataTypeId);

            Dictionary<int, string> groupList = new Dictionary<int, string>();
            groupList.Add(GROUP_NONE, "(Tanpa pengelompokan)");
            groupList.Add(GROUP_CAUSE, "Sebab Risiko");
            groupList.Add(GROUP_EFFECT, "Akibat Risiko");
            groupList.Add(GROUP_RISKCLASS, "Klasifikasi Risiko");
            groupList.Add(GROUP_HQ_BRANCH, "Pusat/Cabang");
            groupList.Add(GROUP_CLASSBRANCH, "Kelas Cabang");
            groupList.Add(GROUP_BRANCH_CLASS1, "Cabang Kelas I");
            groupList.Add(GROUP_BRANCH_CLASS2, "Cabang Kelas II");
            groupList.Add(GROUP_BRANCH_CLASS3, "Cabang Kelas III");
            groupList.Add(GROUP_PROBLEVEL, "Tingkat Probabilitas");
            groupList.Add(GROUP_IMPACTLEVEL, "Tingkat Dampak");
            groupList.Add(GROUP_RISKLEVEL, "Tingkat Risiko");
            groupList.Add(GROUP_RANGERISKLEVEL, "Kelompok Tingkat Risiko");
            vm.Param.RowGroups = new SelectList(groupList, "Key", "Value", vm.Param.RowGroupId);
            vm.Param.ColGroups = new SelectList(groupList, "Key", "Value", vm.Param.ColGroupId);
        }

        private void PrepareRowsAndCols(RiskTableViewModel vm)
        {
            vm.Param.RowIds = new List<int>();
            vm.Param.ColIds = new List<int>();
            vm.Param.RowLabels = new List<string>();
            vm.Param.ColLabels = new List<string>();

            string header;
            GetGroupIdsLabels(vm.Param.ColGroupId, vm.Param.ColIds, vm.Param.ColLabels, out header);
            vm.Param.ColHeader = header;
            GetGroupIdsLabels(vm.Param.RowGroupId, vm.Param.RowIds, vm.Param.RowLabels, out header);
            vm.Param.RowHeader = header;
            vm.Param.ColCount = vm.Param.ColIds.Count();
            vm.Param.RowCount = vm.Param.RowIds.Count();

            vm.Param.RowTotals = new List<decimal>();
            for (var i = 0; i < vm.Param.RowCount; i++)
                vm.Param.RowTotals.Add(0M);

            vm.Param.ColTotals = new List<decimal>();
            for (var i = 0; i < vm.Param.ColCount; i++)
                vm.Param.ColTotals.Add(0M);

            foreach (var i in vm.Param.RowIds)
                foreach (var j in vm.Param.ColIds)
                    vm.CellList.Add(new CellData() { RowId = i, ColId = j, Count = 0, Values = 0 });
        }

        private void GetGroupIdsLabels(int groupId, List<int> listIds, List<string> listLabels, out string header)
        {
            listIds.Clear();
            listLabels.Clear();
            header = string.Empty;

            switch(groupId)
            {
                case GROUP_NONE:
                    header = string.Empty;
                    listIds.Add(1);
                    listLabels.Add("(Semua Data)");
                    break;
                case GROUP_CAUSE:
                    header = "Sebab Risiko";
                    var causeGroups = db.CauseGroups.OrderBy(p => p.CauseGroupId);
                    foreach (var m in causeGroups)
                    {
                        listIds.Add(m.CauseGroupId);
                        listLabels.Add(m.CauseGroupName);
                    }
                    break;
                case GROUP_EFFECT:
                    header = "Akibat Risiko";
                    var effectGroups = db.EffectGroups.OrderBy(p => p.EffectGroupId);
                    foreach (var m in effectGroups)
                    {
                        listIds.Add(m.EffectGroupId);
                        listLabels.Add(m.EffectGroupName);
                    }
                    break;
                case GROUP_RISKCLASS:
                    header = "Klasifikasi Risiko";
                    var riskCats = db.RiskCats.OrderBy(p => p.RiskCatId);
                    foreach (var m in riskCats)
                    {
                        listIds.Add(m.RiskCatId);
                        listLabels.Add(m.RiskCatName);
                    }
                    break;
                case GROUP_HQ_BRANCH:
                    header = "Kantor Pusat/Cabang";
                    listIds.Add(1);
                    listIds.Add(2);
                    listLabels.Add("Kantor Pusat");
                    listLabels.Add("Kantor Cabang");
                    break;
                case GROUP_CLASSBRANCH:
                    header = "Cabang";
                    var branchClasses = db.BranchClasses.OrderBy(p => p.ClassId);
                    foreach (var m in branchClasses)
                    {
                        listIds.Add(m.ClassId);
                        listLabels.Add("Kelas " + m.ClassName);
                    }
                    break;
                case GROUP_BRANCH_CLASS1:
                    header = "Cabang Kelasi I";
                    var branches1 = db.Branches.Where(p => p.ClassId == Utils.BRANCHCLASS1).OrderBy(p => p.ClassId);
                    foreach (var m in branches1)
                    {
                        listIds.Add(m.BranchId);
                        listLabels.Add(m.BranchName);
                    }
                    break;
                case GROUP_BRANCH_CLASS2:
                    header = "Cabang Kelas II";
                    var branches2 = db.Branches.Where(p => p.ClassId == Utils.BRANCHCLASS2).OrderBy(p => p.ClassId);
                    foreach (var m in branches2)
                    {
                        listIds.Add(m.BranchId);
                        listLabels.Add(m.BranchName);
                    }
                    break;
                case GROUP_BRANCH_CLASS3:
                    header = "Cabang Kelas III";
                    var branches3 = db.Branches.Where(p => p.ClassId == Utils.BRANCHCLASS3).OrderBy(p => p.ClassId);
                    foreach (var m in branches3)
                    {
                        listIds.Add(m.BranchId);
                        listLabels.Add(m.BranchName);
                    }
                    break;
                case GROUP_PROBLEVEL:
                    header = "Tingkat Probabilitas";
                    var probLevels = db.ProbLevels.OrderBy(p => p.ProbLevelId);
                    foreach (var m in probLevels)
                    {
                        listIds.Add(m.ProbLevelId);
                        listLabels.Add(m.ProbLevelId.ToString() + " - " + m.ProbLevelName);
                    }
                    break;
                case GROUP_IMPACTLEVEL:
                    header = "Tingkat Dampak";
                    var impactLevels = db.ImpactLevels.OrderBy(p => p.ImpactLevelId);
                    foreach (var m in impactLevels)
                    {
                        listIds.Add(m.ImpactLevelId);
                        listLabels.Add(m.ImpactLevelId.ToString() + " - " + m.ImpactLevelName);
                    }
                    break;
                case GROUP_RISKLEVEL:
                    header = "Tingkat Risiko";
                    int[] riskLevels = { 1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 15, 16, 20, 25 };
                    foreach (var m in riskLevels)
                    {
                        listIds.Add(m);
                        listLabels.Add(m.ToString());
                    }
                    break;
                case GROUP_RANGERISKLEVEL:
                    header = "Tingkat Risiko";
                    string[] ranges = { "x <= 5", "5 < x <= 8", "8 < x <= 12", "12 < x <= 25" };
                    for (var i = 0; i < ranges.Count(); i++ )
                    {
                        listIds.Add(i + 1);
                        listLabels.Add(ranges[i]);
                    }
                    break;
            }
        }

        private void CalcRisk(RiskTableViewModel vm)
        {
            var risks = db.Risks.Where(p => p.ProbLevelId != null && p.ImpactLevelId != null && p.RiskCatId != null);
            if (vm.Param.PosId == 1)
                risks = risks.Where(p => p.DeptId != null);
            else if (vm.Param.PosId == 2)
            {
                risks = risks.Where(p => p.BranchId != null);
                if (vm.Param.BranchId != null)
                    risks = risks.Where(p => p.BranchId == vm.Param.BranchId);
            }

            if (vm.Param.IsApproved)
            {
                risks = risks.Where(p => p.CloseDate == null || p.CloseDate > vm.Param.ReportDate);
                risks = risks.Where(p => p.ApprovalDate != null && p.ApprovalDate <= vm.Param.ReportDate);
            }
            else
            {
                risks = risks.Where(p => p.ApprovalDate == null && p.RiskDate <= vm.Param.ReportDate);
            }

            foreach (var r in risks)
            {
                if (vm.Param.IsApproved)
                {
                    var rm = db.RiskMitigations.Where(p => p.RiskId == r.RiskId && p.ApprovalDate != null && p.ApprovalDate <= vm.Param.ReportDate);
                    if (rm.Count() > 0)
                    {
                        var m = rm.OrderByDescending(p => p.ApprovalDate).First();
                        r.ProbLevelId = (int)m.ProbLevelId;
                        r.ImpactLevelId = (int)m.ImpactLevelId;
                        r.RiskLevel = (int)m.RiskLevel;
                    }
                }
                vm.RiskList.Add(r);
            }
        }

        private void CalcCellValues(RiskTableViewModel vm)
        {
            int row;
            int col;
            foreach (var r in vm.RiskList)
            {
                row = GetValueId(r, vm.Param.RowGroupId, vm.Param.RowIds);
                col = GetValueId(r, vm.Param.ColGroupId, vm.Param.ColIds);
                foreach(var cell in vm.CellList)
                    if (cell.RowId == row && cell.ColId == col)
                    {
                        cell.Count++;
                        switch(vm.Param.DataTypeId)
                        {
                            case DATATYPE_COUNT:
                                cell.Values = 1;
                                break;
                            case DATATYPE_PROBLEVEL:
                                cell.Values += (int)r.ProbLevelId;
                                break;
                            case DATATYPE_IMPACTLEVEL:
                                cell.Values += (int)r.ImpactLevelId;
                                break;
                            case DATATYPE_RISKLEVEL:
                                cell.Values += (int)r.RiskLevel;
                                break;
                        }
                    }
            }
        }

        private int GetValueId(Risk r, int groupId, List<int> Ids)
        {
            switch(groupId)
            {
                case GROUP_NONE:
                    return 1;
                case GROUP_CAUSE:
                    return (int)r.CauseGroupId;
                case GROUP_EFFECT:
                    return (int)r.EffectGroupId;
                case GROUP_RISKCLASS:
                    return (int)r.RiskCatId;
                case GROUP_HQ_BRANCH:
                    if (r.DeptId != null)
                        return 1;
                    else
                        return 2;
                case GROUP_CLASSBRANCH:
                    if (r.BranchId != null)
                        return r.Branch.ClassId;
                    else
                        return -1;
                case GROUP_BRANCH_CLASS1:
                    if (r.BranchId != null && r.Branch.ClassId == Utils.BRANCHCLASS1)
                        return (int)r.BranchId;
                    else
                        return -1;
                case GROUP_BRANCH_CLASS2:
                    if (r.BranchId != null && r.Branch.ClassId == Utils.BRANCHCLASS2)
                        return (int)r.BranchId;
                    else
                        return -1;
                case GROUP_BRANCH_CLASS3:
                    if (r.BranchId != null && r.Branch.ClassId == Utils.BRANCHCLASS3)
                        return (int)r.BranchId;
                    else
                        return -1;
                case GROUP_PROBLEVEL:
                    return (int)r.ProbLevelId;
                case GROUP_IMPACTLEVEL:
                    return (int)r.ImpactLevelId;
                case GROUP_RISKLEVEL:
                    return (int)r.RiskLevel;
                case GROUP_RANGERISKLEVEL:
                    if (r.RiskLevel <= 5)
                        return 1;
                    else if (r.RiskLevel > 5 && r.RiskLevel <= 8)
                        return 2;
                    else if (r.RiskLevel > 8 && r.RiskLevel <= 12)
                        return 3;
                    else
                        return 4;
                default:
                    return -1;

            }
        }

        private string GetDataTypeText(int dataTypeId)
        {
            switch(dataTypeId)
            {
                case DATATYPE_COUNT:
                    return "Banyak Peristiwa Risiko";
                case DATATYPE_PROBLEVEL:
                    return "Tingkat Probabilitas";
                case DATATYPE_IMPACTLEVEL:
                    return "Tingkat Dampak";
                case DATATYPE_RISKLEVEL:
                    return "Tingkat Risiko";
                default:
                    return string.Empty;
            }
        }

        public void ExportToExcel(int? posId, int? branchId, DateTime reportDate, bool isApproved, int dataTypeId, int rowGroupId, int colGroupId)
        {
            
            RiskTableViewModel vm = new RiskTableViewModel();
            vm.RiskList = new List<Risk>();
            vm.Param = new RiskTableParam();
            vm.CellList = new List<CellData>();
            vm.Param.PosId = posId;
            vm.Param.BranchId = branchId;
            vm.Param.ReportDate = reportDate;
            vm.Param.IsApproved = isApproved;
            vm.Param.DataTypeId = dataTypeId;
            vm.Param.RowGroupId = rowGroupId;
            vm.Param.ColGroupId = colGroupId;
            UpdateParam(vm);
            PrepareRowsAndCols(vm);
            CalcRisk(vm);
            CalcCellValues(vm);

            int rowId = -1;
            int colId = -1;
            int index = -1;
            int intValue = 0;
            decimal decValue = 0;

            StringWriter sw = new StringWriter();
            sw.WriteLine("Data: " + GetDataTypeText(vm.Param.DataTypeId));

            sw.WriteLine("<table rules='all' border='1' style='border-collapse:collapse;'>");
                if (vm.Param.ColCount > 1)
                { 
                sw.WriteLine("<tr>");
                    sw.WriteLine("<th rowspan='2' style='text-align: center; background-color: #eee'>" + vm.Param.RowHeader + "</th>");
                    sw.WriteLine("<th colspan='" + vm.Param.ColCount + "' style='text-align: center; background-color: #eee'>" + vm.Param.ColHeader + "</th>");
                sw.WriteLine("</tr>");
                }
                sw.WriteLine("<tr>");
                if (vm.Param.ColCount == 1)
                {
                    sw.WriteLine("<th style='text-align: center; background-color: #eee'>" + vm.Param.RowHeader + "</th>");
                }
                for (var i = 0; i < vm.Param.ColCount; i++)
                {
                    sw.WriteLine("<th style='text-align: center; background-color: #eee'>");
                    sw.WriteLine(vm.Param.ColLabels[i]);
                    sw.WriteLine("</th>");
                }
                sw.WriteLine("</tr>");

                for (var i = 0; i < vm.Param.RowCount; i++)
                { 
                    sw.WriteLine("<tr>");
                        sw.WriteLine("<td style='background-color: #eee; font-weight: bold'>" + vm.Param.RowLabels[i] + "</td>");
                        for (var j = 0; j < vm.Param.ColCount; j++)
                        {
                            sw.WriteLine("<td align='right'>");
                                {
                                    rowId = vm.Param.RowIds[i];
                                    colId = vm.Param.ColIds[j];
                                    index = RiskTableViewModel.GetCellDataIndex(rowId, colId, vm.CellList);
                                    if (index >= 0)
                                    {
                                        if (vm.Param.DataTypeId == 1)
                                        {
                                            intValue = vm.CellList[index].Count;
                                            if (intValue > 0)
                                            { 
                                                sw.WriteLine(intValue);
                                            }
                                        }
                                        else
                                        {
                                            if (vm.CellList[index].Count > 0)
                                            {
                                                decValue = vm.CellList[index].Values / vm.CellList[index].Count;
                                                if (decValue > 0)
                                                { 
                                                    sw.WriteLine(string.Format("{0:#,##0.##}", decValue));
                                                }
                                            }
                                        }
                                    }
                                }
                            sw.WriteLine("</td>");
                        }
                    sw.WriteLine("</tr>");
                }
            sw.WriteLine("</table>");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=risk_table.xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Write(sw.ToString());
            Response.End();
        }
    }
}
