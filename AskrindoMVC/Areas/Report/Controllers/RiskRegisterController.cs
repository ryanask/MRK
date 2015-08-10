using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Areas.Report.Models.RiskRegister;
using AskrindoMVC.Models;
using AskrindoMVC.Helpers;
using System.IO;

namespace AskrindoMVC.Areas.Report.Controllers
{
    [Authorize]
    public class RiskRegisterController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        public ActionResult Index()
        {
            RiskRegisterViewModel vm = new RiskRegisterViewModel();
            
            vm.Param = new Param();
            vm.Param.IsApproved = true;
            vm.Param.ReportDate = DateTime.Now;
            UpdateParam(vm.Param);
            CalcRiskRegister(vm);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(RiskRegisterViewModel vm)
        {
            UpdateParam(vm.Param);
            CalcRiskRegister(vm);
            return View(vm);
        }

        public void ExportToExcel(int? posId, int? branchId, int? riskLevelId, DateTime reportDate,
            bool isApproved, bool showOrg, bool showRiskCode, bool showRiskDate, 
            bool showRiskCat, bool showRiskCause, bool showRiskEffect,
            bool showRiskOwner, bool showProbLevel, bool showImpactLevel,
            bool showApprovedMitigations, bool showPlannedMitigations)
        {
            RiskRegisterViewModel vm = new RiskRegisterViewModel();
            vm.RiskList = new List<RiskRecord>();
            vm.Param = new Param();
            vm.Param.PosId = posId;
            vm.Param.BranchId = branchId;
            vm.Param.RiskLevelId = riskLevelId;
            vm.Param.ReportDate = reportDate;
            vm.Param.IsApproved = isApproved;
            vm.Param.ShowOrg = showOrg;
            vm.Param.ShowRiskCode = showRiskCode;
            vm.Param.ShowRiskDate = showRiskDate;
            vm.Param.ShowRiskCat = showRiskCat;
            vm.Param.ShowRiskCause = showRiskCause;
            vm.Param.ShowRiskEffect = showRiskEffect;
            vm.Param.ShowRiskOwner = showRiskOwner;
            vm.Param.ShowProbLevel = showProbLevel;
            vm.Param.ShowImpactLevel = showImpactLevel;
            vm.Param.ShowApprovedMitigations = showApprovedMitigations;
            vm.Param.ShowPlannedMitigations = showPlannedMitigations;
            CalcRiskRegister(vm);

            StringWriter sw = new StringWriter();

            sw.WriteLine("<table rules='all' border='1' style='border-collapse:collapse;'>");
            sw.WriteLine("<tr>");
            if (vm.Param.ShowRiskCode)
            {
                sw.WriteLine("<th style='background-color: #eee'>Kode Risiko</th>");
            }
            sw.WriteLine("<th style='background-color: #eee'>Peristiwa Risiko</th>");
            if (vm.Param.ShowRiskDate)
            {
                sw.WriteLine("<th style='background-color: #eee'>Tanggal</th>");
            }
            if (vm.Param.ShowOrg)
            { 
                sw.WriteLine("<th style='background-color: #eee'>Unit Kerja</th>");
            }
            if (vm.Param.ShowRiskCat)
            {
                sw.WriteLine("<th style='background-color: #eee'>Klasifikasi Risiko</th>");
            }
            if (vm.Param.ShowRiskCause)
            {
                sw.WriteLine("<th style='background-color: #eee'>Sebab Risiko</th>");
            }
            if (vm.Param.ShowRiskEffect)
            {
                sw.WriteLine("<th style='background-color: #eee'>Akibat Risiko</th>");
            }
            if (vm.Param.ShowRiskOwner)
            {
                sw.WriteLine("<th style='background-color: #eee'>RCP</th>");
            }
            if (vm.Param.ShowProbLevel)
            {
                sw.WriteLine("<th style='background-color: #eee'>Tk. Prob</th>");
            }
            if (vm.Param.ShowImpactLevel)
            {
                sw.WriteLine("<th style='background-color: #eee'>Tk. Dampak</th>");
            }
            sw.WriteLine("<th style='background-color: #eee'>Tk. Risiko</th>");
            if (vm.Param.ShowApprovedMitigations)
            {
                sw.WriteLine("<th style='background-color: #eee'>Mitigasi yg telah Di-approve</th>");
            }
            if (vm.Param.ShowPlannedMitigations)
            {
                sw.WriteLine("<th style='background-color: #eee'>Rencana Mitigasi</th>");
            }
            sw.WriteLine("</tr>");
            foreach (var item in vm.RiskList)
            {
                sw.WriteLine("<tr>");
                if (vm.Param.ShowRiskCode)
                {
                    sw.WriteLine(string.Format("<td>{0}</td>", item.Risk.RiskCode));
                }
                sw.WriteLine(string.Format("<td>{0}</td>", item.Risk.RiskName));
                if (vm.Param.ShowRiskDate)
                {
                    sw.WriteLine(string.Format("<td>{0:dd-MM-yyyy}</td>", item.Risk.RiskDate));
                }
                if (vm.Param.ShowOrg)
                { 
                    sw.WriteLine(string.Format("<td>{0}</td>", Utils.GetRiskOrgName(item.Risk)));
                }
                if (vm.Param.ShowRiskCat)
                {
                    sw.WriteLine("<td>");
                    if (item.Risk.RiskCat != null)
                    {
                        sw.WriteLine(item.Risk.RiskCat.RiskCatName);
                    }
                    sw.WriteLine("</td>");
                }
                if (vm.Param.ShowRiskCause)
                {
                    sw.WriteLine("<td>");
                    if (item.Risk.Cause != null)
                    {
                        sw.WriteLine(item.Risk.Cause.CauseName);
                    }
                    sw.WriteLine("</td>");
                }
                if (vm.Param.ShowRiskEffect)
                {
                    sw.WriteLine("<td>");
                    if (item.Risk.Effect != null)
                    {
                        sw.WriteLine(item.Risk.Effect.EffectName);
                    }
                    sw.WriteLine("</td>");
                }
                if (vm.Param.ShowRiskOwner)
                {
                    sw.WriteLine("<td>{0}</td>", item.Risk.UserInfo.FullName);
                }
                if (vm.Param.ShowProbLevel)
                {
                    sw.WriteLine("<td align='center'>{0}</td>", item.Risk.ProbLevelId);
                }
                if (vm.Param.ShowImpactLevel)
                {
                    sw.WriteLine("<td align='center'>{0}</td>", item.Risk.ImpactLevelId);
                }
                sw.WriteLine("<td align='center'>{0}</td>", item.Risk.RiskLevel);

                if (vm.Param.ShowApprovedMitigations)
                {
                    sw.WriteLine("<td valign='top' style='padding: 0'>");
                    if (item.ApprovedMitigations.Count() > 0)
                    {
                        sw.WriteLine("<table rules='all' border='1' style='border-collapse:collapse;'>");
                        sw.WriteLine("<tr>");
                        if (vm.Param.ShowRiskCode)
                        {
                            sw.WriteLine("<th style='background-color: #eee'>Kode Mitigasi</th>");
                        }
                        sw.WriteLine("<th style='background-color: #eee'>Uraian</th>");
                        if (vm.Param.ShowRiskDate)
                        {
                            sw.WriteLine("<th style='background-color: #eee'>Tanggal</th>");
                        }
                        if (vm.Param.ShowProbLevel)
                        {
                            sw.WriteLine("<th style='background-color: #eee'>Tk. Prob</th>");
                        }
                        if (vm.Param.ShowImpactLevel)
                        {
                            sw.WriteLine("<th style='background-color: #eee'>Tk. Dampak</th>");
                        }
                        sw.WriteLine("<th style='background-color: #eee'>Tk. Risiko</th>");
                        sw.WriteLine("</tr>");
                        foreach (var m in item.ApprovedMitigations)
                        {
                            sw.WriteLine("<tr>");
                            if (vm.Param.ShowRiskCode)
                            {
                                sw.WriteLine(string.Format("<td>{0}</td>", m.MitigationCode));
                            }
                            sw.WriteLine(string.Format("<td>{0}</td>", m.MitigationName));
                            if (vm.Param.ShowRiskDate)
                            {
                                sw.WriteLine(string.Format("<td>{0:dd-MM-yyyy}</td>", m.MitigationDate));
                            }
                            if (vm.Param.ShowProbLevel)
                            {
                                sw.WriteLine(string.Format("<td align='center'>{0}</td>", m.ProbLevelId));
                            }
                            if (vm.Param.ShowImpactLevel)
                            {
                                sw.WriteLine(string.Format("<td align='center'>{0}</td>", m.ImpactLevelId));
                            }
                            sw.WriteLine(string.Format("<td align='center'>{0}</td>", m.RiskLevel));
                            sw.WriteLine("</tr>");
                        }
                        sw.WriteLine("</table>");
                    }
                    sw.WriteLine("</td>");
                }

                if (vm.Param.ShowPlannedMitigations)
                {
                    sw.WriteLine("<td valign='top' style='padding: 0'>");
                    if (item.PlannedMitigations.Count() > 0)
                    {
                        sw.WriteLine("<table rules='all' border='1' style='border-collapse:collapse;'>");
                        sw.WriteLine("<tr>");
                        if (vm.Param.ShowRiskCode)
                        {
                            sw.WriteLine("<th style='background-color: #eee'>Kode Mitigasi</th>");
                        }
                        sw.WriteLine("<th style='background-color: #eee'>Uraian</th>");
                        if (vm.Param.ShowRiskDate)
                        {
                            sw.WriteLine("<th style='background-color: #eee'>Tanggal</th>");
                        }
                        if (vm.Param.ShowProbLevel)
                        {
                            sw.WriteLine("<th style='background-color: #eee'>Tk. Prob</th>");
                        }
                        if (vm.Param.ShowImpactLevel)
                        {
                            sw.WriteLine("<th style='background-color: #eee'>Tk. Dampak</th>");
                        }
                        sw.WriteLine("<th style='background-color: #eee'>Tk. Risiko</th>");
                        sw.WriteLine("</tr>");
                        foreach (var m in item.PlannedMitigations)
                        {
                            sw.WriteLine("<tr>");
                            if (vm.Param.ShowRiskCode)
                            {
                                sw.WriteLine(string.Format("<td>{0}</td>", m.MitigationCode));
                            }
                            sw.WriteLine(string.Format("<td>{0}</td>", m.MitigationName));
                            if (vm.Param.ShowRiskDate)
                            {
                                sw.WriteLine(string.Format("<td>{0:dd-MM-yyyy}</td>", m.MitigationDate));
                            }
                            if (vm.Param.ShowProbLevel)
                            {
                                sw.WriteLine(string.Format("<td align='center'>{0}</td>", m.ProbLevelId));
                            }
                            if (vm.Param.ShowImpactLevel)
                            {
                                sw.WriteLine(string.Format("<td align='center'>{0}</td>", m.ImpactLevelId));
                            }
                            sw.WriteLine(string.Format("<td align='center'>{0}</td>", m.RiskLevel));
                            sw.WriteLine("</tr>");
                        }
                        sw.WriteLine("</table>");
                    }
                    sw.WriteLine("</td>");
                }
                sw.WriteLine("</tr>");
            }
            sw.WriteLine("</table>");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=risk_registers.xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Write(sw.ToString());
            Response.End();
        }

        private void UpdateParam(Param param)
        {
            Dictionary<int, string> posList = new Dictionary<int, string>();
            posList.Add(1, "Kantor Pusat");
            posList.Add(2, "Cabang");
            param.PosList = new SelectList(posList, "Key", "Value", param.PosId);

            Dictionary<int, string> branchList = new Dictionary<int, string>();
            foreach (var branch in db.Branches.OrderBy(m => m.ClassId).ThenBy(m => m.BranchName))
                branchList.Add(branch.BranchId, branch.BranchName + " (Kelas " + branch.BranchClass.ClassName + ")");
            param.Branches = new SelectList(branchList, "Key", "Value", param.BranchId);

            Dictionary<int, string> levelList = new Dictionary<int, string>();
            levelList.Add(1, "x <= 5");
            levelList.Add(2, "5 < x <= 8");
            levelList.Add(3, "8 < x <= 12");
            levelList.Add(4, "12 < x <= 25");
            param.RiskLevels = new SelectList(levelList, "Key", "Value", param.RiskLevelId);
        }

        private void CalcRiskRegister(RiskRegisterViewModel vm)
        {
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
                risks = risks.Where(p => p.CloseDate == null || p.CloseDate > vm.Param.ReportDate);
                risks = risks.Where(p => p.ApprovalDate != null && p.ApprovalDate <= vm.Param.ReportDate);
            }
            else
            {
                risks = risks.Where(p => p.ApprovalDate == null && p.RiskDate <= vm.Param.ReportDate);
            }
            risks = risks.OrderBy(p => p.RiskDate);

            switch(vm.Param.RiskLevelId)
            {
                case 1: // x <= 5
                    risks = risks.Where(p => p.RiskLevel <= 5);
                    break;
                case 2: // 5 < x <= 8
                    risks = risks.Where(p => p.RiskLevel > 5 && p.RiskLevel <= 8);
                    break;
                case 3: // 8 < x <= 12
                    risks = risks.Where(p => p.RiskLevel > 8 && p.RiskLevel <= 12);
                    break;
                case 4: // 12 < x <= 25
                    risks = risks.Where(p => p.RiskLevel > 12 && p.RiskLevel <= 25);
                    break;
            }

            vm.RiskList = new List<RiskRecord>();
            foreach (var r in risks)
            {
                //if (vm.Param.IsApproved)
                //{
                //    var rm = db.RiskMitigations.Where(p => p.RiskId == r.RiskId && p.ApprovalDate != null && p.ApprovalDate <= vm.Param.ReportDate);
                //    if (rm.Count() > 0)
                //    {
                //        var m = rm.OrderByDescending(p => p.ApprovalDate).First();
                //        AddRiskToList(r, (int)m.ProbLevelId, (int)m.ImpactLevelId, vm.RiskList);
                //    }
                //    else
                //        vm.RiskList.Add(r);
                //}
                //else
                //    vm.RiskList.Add(r);
                RiskRecord rc = new RiskRecord();
                rc.Risk = r;
                rc.ApprovedMitigations = db.RiskMitigations.Where(p => p.RiskId == r.RiskId && p.ApprovalDate != null).ToList();
                rc.PlannedMitigations = db.RiskMitigations.Where(p => p.RiskId == r.RiskId && p.ApprovalDate == null).ToList();
                vm.RiskList.Add(rc);
            }
            //vm.RiskList = vm.RiskList.OrderByDescending(p => p.RiskLevel).ThenByDescending(p => p.ImpactLevelId).ThenByDescending(p => p.ProbLevelId).Take(10).ToList();
        }
    }
}
