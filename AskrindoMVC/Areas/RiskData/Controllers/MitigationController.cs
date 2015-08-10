using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using AskrindoMVC.Helpers;
using AskrindoMVC.Areas.RiskData.Models.RiskData;
using System.Data;
using System.Transactions;
using AskrindoMVC.Areas.RiskData.Models.Mitigation;

namespace AskrindoMVC.Areas.RiskData.Controllers
{
    [Authorize]
    public class MitigationController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();
        UserData userData = Utils.LoadUserDataFromSession();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MitigationList()
        {
            UserData data = Utils.LoadUserDataFromSession();
            if (data.IsAdmin)
                return RedirectToAction("MitigationListAdmin");
            else if (data.IsRiskOwner)
                return RedirectToAction("MitigationListRO");
            else
                return RedirectToAction("MitigationListNonRO", new { approved = false });
        }

        public ActionResult MitigationListRO(string status)
        {
            UserData data = Utils.LoadUserDataFromSession();
            var mitigations = db.RiskMitigations.Where(p => p.Risk.UserId == data.UserId);
            if (string.IsNullOrEmpty(status))
                mitigations = mitigations.Where(p => !p.IsReadOnly);
            else if (status.ToLower() == "readonly")
                mitigations = mitigations.Where(p => p.IsReadOnly && p.ApprovalDate == null);
            else if (status.ToLower() == "approved")
                mitigations = mitigations.Where(p => p.ApprovalDate != null);
            return View(mitigations);
        }

        public ActionResult MitigationListNonRO(bool approved)
        {
            UserData data = Utils.LoadUserDataFromSession();
            var aprs = db.MitigationApprovals.Where(p => p.OrgPos == data.OrgPos);
            if (approved)
                aprs = aprs.Where(p => p.ApprovalDate != null && !p.IsReadOnly);
            else
                aprs = aprs.Where(p => p.ApprovalDate == null && p.RiskMitigation.Risk.ApprovalDate != null);

            switch (data.OrgPos)
            {
                case Utils.ORGPOS_DEPT:
                    aprs = aprs.Where(p => p.DeptId == data.DeptId);
                    break;
                case Utils.ORGPOS_SUBDEPT:
                    aprs = aprs.Where(p => p.SubDeptId == data.SubDeptId);
                    break;
                case Utils.ORGPOS_DIVISION:
                    aprs = aprs.Where(p => p.DivisionId == data.DivisionId);
                    break;
                case Utils.ORGPOS_SUBDIV:
                    aprs = aprs.Where(p => p.SubDivId == data.SubDivId);
                    break;
                case Utils.ORGPOS_BRANCH:
                    aprs = aprs.Where(p => p.BranchId == data.BranchId);
                    break;
                case Utils.ORGPOS_SUBBRANCH:
                    aprs = aprs.Where(p => p.SubBranchId == data.SubBranchId);
                    break;
                case Utils.ORGPOS_BIZUNIT:
                    aprs = aprs.Where(p => p.BizUnitId == data.BizUnitId);
                    break;
            }
            return View(aprs);
        }

        public ActionResult MitigationListAdmin()
        {
            MitigationListParamViewModel vm = new MitigationListParamViewModel();
            vm.PosId = 1;
            vm.StateId = 1;
            UpdateListParam(vm);
            return View(vm);
        }

        [HttpPost]
        public ActionResult MitigationListAdmin(MitigationListParamViewModel vm)
        {
            UpdateListParam(vm);
            return View(vm);
        }

        private void UpdateListParam(MitigationListParamViewModel vm)
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
            vm.States = new SelectList(stateList, "Key", "Value", vm.StateId);

            //vm.MitigationApprovals = db.MitigationApprovals;
            vm.RiskMitigations = db.RiskMitigations;
            if (vm.PosId == 1)
                //vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.RiskMitigation.Risk.DeptId != null);
                vm.RiskMitigations = db.RiskMitigations.Where(p => p.Risk.DeptId != null);
            else if (vm.PosId == 2)
            {
                //vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.RiskMitigation.Risk.BranchId != null);
                vm.RiskMitigations = vm.RiskMitigations.Where(p => p.Risk.BranchId != null);
                if (vm.BranchId != null)
                    //vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.RiskMitigation.Risk.BranchId == vm.BranchId);
                    vm.RiskMitigations = vm.RiskMitigations.Where(p => p.Risk.BranchId == vm.BranchId);
            }

            switch (vm.StateId)
            {
                case 1:
                    //vm.MitigationApprovals = vm.MitigationApprovals.Where(p => !p.RiskMitigation.IsReadOnly);
                    vm.RiskMitigations = vm.RiskMitigations.Where(p => !p.IsReadOnly);
                    break;
                case 2:
                    //vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.RiskMitigation.IsReadOnly && p.RiskMitigation.ApprovalDate == null);
                    vm.RiskMitigations = vm.RiskMitigations.Where(p => p.IsReadOnly && p.ApprovalDate == null);
                    break;
                case 3:
                    //vm.MitigationApprovals = vm.MitigationApprovals.Where(p => p.RiskMitigation.ApprovalDate != null);
                    vm.RiskMitigations = vm.RiskMitigations.Where(p => p.ApprovalDate != null);
                    break;
            }
            vm.RiskMitigations = vm.RiskMitigations.OrderBy(p => p.MitigationDate);

        }

        public ActionResult Detail(int id)
        {
            return View(db.RiskMitigations.Single(p => p.MitigationId == id));
        }

        public ActionResult Edit(int id)
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
            vm.ImpactLevels = new SelectList(impactList, "Key", "Value", vm.RiskMitigation.ImpactLevelId);

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(RiskMitigationViewModel vm, int id)
        {
            if (ModelState.IsValid)
            {
                vm.RiskMitigation.RiskLevel = vm.RiskMitigation.ProbLevelId * vm.RiskMitigation.ImpactLevelId;
                db.RiskMitigations.Attach(vm.RiskMitigation);
                db.ObjectStateManager.ChangeObjectState(vm.RiskMitigation, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("MitigationList");
            }
            return View(vm);
        }

        public ActionResult Delete(int id)
        {
            return View(db.RiskMitigations.Single(p => p.MitigationId == id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            RiskMitigation mitigation = db.RiskMitigations.Where(p => p.MitigationId == id).FirstOrDefault();
            db.RiskMitigations.DeleteObject(mitigation);
            db.SaveChanges();
            ViewBag.Message = "Data mitigasi risiko sudah dihapus";
            return View("Info");
        }

        public ActionResult Approve(int approvalId)
        {
            var apr = db.MitigationApprovals.Where(p => p.ApprovalId == approvalId).FirstOrDefault();
            return View(apr);
        }

        [HttpPost, ActionName("Approve")]
        public ActionResult ApproveConfirmed(int approvalId)
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
        }

        public ActionResult CancelApprove(int approvalId)
        {
            var apr = db.MitigationApprovals.Where(p => p.ApprovalId == approvalId).FirstOrDefault();
            return View(apr);
        }

        [HttpPost, ActionName("CancelApprove")]
        public ActionResult CancelApproveConfirmed(int approvalId)
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
        }
    }
}
