using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using System.Data;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Areas.RefData.Controllers
{
    [Authorize]
    public class RiskLevelController : Controller
    {
        private AskrindoMVCEntities db = new AskrindoMVCEntities();
        UserData userData = Utils.LoadUserDataFromSession();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            ViewBag.CanModify = userData.IsAdmin;
            return View(db.RiskLevels);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(RiskLevel r)
        {
            if (ModelState.IsValid)
            {
                db.RiskLevels.AddObject(r);
                //db.ObjectStateManager.ChangeObjectState(r, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(r);
        }

        public ActionResult Edit(int id)
        {
            return View(db.RiskLevels.Single(p => p.LevelId == id));
        }

        [HttpPost]
        public ActionResult Edit(RiskLevel r, int id)
        {
            if (ModelState.IsValid)
            {
                db.RiskLevels.Attach(r);
                db.ObjectStateManager.ChangeObjectState(r, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(r);
        }

        public ActionResult Delete(int id)
        {
            return View(db.RiskLevels.Single(p => p.LevelId == id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var r = db.RiskLevels.Single(p => p.LevelId == id);
            db.RiskLevels.DeleteObject(r);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
