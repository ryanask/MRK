using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using AskrindoMVC.Areas.Report.Models.RiskMap;
using System.IO;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Areas.Report.Controllers
{
    [Authorize]
    public class RiskMapController : Controller
    {
        private AskrindoMVCEntities db = new AskrindoMVCEntities();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            RiskMapViewModel vm = new RiskMapViewModel();
            vm.Param = new RiskMapParam();
            vm.Param.PosId = 1;
            vm.Param.MapDate = DateTime.Now;
            UpdateRiskMapParam(vm.Param);
            CalcRiskMap(vm);

            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(RiskMapViewModel vm)
        {
            UpdateRiskMapParam(vm.Param);
            CalcRiskMap(vm);
            return View(vm);
        }

        public ActionResult ShowRisks(int prob, int impact, int? posId, int? branchId, DateTime mapDate, bool isApproved)
        {
            var risks = db.Risks.Where(p => p.ProbLevelId != null && p.ImpactLevelId != null);

            if (posId == 1)
                risks = risks.Where(p => p.DeptId != null);
            else if (posId == 2)
            {
                risks = risks.Where(p => p.BranchId != null);
                if (branchId != null)
                    risks = risks.Where(p => p.BranchId == branchId);
            }
            if (isApproved)
            {
                risks = risks.Where(p => p.CloseDate == null || p.CloseDate > mapDate);
                risks = risks.Where(p => p.ApprovalDate != null && p.ApprovalDate <= mapDate);
            }
            else
            {
                risks = risks.Where(p => p.ApprovalDate == null && p.RiskDate <= mapDate);
                risks = risks.Where(p => p.ProbLevelId == prob && p.ImpactLevelId == impact);
            }

            List<Risk> riskList = new List<Risk>();
            foreach (var r in risks)
            {
                if (isApproved)
                {
                    var rm = db.RiskMitigations
                        .Where(p => p.RiskId == r.RiskId && p.ApprovalDate != null && p.ApprovalDate <= mapDate && p.ProbLevelId == prob && p.ImpactLevelId == impact)
                        .OrderByDescending(p => p.ApprovalDate);
                    if (rm.Count() > 0)
                        riskList.Add(r);
                    else
                    {
                        if (r.ProbLevelId == prob && r.ImpactLevelId == impact)
                            riskList.Add(r);
                    }
                }
                else
                    riskList.Add(r);
            }

            return View(riskList);
        }

        public void ExportToWord(int? posId, int? branchId, DateTime mapDate, bool isApproved)
        {
            RiskMapViewModel vm = new RiskMapViewModel();
            vm.Param = new RiskMapParam();
            vm.Param.PosId = posId;
            vm.Param.BranchId = branchId;
            vm.Param.MapDate = mapDate;
            vm.Param.IsApproved = isApproved;
            CalcRiskMap(vm);

            var cnt = 0;
            var backColor = string.Empty;
            var foreColor = string.Empty;
            int prob = 0;
            int impact = 0;
            StringWriter sw = new StringWriter();

            sw.WriteLine("<table style='font-family: Tahoma, verdana; font-size: 1em'>");
            for (var i = 7; i >= 1; i--)
            {
                sw.WriteLine("<tr>");
                for (var j = 1; j <= 7; j++)
                {
                    prob = i - 2;
                    impact = j - 2;
                    if (i == 7 && j == 1)
                    {
                        sw.WriteLine("<td rowspan='7' style='width: 20px; text-align: center; font-weight: bold; font-size: 1em'>");
                        sw.WriteLine("P<br />R<br />O<br />B<br />A<br />B<br />I<br />L<br />I<br />T<br />A<br />S");
                        sw.WriteLine("</td>");
                    }
                    else if (i == 1 && j == 1)
                    {
                        sw.WriteLine("<td colspan='6' style='padding-top: 4px; text-align: center; font-weight: bold; font-size: 1em'>");
                        sw.WriteLine("D A M P A K");
                        sw.WriteLine("</td>");
                    }
                    else if (i == 1 && j > 1)
                    {
                    }
                    else if (j>1)
                    {
                        if (impact == 0 && prob != 0)
                        {
                            sw.WriteLine("<td style='padding-right: 8px; text-align: right; width: 80px; height: 46px; font-size: .85em'>");
                            sw.WriteLine(Utils.GetProbLevelText(prob));
                            sw.WriteLine("</td>");
                        }
                        else if (impact != 0 && prob == 0)
                        {
                            sw.WriteLine("<td style='padding-top: 4px; text-align: center; vertical-align: top; width: 80px; font-size: .85em'>");
                            sw.WriteLine(Utils.GetImpactLevelText(impact));
                            sw.WriteLine("</td>");
                        }
                        else if (impact == 0 && prob == 0)
                        {
                            sw.WriteLine("<td></td>");
                        }
                        else
                        {
                            Utils.GetRiskLevelColors(prob * impact, out backColor, out foreColor);
                            cnt = RiskMapViewModel.GetCount(prob, impact, vm.RiskList);
                            sw.WriteLine("<td style='text-align: center; background-color: " + backColor + "; color: " + foreColor + "'>");
                            if (cnt > 0)
                            {
                                sw.WriteLine("<div style='font-size: " + (8 + (decimal)cnt / (decimal)vm.MaxCount * 20M).ToString() + "pt; font-weight: bold'>");
                                sw.WriteLine(cnt);
                                sw.WriteLine("</div>");
                            }
                            sw.WriteLine("</td>");
                        }
                    }
                }
                sw.WriteLine("</tr>");
            }
            sw.WriteLine("</table>");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=risk_map.doc");
            Response.ContentType = "application/vnd.ms-word";
            Response.Write(sw.ToString());
            Response.End();
        }

        private void CalcRiskMap(RiskMapViewModel vm)
        {
            vm.RiskList = new List<Models.RiskMap.RiskMapData>();
            for (var i = 1; i <= 5; i++)
                for (var j = 1; j <= 5; j++)
                    vm.RiskList.Add(new RiskMapData { ProbLevelId = i, ImpactLevelId = j, Count = 0, RiskLevel = i * j });

            var risks = db.Risks.Where(p => p.ProbLevelId != null && p.ImpactLevelId != null);

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
                risks = risks.Where(p => p.CloseDate == null || p.CloseDate > vm.Param.MapDate);
                risks = risks.Where(p => p.ApprovalDate != null && p.ApprovalDate <= vm.Param.MapDate);
            }
            else
            {
                risks = risks.Where(p => p.ApprovalDate == null && p.RiskDate <= vm.Param.MapDate);
            }

            foreach (var r in risks)
            {
                if (vm.Param.IsApproved)
                {
                    var rm = db.RiskMitigations
                        .Where(p => p.RiskId == r.RiskId && p.ApprovalDate != null && p.ApprovalDate <= vm.Param.MapDate)
                        .OrderByDescending(p => p.ApprovalDate)
                        .First();
                    if (rm != null)
                        AddProbImpactToList((int)rm.ProbLevelId, (int)rm.ImpactLevelId, vm.RiskList);
                    else
                        AddProbImpactToList((int)r.ProbLevelId, (int)r.ImpactLevelId, vm.RiskList);
                }
                else
                    AddProbImpactToList((int)r.ProbLevelId, (int)r.ImpactLevelId, vm.RiskList);
            }
            vm.CalcMinMaxCount();
        }

        private void UpdateRiskMapParam(RiskMapParam param)
        {
            Dictionary<int, string> posList = new Dictionary<int, string>();
            posList.Add(1, "Kantor Pusat");
            posList.Add(2, "Cabang");
            param.PosList = new SelectList(posList, "Key", "Value", param.PosId);

            Dictionary<int, string> branchList = new Dictionary<int, string>();
            foreach (var branch in db.Branches.OrderBy(m => m.ClassId).ThenBy(m => m.BranchName))
                branchList.Add(branch.BranchId, branch.BranchName + " (Kelas " + branch.BranchClass.ClassName + ")");
            param.Branches = new SelectList(branchList, "Key", "Value", param.BranchId);
        }

        private void AddRiskToList(Risk r, List<RiskMapData> list)
        {
            foreach(var item in list)
                if (r.ProbLevelId == item.ProbLevelId && r.ImpactLevelId == item.ImpactLevelId)
                {
                    item.Count++;
                    return;
                }
        }

        private void AddProbImpactToList(int probLevel, int impactLevel, List<RiskMapData> list)
        {
            foreach (var item in list)
                if (item.ProbLevelId == probLevel && item.ImpactLevelId == impactLevel)
                {
                    item.Count++;
                    return;
                }
        }
    }
}
