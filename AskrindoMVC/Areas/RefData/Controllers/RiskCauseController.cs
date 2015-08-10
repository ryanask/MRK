using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using System.Data;
using AskrindoMVC.Areas.RefData.Models.RiskCause;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Areas.RefData.Controllers
{
    [Authorize]
    public class RiskCauseController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();
        UserData userData = Utils.LoadUserDataFromSession();

        public ActionResult Index()
        {
            return View();
        }

        #region CauseGroup
        
        public ActionResult CauseGroupList()
        {
            var groups = db.CauseGroups;
            ViewBag.CanModify = userData.IsAdmin;
            return View(groups);
        }

        public ActionResult CauseGroupNew()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CauseGroupNew(CauseGroup group)
        {
            if (ModelState.IsValid)
            {
                db.CauseGroups.AddObject(group);
                db.SaveChanges();
                return RedirectToAction("CauseGroupList");
            }
            return View(group);
        }

        public ActionResult CauseGroupEdit(int id)
        {
            CauseGroup group = db.CauseGroups.Single(p => p.CauseGroupId == id);
            return View(group);
        }

        [HttpPost]
        public ActionResult CauseGroupEdit(CauseGroup group)
        {
            if (ModelState.IsValid)
            {
                db.CauseGroups.Attach(group);
                db.ObjectStateManager.ChangeObjectState(group, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("CauseGroupList");
            }
            return View(group);
        }

        public ActionResult CauseGroupDelete(int id)
        {
            CauseGroup group = db.CauseGroups.Single(p => p.CauseGroupId == id);
            return View(group);
        }

        [HttpPost, ActionName("CauseGroupDelete")]
        public ActionResult CauseGroupDeleteConfirmed(int id)
        {
            CauseGroup group = db.CauseGroups.Single(p => p.CauseGroupId == id);
            db.CauseGroups.DeleteObject(group);
            db.SaveChanges();
            return RedirectToAction("CauseGroupList");
        }

        #endregion

        #region CauseType

        public ActionResult CauseTypeList(int groupId)
        {
            CauseTypeViewModel vm = new CauseTypeViewModel();
            vm.CauseGroup = db.CauseGroups.Single(p => p.CauseGroupId == groupId);
            vm.CauseTypes = db.CauseTypes.Where(p => p.CauseGroupId == groupId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult CauseTypeNew(int groupId)
        {
            CauseTypeViewModel vm = new CauseTypeViewModel();
            vm.CauseGroup = db.CauseGroups.Single(p => p.CauseGroupId == groupId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CauseTypeNew(CauseTypeViewModel vm, int groupId)
        {
            if (ModelState.IsValid)
            {
                vm.CauseType.CauseGroupId = groupId;
                db.CauseTypes.AddObject(vm.CauseType);
                db.SaveChanges();
                return RedirectToAction("CauseTypeList", new { groupId = groupId });
            }
            vm.CauseGroup = db.CauseGroups.Single(p => p.CauseGroupId == groupId);
            return View(vm);
        }

        public ActionResult CauseTypeEdit(int id)
        {
            CauseTypeViewModel vm = new CauseTypeViewModel();
            vm.CauseType = db.CauseTypes.Single(p => p.CauseTypeId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CauseTypeEdit(CauseTypeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.CauseTypes.Attach(vm.CauseType);
                db.ObjectStateManager.ChangeObjectState(vm.CauseType, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("CauseTypeList", new { groupId = vm.CauseType.CauseGroupId });
            }
            return View(vm);
        }

        public ActionResult CauseTypeDelete(int id)
        {
            CauseTypeViewModel vm = new CauseTypeViewModel();
            vm.CauseType = db.CauseTypes.Single(p => p.CauseTypeId == id);
            return View(vm);
        }

        [HttpPost, ActionName("CauseTypeDelete")]
        public ActionResult CauseTypeDeleteConfirmed(int id)
        {
            CauseType type = db.CauseTypes.Single(p => p.CauseTypeId == id);
            var groupId = type.CauseGroupId;
            db.CauseTypes.DeleteObject(type);
            db.SaveChanges();
            return RedirectToAction("CauseTypeList", new { groupId = groupId });
        }

        #endregion

        #region Cause
        
        public ActionResult CauseList(int typeId)
        {
            CauseViewModel vm = new CauseViewModel();
            vm.CauseType = db.CauseTypes.Single(p => p.CauseTypeId == typeId);
            vm.Causes = db.Causes.Where(p => p.CauseTypeId == typeId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult CauseNew(int typeId)
        {
            CauseViewModel vm = new CauseViewModel();
            vm.CauseType = db.CauseTypes.Single(p => p.CauseTypeId == typeId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CauseNew(CauseViewModel vm, int typeId)
        {
            if (ModelState.IsValid)
            {
                vm.Cause.CauseTypeId = typeId;
                db.Causes.AddObject(vm.Cause);
                db.SaveChanges();
                return RedirectToAction("CauseList", new { typeId = typeId });
            }
            vm.CauseType = db.CauseTypes.Single(p => p.CauseTypeId == typeId);
            return View(vm);
        }

        public ActionResult CauseEdit(int id)
        {
            CauseViewModel vm = new CauseViewModel();
            vm.Cause = db.Causes.Single(p => p.CauseId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CauseEdit(CauseViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.Causes.Attach(vm.Cause);
                db.ObjectStateManager.ChangeObjectState(vm.Cause, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("CauseList", new { typeId = vm.Cause.CauseTypeId });
            }
            return View(vm);
        }

        public ActionResult CauseDelete(int id)
        {
            CauseViewModel vm = new CauseViewModel();
            vm.Cause = db.Causes.Single(p => p.CauseId == id);
            return View(vm);
        }

        [HttpPost, ActionName("CauseDelete")]
        public ActionResult CauseDeleteConfirmed(int id)
        {
            Cause cause = db.Causes.Single(p => p.CauseId == id);
            var typeId = cause.CauseTypeId;
            db.Causes.DeleteObject(cause);
            db.SaveChanges();
            return RedirectToAction("CauseList", new { typeId = typeId });
        }

        #endregion

        public ActionResult TreeView()
        {
            return View(db.CauseGroups);
        }

        public ActionResult TableView()
        {
            return View(db.Causes);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
