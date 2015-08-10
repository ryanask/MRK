using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using System.Data;
using AskrindoMVC.Areas.RefData.Models.ImpactLevel;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Areas.RefData.Controllers
{
    [Authorize]
    public class ImpactLevelController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();
        UserData userData = Utils.LoadUserDataFromSession();

        public ActionResult Index()
        {
            ViewBag.CanModify = userData.IsAdmin;
            return View(db.ImpactLevels);
        }

        public ActionResult Edit(int id)
        {
            return View(db.ImpactLevels.Single(p => p.ImpactLevelId == id));
        }

        [HttpPost]
        public ActionResult Edit(ImpactLevel level)
        {
            if (ModelState.IsValid)
            {
                db.ImpactLevels.Attach(level);
                db.ObjectStateManager.ChangeObjectState(level, EntityState.Modified);
                db.SaveChanges();
                UpdateImpactLevelMoneyValues();
                return RedirectToAction("Index");
            }
            return View(level);
        }

        public ActionResult ImpactPercentage()
        {
            ViewBag.CanModify = userData.IsAdmin;
            return View(db.ImpactRefs.First());
        }

        public ActionResult ImpactPercentageEdit()
        {
            return View(db.ImpactRefs.First());
        }

        [HttpPost]
        public ActionResult ImpactPercentageEdit(ImpactRef impactRef)
        {
            if (ModelState.IsValid)
            {
                db.ImpactRefs.Attach(impactRef);
                db.ObjectStateManager.ChangeObjectState(impactRef, EntityState.Modified);
                db.SaveChanges();
                UpdateImpactLevelMoneyValues();
                return RedirectToAction("ImpactPercentage");
            }
            return View(impactRef);
        }

        private void UpdateImpactLevelMoneyValues()
        {
            ImpactRef impactRef = db.ImpactRefs.First();
            decimal maxMoney = impactRef.MaxMoney;

            foreach (var level in db.ImpactLevels)
            {
                level.MoneyMin = (decimal)level.PctMin / 100 * maxMoney;
                level.MoneyMax = (decimal)level.PctMax / 100 * maxMoney;
            }

            db.SaveChanges();
        }

        public ActionResult ShowImpactValue(string item)
        {
            ImpactRef impactRef = db.ImpactRefs.First();
            decimal pct = 0;
            switch (item.ToLower())
            {
                case "hq":
                    pct = impactRef.HQPct;
                    ViewBag.LevelName = "Kantor Pusat";
                    break;
                case "branch1":
                    pct = impactRef.Branch1Pct;
                    ViewBag.LevelName = "Cabang Kelas I";
                    break;
                case "branch2":
                    pct = impactRef.Branch2Pct;
                    ViewBag.LevelName = "Cabang Kelas II";
                    break;
                case "branch3":
                    pct = impactRef.Branch3Pct;
                    ViewBag.LevelName = "Cabang Kelas III";
                    break;
                case "bizunit":
                    pct = impactRef.BizUnitPct;
                    ViewBag.LevelName = "KUP";
                    break;
                case "supportinghq":
                    pct = impactRef.SupportingHQPct;
                    ViewBag.LevelName = "Supporting Unit Kantor Pusat";
                    break;
                case "supportingbranch":
                    pct = impactRef.SupportingBranchPct;
                    ViewBag.LevelName = "Supporting Unit Cabang";
                    break;
                case "supportingbizunit":
                    pct = impactRef.SupportingBizUnitPct;
                    ViewBag.LevelName = "Supporting Unit KUP";
                    break;
            }

            ImpactValueViewModel vm = new ImpactValueViewModel();
            vm.ImpactValues = new List<ImpactValue>();
            foreach (var level in db.ImpactLevels)
            {
                ImpactValue value = new ImpactValue();
                value.ImpactLevelId = level.ImpactLevelId;
                value.ImpactLevelName = level.ImpactLevelName;
                value.MinValue = pct / 100 * (decimal)level.MoneyMin;
                value.MaxValue = pct / 100 * (decimal)level.MoneyMax;
                vm.ImpactValues.Add(value);
            }
            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
