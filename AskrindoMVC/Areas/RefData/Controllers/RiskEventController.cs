using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Areas.RefData.Controllers
{
    public class RiskEventController : Controller
    {
        //
        // GET: /RefData/RiskEvent/
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        public ActionResult Index()
        {
            var risks = db.RiskEvents.ToList();
            return View(risks);
        }

        [HttpGet]
        public ActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(RiskEvent m)
        {
            if (ModelState.IsValid)
            {
                db.RiskEvents.AddObject(m);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(m);
            
        }

        public ActionResult Edit(int id)
        {
            RiskEvent r = db.RiskEvents.Single(p => p.RiskEventID == id);
            return View(r);
        }

        [HttpPost]
        public ActionResult Edit(RiskEvent r, int id)
        {
            if (ModelState.IsValid)
            {
                db.RiskEvents.Attach(r);
                db.ObjectStateManager.ChangeObjectState(r, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(r);
        }

        public ActionResult Delete(int id)
        {
            return View(db.RiskEvents.Single(p => p.RiskEventID == id));
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var r = db.RiskEvents.Single(p => p.RiskEventID == id);
            db.RiskEvents.DeleteObject(r);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
