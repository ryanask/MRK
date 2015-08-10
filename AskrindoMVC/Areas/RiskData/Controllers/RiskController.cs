using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using AskrindoMVC.Areas.RiskData.Models.RiskData;
using AskrindoMVC.Helpers;
using System.Transactions;
using System.Data;
using System.Text;
using System.IO;

namespace AskrindoMVC.Areas.RiskData.Controllers
{
    [Authorize]
    public class RiskController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RiskList()
        {
            UserData data = Utils.LoadUserDataFromSession();
            if (data.IsAdmin)
                return RedirectToAction("RiskListAdmin");
            else if (data.IsRiskOwner)
                return RedirectToAction("RiskListRO");
            else
                return RedirectToAction("RiskListNonRO", new { approved = false });
        }

        public ActionResult RiskListRO(string status)
        {
            UserData data = Utils.LoadUserDataFromSession();
            var risks = db.Risks.Where(p => p.UserId == data.UserId);
            if (string.IsNullOrEmpty(status)) 
            {
                risks = risks.Where(p => !p.IsReadOnly && p.ApprovalDate == null && p.CloseDate == null);
            }
            else if (status.ToLower() == "readonly")
            {
                risks = risks.Where(p => p.IsReadOnly && p.ApprovalDate == null && p.CloseDate == null);
            }
            else if (status.ToLower() == "approved")
            {
                risks = risks.Where(p => p.ApprovalDate != null && p.CloseDate == null);
            }
            else if (status.ToLower() == "closed")
            {
                risks = risks.Where(p => p.CloseDate != null);
            }
            return View(risks);
        }

        public ActionResult RiskListNonRO(bool approved)
        {
            UserData data = Utils.LoadUserDataFromSession();
            var nfo = db.UserInfos.Where(p => p.UserId == data.UserId).FirstOrDefault();
            IQueryable<RiskApproval> approvals = Enumerable.Empty<RiskApproval>().AsQueryable();
            approvals = db.RiskApprovals.Where(p => p.OrgPos == nfo.OrgPos);
            if (approved)
                approvals = approvals.Where(p => p.ApprovalDate != null);
            else
                approvals = approvals.Where(p => p.ApprovalDate == null);

            switch (nfo.OrgPos)
            {
                case Utils.ORGPOS_DEPT:
                    approvals = approvals.Where(p => p.DeptId == nfo.DeptId);
                    break;
                case Utils.ORGPOS_SUBDEPT:
                    approvals = approvals.Where(p => p.SubDeptId == nfo.SubDeptId);
                    break;
                case Utils.ORGPOS_DIVISION:
                    approvals = approvals.Where(p => p.DivisionId == nfo.DivisionId);
                    break;
                case Utils.ORGPOS_SUBDIV:
                    approvals = approvals.Where(p => p.SubDivId == nfo.SubDivId);
                    break;
                case Utils.ORGPOS_BRANCH:
                    approvals = approvals.Where(p => p.BranchId == nfo.BranchId);
                    break;
                case Utils.ORGPOS_SUBBRANCH:
                    approvals = approvals.Where(p => p.SubBranchId == nfo.SubBranchId);
                    break;
                case Utils.ORGPOS_BIZUNIT:
                    approvals = approvals.Where(p => p.BizUnitId == nfo.BizUnitId);
                    break;
            }
            return View(approvals);
        }

        public ActionResult RiskListAdmin()
        {
            RiskListParamViewModel vm = new RiskListParamViewModel();
            vm.PosId = 1;
            vm.StateId = 1;

            UpdateRiskListParam(vm);
            return View(vm);
        }

        [HttpPost]
        public ActionResult RiskListAdmin(RiskListParamViewModel vm)
        {
            UpdateRiskListParam(vm);
            return View(vm);
        }

        private void UpdateRiskListParam(RiskListParamViewModel vm)
        {
            Dictionary<int, string> posList = new Dictionary<int, string>();
            posList.Add(1, "Kantor Pusat");
            posList.Add(2, "Cabang");
            vm.PosList = new SelectList(posList, "Key", "Value", vm.PosId);

            Dictionary<int, string> branchList = new Dictionary<int, string>();
            foreach (var branch in db.Branches.OrderBy(m => m.ClassId).ThenBy(m => m.BranchName))
                branchList.Add(branch.BranchId, branch.BranchName + " (Kelas " + branch.BranchClass.ClassName + ")");
            vm.Branches = new SelectList(branchList, "Key", "Value", vm.BranchId);

            Dictionary<int, string> stateList = new Dictionary<int, string>();
            stateList.Add(1, "Belum di-approve");
            stateList.Add(2, "Dalam proses approval");
            stateList.Add(3, "Approved");
            stateList.Add(4, "Tutup");
            vm.States = new SelectList(stateList, "Key", "Value", vm.StateId);

            vm.Risks = db.Risks;
            if (vm.PosId == 1)
                vm.Risks = vm.Risks.Where(p => p.DeptId != null);
            else if (vm.PosId == 2)
            {
                vm.Risks = vm.Risks.Where(p => p.BranchId != null);
                if (vm.BranchId != null)
                    vm.Risks = vm.Risks.Where(p => p.BranchId == vm.BranchId);
            }

            switch (vm.StateId)
            {
                case 1:
                    vm.Risks = vm.Risks.Where(p => !p.IsReadOnly);
                    break;
                case 2:
                    vm.Risks = vm.Risks.Where(p => p.IsReadOnly && p.ApprovalDate == null);
                    break;
                case 3:
                    vm.Risks = vm.Risks.Where(p => p.ApprovalDate != null && p.CloseDate == null);
                    break;
                case 4:
                    vm.Risks = vm.Risks.Where(p => p.CloseDate != null);
                    break;
            }
            vm.Risks = vm.Risks.OrderBy(p => p.RiskDate);
        }

        public ActionResult CloseRisk(int id)
        {
            return View(db.Risks.Single(p => p.RiskId == id));
        }

        [HttpPost, ActionName("CloseRisk")]
        public ActionResult CloseRiskConfirmed(int id)
        {
            var risk = db.Risks.Single(p => p.RiskId == id);
            risk.CloseDate = DateTime.Now;
            db.SaveChanges();
            ViewBag.Message = "Data risiko telah ditutup";
            return View("Info");
        }

        public ActionResult CancelRiskClosure(int id)
        {
            return View(db.Risks.Single(p => p.RiskId == id));
        }

        [HttpPost, ActionName("CancelRiskClosure")]
        public ActionResult CancelRiskClosureConfirmed(int id)
        {
            var risk = db.Risks.Single(p => p.RiskId == id);
            risk.CloseDate = null;
            db.SaveChanges();
            ViewBag.Message = "Penutupan data risiko telah dibatalkan";
            return View("Info");
        }

        public ActionResult ApproveRisk(int id)
        {
            var apr = db.RiskApprovals.Where(p => p.ApprovalId == id).FirstOrDefault();
            return View(apr);
        }

        [HttpPost, ActionName("ApproveRisk")]
        public ActionResult ApproveRiskConfirmed(int id)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    UserData data = Utils.LoadUserDataFromSession();
                    RiskApproval apr = db.RiskApprovals.Where(p => p.ApprovalId == id).FirstOrDefault();
                    apr.ApprovalDate = DateTime.Now;
                    apr.UserId = data.UserId;
                    apr.JobTitle = data.JobTitle;
                    db.SaveChanges();

                    apr.Risk.IsReadOnly = true;
                    db.SaveChanges();

                    if (apr.LastApproval)
                    {
                        // risk approval completed
                        apr.Risk.ApprovalDate = apr.ApprovalDate;
                        db.SaveChanges();

                        RiskApproval prevApr = db.RiskApprovals
                            .Where(p => p.RiskId == apr.RiskId && p.ApprovalId != apr.ApprovalId && p.ApprovalDate != null)
                            .FirstOrDefault();
                        if (prevApr != null)
                        {
                            prevApr.IsReadOnly = true;
                            db.SaveChanges();
                        }

                        RiskState rs = new RiskState();
                        rs.RiskId = apr.RiskId;
                        rs.StateDate = (DateTime)apr.ApprovalDate;
                        rs.ProbLevelId = (int)apr.Risk.ProbLevelId;
                        rs.ImpactLevelId = (int)apr.Risk.ImpactLevelId;
                        rs.RiskLevel = (int)apr.Risk.RiskLevel;
                        db.RiskStates.AddObject(rs);
                        db.SaveChanges();
                    }
                    else
                    {
                        // next approval schedule
                        RiskApproval nextApr = new RiskApproval();
                        nextApr.RiskId = apr.RiskId;
                        nextApr.LimitDate = DateTime.Now.AddDays(Utils.MAX_LIMIT_APPROVAL_DAYS);
                        nextApr.LastApproval = true;

                        switch (apr.OrgPos)
                        {
                            case Utils.ORGPOS_SUBDIV:
                                SubDiv subDiv = db.SubDivs.Single(p => p.SubDivId == apr.SubDivId);
                                nextApr.OrgPos = Utils.ORGPOS_DIVISION;
                                nextApr.DivisionId = subDiv.DivisionId;
                                nextApr.OrgName = subDiv.Division.DivisionName;
                                break;
                            case Utils.ORGPOS_SUBBRANCH:
                                SubBranch subBranch = db.SubBranches.Single(p => p.SubBranchId == apr.SubBranchId);
                                nextApr.OrgPos = Utils.ORGPOS_BRANCH;
                                nextApr.BranchId = subBranch.BranchId;
                                nextApr.OrgName = subBranch.Branch.BranchName;
                                break;
                            case Utils.ORGPOS_BIZUNIT:
                                BizUnit biz = db.BizUnits.Single(p => p.BizUnitId == apr.BizUnitId);
                                nextApr.OrgPos = Utils.ORGPOS_BRANCH;
                                nextApr.BranchId = biz.BranchId;
                                nextApr.OrgName = biz.Branch.BranchName;
                                break;
                        }
                        db.RiskApprovals.AddObject(nextApr);
                        db.SaveChanges();
                    }
                    trans.Complete();
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Tidak bisa meng-approve data peristiwa risiko : " + e.Message;
                    return View("Error");
                }
            }
            return RedirectToAction("ApproveRiskSuccess");
        }

        public ActionResult ApproveRiskSuccess()
        {
            return View();
        }

        public ActionResult DisapproveRisk(int id)
        {
            var apr = db.RiskApprovals.Where(p => p.ApprovalId == id).FirstOrDefault();
            return View(apr);
        }

        [HttpPost, ActionName("DisapproveRisk")]
        public ActionResult DisapproveRiskConfirmed(int id)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    RiskApproval apr = db.RiskApprovals.Where(p => p.ApprovalId == id).FirstOrDefault();

                    RiskApproval nextApr = db.RiskApprovals
                        .Where(p => p.RiskId == apr.RiskId && p.ApprovalDate == null && p.ApprovalId != apr.ApprovalId).FirstOrDefault();
                    if (nextApr != null)
                        db.RiskApprovals.DeleteObject(nextApr);

                    apr.UserId = null;
                    apr.JobTitle = null;
                    apr.ApprovalDate = null;
                    apr.IsReadOnly = false;

                    if (apr.LastApproval)
                    {
                        apr.Risk.ApprovalDate = null;
                        RiskState rs = db.RiskStates.Where(p => p.RiskId == apr.RiskId && p.MitigationId == null).FirstOrDefault();
                        if (rs != null)
                            db.RiskStates.DeleteObject(rs);
                    }

                    RiskApproval prevApr = db.RiskApprovals
                        .Where(p => p.RiskId == apr.RiskId && p.ApprovalDate != null && p.ApprovalId != apr.ApprovalId).FirstOrDefault();
                    if (prevApr == null)
                        apr.Risk.IsReadOnly = false;
                    else
                        prevApr.IsReadOnly = false;

                    db.SaveChanges();
                    trans.Complete();
                }
                catch
                {
                    ViewBag.Message = "Tidak bisa membatalkan approval data peristiwa risiko";
                    return View("Error");
                }
            }
            return RedirectToAction("DispproveRiskSuccess");
        }

        public ActionResult DispproveRiskSuccess()
        {
            return View();
        }

        public ActionResult RiskDetail(int id)
        {
            var risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            if (risk == null)
            {
                ViewBag.Message = "Data risiko tidak ditemukan";
                return View("RiskMessage");
            }
            return View(risk);
        }

        public ActionResult RiskNew()
        {
            UserData data = Utils.LoadUserDataFromSession();
            if (data.IsAdmin || !data.IsRiskOwner)
            {
                //ViewBag.Message = "Hanya Risk Owner yang bisa meng-input data peristiwa risiko";
                return View("NewRiskError");
            }

            Random rand = new Random();
            RiskEditViewModel vm = new RiskEditViewModel();
            vm.Risk = new Risk();
            vm.Risk.RiskCode = Utils.GetFormattedSerialNumber(data);

            vm.Risk.RiskDate = DateTime.Now;

            vm.CauseGroups = new SelectList(db.CauseGroups, "CauseGroupId", "CauseGroupName");
            vm.CauseTypes = new SelectList(Enumerable.Empty<SelectListItem>(), "CauseTypeId", "CauseTypeName");
            vm.Causes = new SelectList(Enumerable.Empty<SelectListItem>(), "CauseId", "CauseName");
            
            vm.EffectGroups = new SelectList(db.EffectGroups, "EffectGroupId", "EffectGroupName");
            vm.EffectTypes = new SelectList(Enumerable.Empty<SelectListItem>(), "EffectTypeId", "EffectTypeName");
            vm.Effects = new SelectList(Enumerable.Empty<SelectListItem>(), "EffectId", "EffectName");

            vm.RiskCats = new SelectList(db.RiskCats, "RiskCatId", "RiskCatName");
            vm.RiskGroups = new SelectList(Enumerable.Empty<SelectListItem>(), "RiskGroupId", "RiskGroupName");
            vm.RiskTypes = new SelectList(Enumerable.Empty<SelectListItem>(), "RiskTypeId", "RiskTypeName");

            return View(vm);
        }

        [HttpPost]
        public ActionResult RiskNew(RiskEditViewModel vm)
        {
            UserData data = Utils.LoadUserDataFromSession();
            bool saved = true;
            if (ModelState.IsValid)
            {
                if (vm.Risk.CauseGroupId != null && vm.Risk.CauseId == null)
                {
                    ModelState.AddModelError("", "Sebab Risiko harus diisi lengkap atau kosong sama sekali");
                    saved = false;
                }
                if (vm.Risk.EffectGroupId != null && vm.Risk.EffectId == null)
                {
                    ModelState.AddModelError("", "Akibat Risiko harus diisi lengkap atau kosong sama sekali");
                    saved = false;
                }
                if (vm.Risk.RiskCatId != null && vm.Risk.RiskTypeId == null)
                {
                    ModelState.AddModelError("", "Klasifikasi Risiko harus diisi lengkap atau kosong sama sekali");
                    saved = false;
                }

                if (saved)
                {
                    try
                    {
                        using (TransactionScope trans = new TransactionScope())
                        {
                            Risk risk = new Risk();
                            risk.UserId = data.UserId;
                            risk.JobTitle = data.JobTitle;
                            risk.RiskCode = Utils.GetFormattedSerialNumber(data);
                            risk.RiskName = vm.Risk.RiskName;
                            risk.RiskDate = vm.Risk.RiskDate;
                            risk.OrgPos = data.OrgPos;
                            risk.DeptId = data.DeptId;
                            risk.SubDeptId = data.SubDeptId;
                            risk.DivisionId = data.DivisionId;
                            risk.SubDivId = data.SubDivId;
                            risk.BranchId = data.BranchId;
                            risk.SubBranchId = data.SubBranchId;
                            risk.BizUnitId = data.BizUnitId;
                            if (vm.Risk.CauseId != null)
                            {
                                risk.CauseGroupId = vm.Risk.CauseGroupId;
                                risk.CauseTypeId = vm.Risk.CauseTypeId;
                                risk.CauseId = vm.Risk.CauseId;
                            }
                            if (vm.Risk.EffectId != null)
                            {
                                risk.EffectGroupId = vm.Risk.EffectGroupId;
                                risk.EffectTypeId = vm.Risk.EffectTypeId;
                                risk.EffectId = vm.Risk.EffectId;
                            }
                            if (vm.Risk.RiskTypeId != null)
                            {
                                risk.RiskCatId = vm.Risk.RiskCatId;
                                risk.RiskGroupId = vm.Risk.RiskGroupId;
                                risk.RiskTypeId = vm.Risk.RiskTypeId;
                            }
                            risk.ProbLevelId = Utils.PROBLEVEL1;
                            risk.ImpactLevelId = Utils.IMPACTLEVEL1;
                            risk.RiskLevel = risk.ProbLevelId * risk.ImpactLevelId;
                            risk.IsReadOnly = false;
                            db.Risks.AddObject(risk);
                            db.SaveChanges();

                            RiskProb prob = new RiskProb();
                            prob.RiskId = risk.RiskId;
                            prob.ProbOption = Utils.PROBOPTION_FREQUENCY;
                            prob.FreqId = Utils.FREQUENCY1;
                            prob.ProbLevelId = Utils.PROBLEVEL1;
                            db.RiskProbs.AddObject(prob);
                            db.SaveChanges();

                            RiskImpact impact = new RiskImpact();
                            impact.RiskId = risk.RiskId;
                            impact.IsMoneyImpact = true;
                            impact.ImpactLevelId = Utils.IMPACTLEVEL1;
                            db.RiskImpacts.AddObject(impact);
                            db.SaveChanges();

                            Utils.CreateFirstApprovalSchedule(risk.RiskId);
                            Utils.IncrementSerialNumber(db);

                            trans.Complete();
                            return RedirectToAction("RiskDetail", new { id = risk.RiskId });
                        }
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Tidak bisa menyimpan data risiko. Error: " + e.Message);
                    }
                }
            }

            vm.Risk.RiskCode = Utils.GetFormattedSerialNumber(data);

            vm.CauseGroups = new SelectList(db.CauseGroups, "CauseGroupId", "CauseGroupName");
            vm.CauseTypes = new SelectList(db.CauseTypes.Where(p => p.CauseGroupId == vm.Risk.CauseGroupId), "CauseTypeId", "CauseTypeName");
            vm.Causes = new SelectList(db.Causes.Where(p => p.CauseTypeId == vm.Risk.CauseTypeId), "CauseId", "CauseName");

            vm.EffectGroups = new SelectList(db.EffectGroups, "EffectGroupId", "EffectGroupName");
            vm.EffectTypes = new SelectList(db.EffectTypes.Where(p => p.EffectGroupId == vm.Risk.EffectGroupId), "EffectTypeId", "EffectTypeName");
            vm.Effects = new SelectList(db.Effects.Where(p => p.EffectTypeId == vm.Risk.EffectTypeId), "EffectId", "EffectName");

            vm.RiskCats = new SelectList(db.RiskCats, "RiskCatId", "RiskCatName");
            vm.RiskGroups = new SelectList(db.RiskGroups.Where(p => p.RiskCatId == vm.Risk.RiskCatId), "RiskGroupId", "RiskGroupName");
            vm.RiskTypes = new SelectList(db.RiskTypes.Where(p => p.RiskGroupId == vm.Risk.RiskGroupId), "RiskTypeId", "RiskTypeName");

            return View(vm);
        }

        public ActionResult RiskEdit(int id)
        {
            RiskEditViewModel vm = new RiskEditViewModel();
            vm.Risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();

            vm.CauseGroups = new SelectList(db.CauseGroups, "CauseGroupId", "CauseGroupName", vm.Risk.CauseGroupId);
            vm.CauseTypes = new SelectList(db.CauseTypes.Where(p => p.CauseGroupId == vm.Risk.CauseGroupId), "CauseTypeId", "CauseTypeName", vm.Risk.CauseTypeId);
            vm.Causes = new SelectList(db.Causes.Where(p => p.CauseTypeId == vm.Risk.CauseTypeId), "CauseId", "CauseName", vm.Risk.CauseId);

            vm.EffectGroups = new SelectList(db.EffectGroups, "EffectGroupId", "EffectGroupName", vm.Risk.EffectGroupId);
            vm.EffectTypes = new SelectList(db.EffectTypes.Where(p => p.EffectGroupId == vm.Risk.EffectGroupId), "EffectTypeId", "EffectTypeName", vm.Risk.EffectTypeId);
            vm.Effects = new SelectList(db.Effects.Where(p => p.EffectTypeId == vm.Risk.EffectTypeId), "EffectId", "EffectName", vm.Risk.EffectId);

            vm.RiskCats = new SelectList(db.RiskCats, "RiskCatId", "RiskCatName", vm.Risk.RiskCatId);
            vm.RiskGroups = new SelectList(db.RiskGroups.Where(p => p.RiskCatId == vm.Risk.RiskCatId), "RiskGroupId", "RiskGroupName", vm.Risk.RiskGroupId);
            vm.RiskTypes = new SelectList(db.RiskTypes.Where(p => p.RiskGroupId == vm.Risk.RiskGroupId), "RiskTypeId", "RiskTypeName", vm.Risk.RiskTypeId);

            return View(vm);
        }

        [HttpPost]
        public ActionResult RiskEdit(RiskEditViewModel vm, int id)
        {
            bool saved = true;
            if (ModelState.IsValid)
            {
                if (vm.Risk.CauseGroupId != null && vm.Risk.CauseId == null)
                {
                    ModelState.AddModelError("", "Sebab Risiko harus diisi lengkap atau kosong sama sekali");
                    saved = false;
                }
                if (vm.Risk.EffectGroupId != null && vm.Risk.EffectId == null)
                {
                    ModelState.AddModelError("", "Akibat Risiko harus diisi lengkap atau kosong sama sekali");
                    saved = false;
                }
                if (vm.Risk.RiskCatId != null && vm.Risk.RiskTypeId == null)
                {
                    ModelState.AddModelError("", "Klasifikasi Risiko harus diisi lengkap atau kosong sama sekali");
                    saved = false;
                }

                if (saved)
                {
                    db.Risks.Attach(vm.Risk);
                    db.ObjectStateManager.ChangeObjectState(vm.Risk, EntityState.Modified);
                    db.SaveChanges();
                    return RedirectToAction("RiskDetail", new { id = id });
                }
            }

            vm.CauseGroups = new SelectList(db.CauseGroups, "CauseGroupId", "CauseGroupName", vm.Risk.CauseGroupId);
            vm.CauseTypes = new SelectList(db.CauseTypes.Where(p => p.CauseGroupId == vm.Risk.CauseGroupId), "CauseTypeId", "CauseTypeName", vm.Risk.CauseTypeId);
            vm.Causes = new SelectList(db.Causes.Where(p => p.CauseTypeId == vm.Risk.CauseTypeId), "CauseId", "CauseName", vm.Risk.CauseId);

            vm.EffectGroups = new SelectList(db.EffectGroups, "EffectGroupId", "EffectGroupName", vm.Risk.EffectGroupId);
            vm.EffectTypes = new SelectList(db.EffectTypes.Where(p => p.EffectGroupId == vm.Risk.EffectGroupId), "EffectTypeId", "EffectTypeName", vm.Risk.EffectTypeId);
            vm.Effects = new SelectList(db.Effects.Where(p => p.EffectTypeId == vm.Risk.EffectTypeId), "EffectId", "EffectName", vm.Risk.EffectId);

            vm.RiskCats = new SelectList(db.RiskCats, "RiskCatId", "RiskCatName", vm.Risk.RiskCatId);
            vm.RiskGroups = new SelectList(db.RiskGroups.Where(p => p.RiskCatId == vm.Risk.RiskCatId), "RiskGroupId", "RiskGroupName", vm.Risk.RiskGroupId);
            vm.RiskTypes = new SelectList(db.RiskTypes.Where(p => p.RiskGroupId == vm.Risk.RiskGroupId), "RiskTypeId", "RiskTypeName", vm.Risk.RiskTypeId);

            return View(vm);
        }

        public ActionResult RiskDelete(int id)
        {
            var risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            if (risk.IsReadOnly)
            {
                ViewBag.Message = "Tidak bisa menghapus data risiko";
                return View("Error");
            }
            
            return View(risk);
        }

        [HttpPost, ActionName("RiskDelete")]
        public ActionResult RiskDeleteConfirmed(int id)
        {   
            var risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            db.Risks.DeleteObject(risk);
            db.SaveChanges();
            return RedirectToAction("RiskList");
        }

        public ActionResult ProbDetail(int id)
        {
            RiskProb prob = db.RiskProbs.Include("Risk").Where(p => p.RiskId == id).FirstOrDefault();
            if (prob == null)
            {
                prob = new RiskProb();
                prob.RiskId = id;
                prob.ProbOption = Utils.PROBOPTION_FREQUENCY;
                prob.FreqId = Utils.FREQUENCY1;
                prob.ProbLevelId = Utils.PROBLEVEL1;
                db.RiskProbs.AddObject(prob);
                db.SaveChanges();

                prob.Risk.ProbLevelId = Utils.PROBLEVEL1;
                Utils.CalcRiskLevel(prob.Risk);
                db.SaveChanges();
            }
            return View(prob);
        }

        public ActionResult ProbEdit(int id)
        {
            RiskProb prob = db.RiskProbs.Where(p => p.RiskId == id).FirstOrDefault();
            ViewBag.Freqs = new SelectList(db.Freqs.ToList(), "FreqId", "FreqName", prob.FreqId);
            return View(prob);
        }

        [HttpPost]
        public ActionResult ProbEdit(RiskProb prob, int id)
        {
            if (ModelState.IsValid)
            {
                bool isOk = true;
                double? value = null;
                int probLevelId;

                switch (prob.ProbOption)
                {
                    case Utils.PROBOPTION_POISSON:
                        isOk = prob.Poisson1 != null && prob.Poisson2 != null;
                        if (isOk)
                        {
                            value = (1 - poissondistr.poissondistribution(0, (double)prob.Poisson1 / (double)prob.Poisson2)) * 100;
                            prob.Binom1 = null;
                            prob.Binom2 = null;
                            prob.Approx1 = null;
                            prob.Approx2 = null;
                            prob.Approx3 = null;
                            prob.Compare = null;
                            prob.FreqId = null;
                        }
                        break;
                    case Utils.PROBOPTION_BINOMIAL:
                        isOk = prob.Binom1 != null && prob.Binom2 != null;
                        if (isOk)
                        {
                            value = (double)(1 - binomialdistr.binomialdistribution(0, (int)prob.Binom2, (double)prob.Binom1 / (double)prob.Binom2)) * 100;
                            prob.Poisson1 = null;
                            prob.Poisson2 = null;
                            prob.Approx1 = null;
                            prob.Approx2 = null;
                            prob.Approx3 = null;
                            prob.Compare = null;
                            prob.FreqId = null;
                        }
                        break;
                    case Utils.PROBOPTION_APPROXIMATION:
                        isOk = prob.Approx1 != null && prob.Approx2 != null && prob.Approx3 != null;
                        if (isOk)
                        {
                            value = ((double)prob.Approx1 + 4 * (double)prob.Approx2 + (double)prob.Approx3) / 6;
                            prob.Poisson1 = null;
                            prob.Poisson2 = null;
                            prob.Binom1 = null;
                            prob.Binom2 = null;
                            prob.Compare = null;
                            prob.FreqId = null;
                        }
                        break;
                    case Utils.PROBOPTION_COMPARISON:
                        isOk = prob.Compare != null;
                        if (isOk)
                        {
                            value = (double)prob.Compare;
                            prob.Poisson1 = null;
                            prob.Poisson2 = null;
                            prob.Binom1 = null;
                            prob.Binom2 = null;
                            prob.Approx1 = null;
                            prob.Approx2 = null;
                            prob.Approx3 = null;
                            prob.FreqId = null;
                        }
                        break;
                    case Utils.PROBOPTION_FREQUENCY:
                        isOk = prob.FreqId != null;
                        if (isOk)
                        {
                            prob.Poisson1 = null;
                            prob.Poisson2 = null;
                            prob.Binom1 = null;
                            prob.Binom2 = null;
                            prob.Approx1 = null;
                            prob.Approx2 = null;
                            prob.Approx3 = null;
                            prob.Compare = null;
                        }
                        break;
                }

                if (isOk)
                {
                    decimal? probValue = null;
                    if (value != null)
                    {
                        probValue = Convert.ToDecimal(value);
                        probLevelId = Utils.GetProbLevelFromValue((decimal)probValue);
                    }
                    else
                        probLevelId = (int)prob.FreqId;

                    prob.ProbValue = probValue;
                    prob.ProbLevelId = probLevelId;

                    db.RiskProbs.Attach(prob);
                    db.ObjectStateManager.ChangeObjectState(prob, EntityState.Modified);

                    Risk risk = db.Risks.Where(p => p.RiskId == id).SingleOrDefault();
                    risk.ProbValue = prob.ProbValue;
                    risk.ProbLevelId = prob.ProbLevelId;
                    Utils.CalcRiskLevel(risk);
                    db.SaveChanges();

                    return RedirectToAction("ProbDetail", new { id = id });
                }
                else
                {
                    ModelState.AddModelError("", "Parameter untuk menghitung probabilitas tidak lengkap");
                    ViewBag.Freqs = new SelectList(db.Freqs.ToList(), "FreqId", "FreqName", prob.FreqId);
                    return View(prob);
                }
            }
            
            return RedirectToAction("ProbDetail", new { id = id });
        }

        public ActionResult ImpactDetail(int id)
        {
            RiskImpact impact = db.RiskImpacts.Where(p=>p.RiskId==id).FirstOrDefault();
            if (impact == null)
            {
                impact = new RiskImpact();
                impact.RiskId = id;
                impact.IsMoneyImpact = true;
                impact.ImpactLevelId = Utils.IMPACTLEVEL1;
                db.RiskImpacts.AddObject(impact);
                db.SaveChanges();

                impact.Risk.ImpactLevelId = Utils.IMPACTLEVEL1;
                if (impact.Risk.ProbLevelId != null)
                    impact.Risk.RiskLevel = (int)impact.Risk.ProbLevelId * (int)impact.Risk.ImpactLevelId;
                db.SaveChanges();
            }
            RiskImpactModelView vm = new RiskImpactModelView();
            vm.RiskImpact = impact;
            vm.RiskNonMoneyImpacts = db.RiskNonMoneyImpacts.Where(p => p.RiskId == id);
            return View(vm);
        }

        public ActionResult ImpactEdit(int id)
        {
            RiskImpact impact = db.RiskImpacts.Where(p => p.RiskId == id).FirstOrDefault();
            return View(impact);
        }

        [HttpPost]
        public ActionResult ImpactEdit(RiskImpact impact, int id)
        {
            if (ModelState.IsValid)
            {
                RiskImpact oldImpact = db.RiskImpacts.Where(p => p.RiskId == id).FirstOrDefault();
                if (oldImpact.IsMoneyImpact != impact.IsMoneyImpact)
                {
                    using (TransactionScope trans = new TransactionScope())
                    {
                        try
                        {
                            oldImpact.IsMoneyImpact = impact.IsMoneyImpact;
                            oldImpact.MoneyDirect = null;
                            oldImpact.MoneyIndirect = null;
                            oldImpact.ImpactLevelId = Utils.IMPACTLEVEL1;

                            if (impact.IsMoneyImpact)
                            {
                                var nonMoneys = db.RiskNonMoneyImpacts.Where(p => p.RiskId == id).ToList();
                                foreach (var item in nonMoneys)
                                    db.RiskNonMoneyImpacts.DeleteObject(item);
                            }

                            Risk risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
                            risk.ImpactLevelId = Utils.IMPACTLEVEL1;
                            risk.ImpactMoney = null;
                            Utils.CalcRiskLevel(risk);

                            db.SaveChanges();
                            trans.Complete();
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", "Tidak bisa menyimpan data. Error: " + e.Message);
                            return View(impact);
                        }
                    }
                }

                if (impact.IsMoneyImpact)
                    return RedirectToAction("ImpactMoneyEdit", new { id = id });
                else
                    return RedirectToAction("ImpactNonMoneyList", new { id = id });
            }
            return View(impact);
        }

        public ActionResult ImpactMoneyEdit(int id)
        {
            RiskImpact impact = db.RiskImpacts.Where(p => p.RiskId == id).FirstOrDefault();
            return View(impact);
        }

        [HttpPost]
        public ActionResult ImpactMoneyEdit(RiskImpact impact, int id)
        {
            if (ModelState.IsValid)
            {
                UserData data = Utils.LoadUserDataFromSession();
                using (TransactionScope trans = new TransactionScope())
                {
                    try
                    {
                        decimal moneyValue = 0M;
                        if (impact.MoneyDirect != null)
                            moneyValue += (decimal)impact.MoneyDirect;
                        if (impact.MoneyIndirect != null)
                            moneyValue += (decimal)impact.MoneyIndirect;
                        
                        var impactPos = Utils.GetImpactPos(data);
                        var level = Utils.GetImpactLevelFromMoney(moneyValue, impactPos);

                        impact.ImpactLevelId = level;

                        db.RiskImpacts.Attach(impact);
                        db.ObjectStateManager.ChangeObjectState(impact, EntityState.Modified);
                        db.SaveChanges();

                        var risk = db.Risks.Single(p => p.RiskId == id);
                        risk.ImpactMoney = moneyValue;
                        risk.ImpactLevelId = level;
                        Utils.CalcRiskLevel(risk);
                        db.SaveChanges();
                        trans.Complete();
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Tidak bisa menyimpan data. Error: " + e.Message);
                        return View(impact);
                    }

                    return RedirectToAction("ImpactDetail", new { id = id });
                }

            }
            return View(impact);
        }

        public ActionResult ImpactNonMoneyList(int id)
        {
            RiskImpactModelView vm = new RiskImpactModelView();
            vm.RiskImpact = db.RiskImpacts.Where(p => p.RiskId == id).FirstOrDefault();
            vm.RiskNonMoneyImpacts = db.RiskNonMoneyImpacts.Where(p => p.RiskId == id);
            return View(vm);
        }

        public ActionResult ImpactNonMoneySelect(int id)
        {
            ImpactSelectViewModel vm = new ImpactSelectViewModel();
            vm.RiskId = id;
            vm.Risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            vm.ImpactCats = db.ImpactCats;
            vm.ImpactLevels = db.ImpactLevels;
            vm.ImpactDetails = db.ImpactDetails.Where(p => p.ImpactTypeId == vm.ImpactTypeId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ImpactNonMoneySelect(ImpactSelectViewModel vm, int id)
        {
            if (vm.ImpactTypeId != 0 && vm.ImpactLeveliId != 0)
            {
                ImpactDetail detail = db.ImpactDetails.
                    Where(p => p.ImpactTypeId == vm.ImpactTypeId && 
                               p.ImpactLevelId == vm.ImpactLeveliId).FirstOrDefault();
                RiskNonMoneyImpact nmImpact = db.RiskNonMoneyImpacts.
                    Where(p => p.RiskId == id && 
                               p.ImpactTypeId == vm.ImpactTypeId).FirstOrDefault();
                if (nmImpact == null)
                {
                    nmImpact = new RiskNonMoneyImpact();
                    nmImpact.RiskId = id;
                    nmImpact.ImpactDetailId = detail.ImpactDetailId;
                    nmImpact.ImpactTypeId = vm.ImpactTypeId;
                    nmImpact.ImpactLevelId = vm.ImpactLeveliId;
                    db.RiskNonMoneyImpacts.AddObject(nmImpact);
                }
                else
                {
                    nmImpact.ImpactDetailId = detail.ImpactDetailId;
                    nmImpact.ImpactLevelId = vm.ImpactLeveliId;
                }
                db.SaveChanges();

                var maxLevelId = db.RiskNonMoneyImpacts.Where(p => p.RiskId == id).Max(p => p.ImpactLevelId);
                var risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
                risk.ImpactLevelId = maxLevelId;
                Utils.CalcRiskLevel(risk);
                db.SaveChanges();
                return RedirectToAction("ImpactNonMoneyList", new { id = id });
            }
            if (vm.ImpactTypeId == 0)
                ModelState.AddModelError("", "Dampak harus diisi");
            if (vm.ImpactLeveliId == 0)
                ModelState.AddModelError("", "Tingkat Dampak harus diisi");
            vm.Risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            vm.ImpactCats = db.ImpactCats;
            vm.ImpactLevels = db.ImpactLevels;
            vm.ImpactDetails = db.ImpactDetails.Where(p => p.ImpactTypeId == vm.ImpactTypeId);
            return View(vm);
        }

        public ActionResult ImpactNonMoneyDelete(int id)
        {
            var impact = db.RiskNonMoneyImpacts.Where(p => p.Id == id).FirstOrDefault();
            return View(impact);
        }

        [HttpPost, ActionName("ImpactNonMoneyDelete")]
        public ActionResult ImpactNonMoneyDeleteConfirmed(int id)
        {
            var impact = db.RiskNonMoneyImpacts.Where(p => p.Id == id).FirstOrDefault();
            var riskId = impact.RiskId;
            db.RiskNonMoneyImpacts.DeleteObject(impact);
            db.SaveChanges();
            return RedirectToAction("ImpactNonMoneyList", new { id = riskId });
        }

        public ActionResult AttachmentList(int id)
        {
            RiskAttachmentViewModel vm = new RiskAttachmentViewModel();
            vm.Risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            vm.RiskAttachments = db.RiskAttachments.Where(p => p.RiskId == id);
            return View(vm);
        }

        public ActionResult AttachmentNew(int id)
        {
            RiskAttachmentViewModel vm = new RiskAttachmentViewModel();
            vm.Risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            vm.RiskAttachment = new RiskAttachment();
            vm.RiskAttachment.RiskId = id;
            return View(vm);
        }

        [HttpPost]
        public ActionResult AttachmentNew(HttpPostedFileBase file, RiskAttachmentViewModel vm)
        {
            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("", "File yang akan di-upload harus diisi");
                vm.Risk = db.Risks.Where(p => p.RiskId == vm.RiskAttachment.RiskId).FirstOrDefault();
                return View(vm);
            }
            if (ModelState.IsValid)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] fileData = ms.GetBuffer();

                    var attach = new RiskAttachment();
                    attach.RiskId = vm.RiskAttachment.RiskId;
                    attach.AttachName = vm.RiskAttachment.AttachName;
                    attach.Notes = vm.RiskAttachment.Notes;
                    attach.Filename = file.FileName;
                    attach.ContentType = file.ContentType;
                    attach.ContentLength = file.ContentLength;
                    attach.Data = fileData;
                    db.RiskAttachments.AddObject(attach);
                    db.SaveChanges();
                    return RedirectToAction("AttachmentList", new { id = vm.RiskAttachment.RiskId });
                }
            }
            vm.Risk = db.Risks.Where(p => p.RiskId == vm.RiskAttachment.RiskId).FirstOrDefault();
            return View(vm);
        }

        public ActionResult AttachmentEdit(int id)
        {
            RiskAttachmentViewModel vm = new RiskAttachmentViewModel();
            vm.RiskAttachment = db.RiskAttachments.Where(p => p.AttachId == id).FirstOrDefault();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AttachmentEdit(HttpPostedFileBase file, RiskAttachmentViewModel vm)
        {
            if (ModelState.IsValid)
            {
                RiskAttachment attach = db.RiskAttachments.Where(p => p.AttachId == vm.RiskAttachment.AttachId).FirstOrDefault();
                attach.AttachName = vm.RiskAttachment.AttachName;
                attach.Notes = vm.RiskAttachment.Notes;

                if (file != null && file.ContentLength > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] fileData = ms.GetBuffer();
                        attach.Filename = file.FileName;
                        attach.ContentType = file.ContentType;
                        attach.ContentLength = file.ContentLength;
                        attach.Data = fileData;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("AttachmentList", new { id = vm.RiskAttachment.RiskId });
            }
            return View(vm);
        }

        public ActionResult AttachmentDelete(int id)
        {
            RiskAttachment attach = db.RiskAttachments.Where(p => p.AttachId == id).FirstOrDefault();
            return View(attach);
        }

        [HttpPost, ActionName("AttachmentDelete")]
        public ActionResult AttachmentDeleteConfirmed(int id)
        {
            var attach = db.RiskAttachments.Where(p => p.AttachId == id).FirstOrDefault();
            var riskId = attach.RiskId;
            db.RiskAttachments.DeleteObject(attach);
            db.SaveChanges();
            return RedirectToAction("AttachmentList", new { id = riskId });
        }

        public FileContentResult AttachmentDownload(int id)
        {
            byte[] fileData;
            var attach = db.RiskAttachments.Where(p => p.AttachId == id).FirstOrDefault();
            fileData = attach.Data.ToArray();
            return File(fileData, attach.ContentType, attach.Filename);
        }

        [HttpGet]
        public ActionResult LoadImpactDetails(int impactTypeId)
        {
            var details = db.ImpactDetails.Where(p => p.ImpactTypeId == impactTypeId);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='list'>");
            sb.Append("<tr>");
            sb.Append("<th colspan='2'>Level</th>");
            sb.Append("<th>Keterangan</th>");
            sb.Append("</tr>");
            foreach (var detail in details)
            {
                sb.Append("<tr>");
                sb.Append(string.Format("<td style='width: 18px'>{0} -</td>", detail.ImpactLevelId));
                sb.Append(string.Format("<td>{0}</td>", detail.ImpactLevel.ImpactLevelName));
                sb.Append(string.Format("<td>{0}</td>", detail.ImpactDetailName));
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return Content(sb.ToString());
        }

        public ActionResult MitigationList(int id, bool approved)
        {
            UserData data = Utils.LoadUserDataFromSession();
            if (data.IsAdmin)
                return RedirectToAction("MitigationListAdmin", new { id = id, approved = approved });
            else if (data.IsRiskOwner)
                return RedirectToAction("MitigationListRO", new { id = id });
            else
                return RedirectToAction("MitigationListNonRO", new { id = id, approved = approved });
        }

        public ActionResult MitigationListRO(int id, string status)
        {
            RiskMitigationViewModel vm = new RiskMitigationViewModel();
            vm.Risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            vm.RiskMitigations = db.RiskMitigations.Where(p => p.RiskId == id);

            if (string.IsNullOrEmpty(status))
                vm.RiskMitigations = vm.RiskMitigations.Where(p => !p.IsReadOnly && p.ApprovalDate == null);
            else if (status.ToLower() == "readonly")
                vm.RiskMitigations = vm.RiskMitigations.Where(p => p.IsReadOnly && p.ApprovalDate == null);
            else if (status.ToLower() == "approved")
                vm.RiskMitigations = vm.RiskMitigations.Where(p => p.ApprovalDate != null);

            return View(vm);
        }

        public ActionResult MitigationListNonRO(int id, bool approved)
        {
            UserData data = Utils.LoadUserDataFromSession();

            RiskMitigationViewModel vm = new RiskMitigationViewModel();
            vm.Risk = db.Risks.Where(p => p.RiskId == id).FirstOrDefault();
            vm.MitigationApprovals = db.MitigationApprovals.Where(p => p.OrgPos == data.OrgPos && p.RiskMitigation.RiskId == id);
            if (approved)
                vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.ApprovalDate != null);
            else
                vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.ApprovalDate == null);

            switch (data.OrgPos)
            {
                case Utils.ORGPOS_DEPT:
                    vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.DeptId == data.DeptId);
                    break;
                case Utils.ORGPOS_SUBDEPT:
                    vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.SubDeptId == data.SubDeptId);
                    break;
                case Utils.ORGPOS_DIVISION:
                    vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.DivisionId == data.DivisionId);
                    break;
                case Utils.ORGPOS_SUBDIV:
                    vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.SubDivId == data.SubDivId);
                    break;
                case Utils.ORGPOS_BRANCH:
                    vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.BranchId == data.BranchId);
                    break;
                case Utils.ORGPOS_SUBBRANCH:
                    vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.SubBranchId == data.SubBranchId);
                    break;
                case Utils.ORGPOS_BIZUNIT:
                    vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.BizUnitId == data.BizUnitId);
                    break;
            }

            return View(vm);
        }
        
        public ActionResult MitigationNew(int riskId)
        {
            RiskMitigationViewModel vm = new RiskMitigationViewModel();
            vm.Risk = db.Risks.Where(p => p.RiskId == riskId).FirstOrDefault();

            vm.MitigationCats = new SelectList(db.MitigationCats, "MitigationCatId", "MitigationCatName");
            vm.MitigationTypes = new SelectList(Enumerable.Empty<SelectListItem>(), "MitigationTypeId", "MitigationTypeName");

            Dictionary<int, string> probList = new Dictionary<int, string>();
            foreach (var item in db.ProbLevels)
                probList.Add(item.ProbLevelId, item.ProbLevelId + " - " + item.ProbLevelName);
            vm.ProbLevels = new SelectList(probList, "Key", "Value");

            Dictionary<int, string> impactList = new Dictionary<int, string>();
            foreach (var item in db.ImpactLevels)
                impactList.Add(item.ImpactLevelId, item.ImpactLevelId + " - " + item.ImpactLevelName);
            vm.ImpactLevels = new SelectList(impactList, "Key", "Value");
            
            vm.RiskMitigation = new RiskMitigation();
            vm.RiskMitigation.RiskId = riskId;
            vm.RiskMitigation.MitigationCode = Utils.CreateNewMiticationCode(riskId);
            vm.RiskMitigation.InputDate = DateTime.Now;
            vm.RiskMitigation.MitigationDate = DateTime.Now;
            vm.RiskMitigation.OrgPos = vm.Risk.OrgPos;

            return View(vm);
        }

        [HttpPost]
        public ActionResult MitigationNew(RiskMitigationViewModel vm, int riskId)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    vm.Risk = db.Risks.Where(p => p.RiskId == riskId).FirstOrDefault();
                    vm.RiskMitigation.RiskLevel = vm.RiskMitigation.ProbLevelId * vm.RiskMitigation.ImpactLevelId;
                    vm.RiskMitigation.DeptId = vm.Risk.DeptId;
                    vm.RiskMitigation.SubDeptId = vm.Risk.SubDeptId;
                    vm.RiskMitigation.DivisionId = vm.Risk.DivisionId;
                    vm.RiskMitigation.SubDivId = vm.Risk.SubDivId;
                    vm.RiskMitigation.BranchId = vm.Risk.BranchId;
                    vm.RiskMitigation.SubBranchId = vm.Risk.SubBranchId;
                    vm.RiskMitigation.BizUnitId = vm.Risk.BizUnitId;
                    db.RiskMitigations.AddObject(vm.RiskMitigation);
                    db.SaveChanges();

                    Utils.CreateFirstMitigationApprovalSchedule(vm.RiskMitigation.MitigationId);
                    trans.Complete();
                }
                return RedirectToAction("MitigationEdit", new { id = vm.RiskMitigation.MitigationId, approved = false });
            }
            return View(vm);
        }

        public ActionResult MitigationUnit(int? MitigationId)
        {
            RiskMitigationViewModel vm = new RiskMitigationViewModel();
        
            vm.MitigationUnit = db.MitigationUnits.Where(m => m.MitigationId == MitigationId).ToList();
            vm.RiskMitigation = db.RiskMitigations.Single(m => m.MitigationId == MitigationId);

            vm.DivisionTable = from a in db.Divisions
                    join
                    b in db.MitigationUnits
                    on a.DivisionId equals b.DivisionId
                    where b.MitigationId == MitigationId
                    select new divisionTable
                    {
                        DivisionId = a.DivisionId,
                        DivisionName = a.DivisionName,
                        MitigationId = b.MitigationId,
                        MitigationUnitId = b.MitigationUnitId
                    };

            return View(vm);
        }

        public ActionResult MitigationUnitInsert(int? id)
        {
            RiskMitigationViewModel rm = new RiskMitigationViewModel();
            rm.MitigationUnits = new MitigationUnit();

            Dictionary<int, string> unitList = new Dictionary<int, string>();
            foreach (var item in db.Divisions)
                unitList.Add(item.DivisionId, item.DivisionId + " - " + item.DivisionName);
            rm.divisions = new SelectList(unitList, "Key", "Value", 0);
            rm.MitigationUnits.MitigationId = (int) id;

            return View(rm);
        }

        [HttpPost]
        public ActionResult MitigationUnitInsert(RiskMitigationViewModel rm)
        {
                using (TransactionScope trans = new TransactionScope())
                {
                    rm.MitigationUnits.MitigationUnitId = db.MitigationUnits.OrderByDescending(m => m.MitigationUnitId).First().MitigationUnitId + 1;
                    rm.MitigationUnits.DivisionId = rm.division.DivisionId;
                    db.MitigationUnits.AddObject(rm.MitigationUnits);
                    db.SaveChanges();
                    trans.Complete();
                }
                return RedirectToAction("MitigationUnit", new { MitigationId = rm.MitigationUnits.MitigationId, approved = false });
        }

        public ActionResult MitigationUnitDelete(int? id, int? MitigationId)
        {
            RiskMitigationViewModel vm = new RiskMitigationViewModel();
            vm.MitigationUnits = db.MitigationUnits.Where(m => m.MitigationUnitId == id).First();
            db.MitigationUnits.DeleteObject(vm.MitigationUnits);
            db.SaveChanges();

            return RedirectToAction("MitigationUnit", new { MitigationId = MitigationId, approved = false });
        }

        public ActionResult MitigationEdit(int id)
        {
            RiskMitigationViewModel vm = new RiskMitigationViewModel();
            vm.RiskMitigation = db.RiskMitigations.Where(p => p.MitigationId == id).FirstOrDefault();

            vm.MitigationCats = new SelectList(db.MitigationCats, "MitigationCatId", "MitigationCatName", vm.RiskMitigation.MitigationCatId);
            vm.MitigationTypes = new SelectList(db.MitigationTypes.Where(p => p.MitigationCatId == vm.RiskMitigation.MitigationCatId), "MitigationTypeId", "MitigationTypeName", vm.RiskMitigation.MitigationTypeId);

            Dictionary<int, string> probList = new Dictionary<int, string>();
            foreach (var item in db.ProbLevels)
                probList.Add(item.ProbLevelId, item.ProbLevelId + " - " + item.ProbLevelName);
            vm.ProbLevels = new SelectList(probList, "Key", "Value", vm.RiskMitigation.ProbLevelId);

            Dictionary<int, string> impactList = new Dictionary<int, string>();
            foreach (var item in db.ImpactLevels)
                impactList.Add(item.ImpactLevelId, item.ImpactLevelId + " - " + item.ImpactLevelName);
            vm.ImpactLevels = new SelectList(impactList, "Key", "Value",vm.RiskMitigation.ImpactLevelId);

            return View(vm);
        }

        [HttpPost]
        public ActionResult MitigationEdit(RiskMitigationViewModel vm, int id)
        {
            if (ModelState.IsValid)
            {
                vm.RiskMitigation.RiskLevel = vm.RiskMitigation.ProbLevelId * vm.RiskMitigation.ImpactLevelId;
                vm.RiskMitigation.MitigationDate = DateTime.Now;
                db.RiskMitigations.Attach(vm.RiskMitigation);
                db.ObjectStateManager.ChangeObjectState(vm.RiskMitigation, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("MitigationList", new { id = vm.RiskMitigation.RiskId, approved = false });
            }
            return View(vm);
        }

        public ActionResult MitigationDelete(int id)
        {
            RiskMitigation mitigation = db.RiskMitigations.Where(p => p.MitigationId == id).FirstOrDefault();
            return View(mitigation);
        }

        [HttpPost, ActionName("MitigationDelete")]
        public ActionResult MitigationDeleteConfirmed(int id)
        {
            RiskMitigation mitigation = db.RiskMitigations.Where(p => p.MitigationId == id).FirstOrDefault();
            var riskId = mitigation.RiskId;
            db.RiskMitigations.DeleteObject(mitigation);
            db.SaveChanges();
            return RedirectToAction("MitigationList", new { id = riskId, approved = false });
        }

        public ActionResult MitigationDetail(int id)
        {
            RiskMitigation mitigation = db.RiskMitigations.Where(p => p.MitigationId == id).FirstOrDefault();
            return View(mitigation);
        }

        public ActionResult ApproveMitigation(int approvalId)
        {
            MitigationApproval apr = db.MitigationApprovals.Where(p => p.ApprovalId == approvalId).FirstOrDefault();
            return View(apr);
        }

        [HttpPost, ActionName("ApproveMitigation")]
        public ActionResult ApproveMitigationConfirmed(int approvalId)
        {
            try
            {
                Utils.ApproveMitigation(approvalId, db);
            }
            catch (Exception e)
            {
                ViewBag.Message = "Tidak bisa meng-approve data mitigasi risiko : " + e.Message;
                return View("Error");
            }
            ViewBag.Message = "Data mitigasi risiko telah di-approve";
            return View("Info");
            //using (TransactionScope trans = new TransactionScope())
            //{
            //    try
            //    {
            //        UserData data = Utils.LoadUserDataFromSession();
            //        MitigationApproval apr = db.MitigationApprovals.Where(p => p.ApprovalId == approvalId).FirstOrDefault();
            //        apr.ApprovalDate = DateTime.Now;
            //        apr.UserId = data.UserId;
            //        apr.JobTitle = data.JobTitle;
            //        db.SaveChanges();

            //        apr.RiskMitigation.IsReadOnly = true;
            //        db.SaveChanges();

            //        if (apr.LastApproval)
            //        {
            //            // mitigation approval complete
            //            apr.RiskMitigation.ApprovalDate = apr.ApprovalDate;
            //            db.SaveChanges();

            //            MitigationApproval prevApr = db.MitigationApprovals
            //                .Where(p => p.MitigationId == apr.MitigationId && p.ApprovalId != apr.ApprovalId && p.ApprovalDate != null)
            //                .FirstOrDefault();
            //            if (prevApr != null)
            //                prevApr.IsReadOnly = true;
            //            db.SaveChanges();

            //            RiskState rs = new RiskState();
            //            rs.RiskId = apr.RiskMitigation.RiskId;
            //            rs.MitigationId = apr.MitigationId;
            //            rs.StateDate = (DateTime)apr.ApprovalDate;
            //            rs.ProbLevelId = (int)apr.RiskMitigation.ProbLevelId;
            //            rs.ImpactLevelId = (int)apr.RiskMitigation.ImpactLevelId;
            //            rs.RiskLevel = (int)apr.RiskMitigation.RiskLevel;
            //            db.RiskStates.AddObject(rs);
            //            db.SaveChanges();
            //        }
            //        else
            //        {
            //            // create next approval schedule
            //            MitigationApproval nextApr = new MitigationApproval();
            //            nextApr.MitigationId = apr.MitigationId;
            //            nextApr.LimitDate = DateTime.Now.AddDays(Utils.MAX_LIMIT_APPROVAL_DAYS);
            //            nextApr.LastApproval = true;

            //            switch (apr.OrgPos)
            //            {
            //                case Utils.ORGPOS_SUBDIV:
            //                    SubDiv subDiv = db.SubDivs.Single(p => p.SubDivId == apr.SubDivId);
            //                    nextApr.OrgPos = Utils.ORGPOS_DIVISION;
            //                    nextApr.DivisionId = subDiv.DivisionId;
            //                    break;
            //                case Utils.ORGPOS_SUBBRANCH:
            //                    SubBranch subBranch = db.SubBranches.Single(p => p.SubBranchId == apr.SubBranchId);
            //                    nextApr.OrgPos = Utils.ORGPOS_BRANCH;
            //                    nextApr.BranchId = subBranch.BranchId;
            //                    break;
            //                case Utils.ORGPOS_BIZUNIT:
            //                    BizUnit bizUnit = db.BizUnits.Single(p => p.BizUnitId == apr.BizUnitId);
            //                    nextApr.OrgPos = Utils.ORGPOS_BRANCH;
            //                    nextApr.BranchId = bizUnit.BranchId;
            //                    break;
            //            }
            //            db.MitigationApprovals.AddObject(nextApr);
            //            db.SaveChanges();
            //        }
            //        trans.Complete();
            //    }
            //    catch (Exception e)
            //    {
            //        ViewBag.Message = "Tidak bisa meng-approve data mitigasi risiko : " + e.Message;
            //        return View("Error");
            //    }
            //}
            //ViewBag.Message = "Data mitigasi risiko telah di-approve";
            //return View("Info");
        }

        public ActionResult DisapproveMitigation(int approvalId)
        {
            MitigationApproval apr = db.MitigationApprovals.Where(p => p.ApprovalId == approvalId).FirstOrDefault();
            return View(apr);
        }

        [HttpPost, ActionName("DisapproveMitigation")]
        public ActionResult DisapproveMitigationConfirmed(int approvalId)
        {
            try
            {
                Utils.CancelMitigationApproval(approvalId, db);
            }
            catch (Exception e)
            {
                ViewBag.Message = "Tidak bisa membatalkan approve mitigasi risiko : " + e.Message;
                return View("Error");
            }
            ViewBag.Message = "Approve data mitigasi risiko telah dibatalkan";
            return View("Info");
            //using (TransactionScope trans = new TransactionScope())
            //{
            //    try
            //    {
            //        UserData data = Utils.LoadUserDataFromSession();
            //        MitigationApproval apr = db.MitigationApprovals.Where(p => p.ApprovalId == approvalId).FirstOrDefault();
            //        MitigationApproval nextApr = db.MitigationApprovals
            //            .Where(p => p.MitigationId == apr.MitigationId && p.ApprovalDate == null && p.ApprovalId != apr.ApprovalId)
            //            .FirstOrDefault();
            //        if (nextApr != null)
            //            db.MitigationApprovals.DeleteObject(nextApr);
            //        apr.UserId = null;
            //        apr.JobTitle = null;
            //        apr.ApprovalDate = null;
            //        db.SaveChanges();

            //        if (apr.LastApproval)
            //        {
            //            apr.RiskMitigation.ApprovalDate = null;
            //            RiskState rs = db.RiskStates.Where(p => p.RiskId == apr.RiskMitigation.RiskId && p.MitigationId == apr.MitigationId).FirstOrDefault();
            //            if (rs != null)
            //            {
            //                db.RiskStates.DeleteObject(rs);
            //                db.SaveChanges();
            //            }
            //        }

            //        MitigationApproval prevApr = db.MitigationApprovals
            //            .Where(p => p.MitigationId == apr.MitigationId && p.ApprovalDate != null && p.ApprovalId != apr.ApprovalId)
            //            .FirstOrDefault();
            //        if (prevApr == null)
            //            apr.RiskMitigation.IsReadOnly = false;
            //        else
            //            prevApr.IsReadOnly = false;
            //        db.SaveChanges();
            //        trans.Complete();
            //    }
            //    catch 
            //    {
            //        ViewBag.Message = "Tidak bisa membatalkan approve mitigasi risiko";
            //        return View("Error");
            //    }
            //}
            //return RedirectToAction("DisapproveMitigationSuccess");
        }

        public ActionResult DisapproveMitigationSuccess()
        {
            return View();
        }

        public ActionResult RiskReference()
        {
            var risks = db.RiskEvents.ToList();
            return View(risks);
        }

        #region JSON

        [HttpGet]
        public JsonResult LoadCauseTypes(string groupId)
        {
            int id = string.IsNullOrEmpty(groupId) ? 0 : Convert.ToInt32(groupId);
            var causeTypes = db.CauseTypes.Where(p => p.CauseGroupId == id).ToList();
            var selectItems = causeTypes.Select(m => new SelectListItem()
            {
                Text = m.CauseTypeName,
                Value = m.CauseTypeId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadCauses(string typeId)
        {
            int id = string.IsNullOrEmpty(typeId) ? 0 : Convert.ToInt32(typeId);
            var causes = db.Causes.Where(p => p.CauseTypeId == id).ToList();
            var selectItems = causes.Select(p => new SelectListItem()
            {
                Text = p.CauseName,
                Value = p.CauseId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadEffectTypes(string groupId)
        {
            int id = string.IsNullOrEmpty(groupId) ? 0 : Convert.ToInt32(groupId);
            var effectTypes = db.EffectTypes.Where(p => p.EffectGroupId == id).ToList();
            var selectItems = effectTypes.Select(p => new SelectListItem()
            {
                Text = p.EffectTypeName,
                Value = p.EffectTypeId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadEffects(string typeId)
        {
            var id = string.IsNullOrEmpty(typeId) ? 0 : Convert.ToInt32(typeId);
            var effects = db.Effects.Where(p => p.EffectTypeId == id).ToList();
            var selectItems = effects.Select(p => new SelectListItem()
            {
                Text = p.EffectName,
                Value = p.EffectId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadRiskGroups(string catId)
        {
            var id = string.IsNullOrEmpty(catId) ? 0 : Convert.ToInt32(catId);
            var groups = db.RiskGroups.Where(p => p.RiskCatId == id).ToList();
            var selectItems = groups.Select(p => new SelectListItem()
            {
                Text = p.RiskGroupName,
                Value = p.RiskGroupId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadRiskTypes(string groupId)
        {
            var id = string.IsNullOrEmpty(groupId) ? 0 : Convert.ToInt32(groupId);
            var types = db.RiskTypes.Where(p => p.RiskGroupId == id).ToList();
            var selectItems = types.Select(p => new SelectListItem()
            {
                Text = p.RiskTypeName,
                Value = p.RiskTypeId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult LoadMitigationTypes(string catId)
        {
            var id = string.IsNullOrEmpty(catId) ? 0 : Convert.ToInt32(catId);
            var types = db.MitigationTypes.Where(p => p.MitigationCatId == id).ToList();
            var selectItems = types.Select(p => new SelectListItem()
            {
                Text = p.MitigationTypeName,
                Value = p.MitigationTypeId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
