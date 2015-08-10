using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Helpers;
using AskrindoMVC.Models;
using AskrindoMVC.Models.RiskInfo;

namespace AskrindoMVC.Controllers
{
    public class RiskInfoController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        public ActionResult Index()
        {
            return View(db.Risks);
        }

        public ActionResult Details(int id)
        {
            var risk = db.Risks.Single(p => p.RiskId == id);
            return View(risk);
        }

        public ActionResult Probability(int id)
        {
            var prob = db.RiskProbs.Where(p => p.RiskId == id).FirstOrDefault();
            if (prob == null)
            {
                var risk = db.Risks.Single(p => p.RiskId == id);
                ViewBag.ErrorTitle = "Probabilitas";
                ViewBag.Message = "Data probabilitas tidak ditemukan.";
                return View("RiskInfoError", risk);
            }
            return View(prob);
        }

        public ActionResult Impact(int id)
        {
            RiskImpactViewModel vm = new RiskImpactViewModel();
            vm.RiskImpact = db.RiskImpacts.Where(p => p.RiskId == id).FirstOrDefault();
            vm.RiskNonMoneyImpacts = db.RiskNonMoneyImpacts.Where(p => p.RiskId == id);
            return View(vm);
        }

        public ActionResult Attachments(int id)
        {
            RiskAttachmentViewModel vm = new RiskAttachmentViewModel();
            vm.Risk = db.Risks.Single(p => p.RiskId == id);
            vm.RiskAttachments = db.RiskAttachments.Where(p => p.RiskId == id);
            return View(vm);
        }

        public FileContentResult DownloadAttachment(int id)
        {
            byte[] fileData;
            var attach = db.RiskAttachments.Where(p => p.AttachId == id).FirstOrDefault();
            fileData = attach.Data.ToArray();
            return File(fileData, attach.ContentType, attach.Filename);
        }

        public ActionResult Approvals(int id)
        {
            RiskApprovalsViewModel vm = new RiskApprovalsViewModel();
            vm.Risk = db.Risks.Single(p => p.RiskId == id);
            vm.RiskApprovals = db.RiskApprovals.Where(p => p.RiskId == id && p.ApprovalDate != null);
            return View(vm);
        }

        public ActionResult Mitigations(int id)
        {
            RiskMitigationViewModel vm = new RiskMitigationViewModel();
            vm.Risk = db.Risks.Single(p => p.RiskId == id);
            vm.ApprovedMitigations = db.RiskMitigations.Where(p => p.RiskId == id && p.ApprovalDate != null);
            vm.PlannedMitigations = db.RiskMitigations.Where(p => p.RiskId == id && p.ApprovalDate == null);
            return View(vm);
        }

        public ActionResult RiskStates(int id)
        {
            ViewBag.Risk = db.Risks.Single(p => p.RiskId == id);
            return View(db.RiskStates.Where(p => p.RiskId == id).OrderBy(p => p.StateDate));
        }
    }
}
