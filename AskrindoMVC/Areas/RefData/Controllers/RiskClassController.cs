using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using System.Data;
using AskrindoMVC.Areas.RefData.Models.RiskClass;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Areas.RefData.Controllers
{
    [Authorize]
    public class RiskClassController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();
        UserData userData = Utils.LoadUserDataFromSession();

        public ActionResult Index()
        {
            return View();
        }

        #region RiskCat
        
        public ActionResult RiskCatList()
        {
            ViewBag.CanModify = userData.IsAdmin;
            return View(db.RiskCats);
        }

        public ActionResult RiskCatNew()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RiskCatNew(RiskCat cat)
        {
            if (ModelState.IsValid)
            {
                db.RiskCats.AddObject(cat);
                db.SaveChanges();
                return RedirectToAction("RiskCatList");
            }
            return View(cat);
        }

        public ActionResult RiskCatEdit(int id)
        {
            return View(db.RiskCats.Single(p => p.RiskCatId == id));
        }

        [HttpPost]
        public ActionResult RiskCatEdit(RiskCat cat)
        {
            if (ModelState.IsValid)
            {
                db.RiskCats.Attach(cat);
                db.ObjectStateManager.ChangeObjectState(cat, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("RiskCatList");
            }
            return View(cat);
        }

        public ActionResult RiskCatDelete(int id)
        {
            return View(db.RiskCats.Single(p => p.RiskCatId == id));
        }

        [HttpPost, ActionName("RiskCatDelete")]
        public ActionResult RiskCatDeleteConfirmed(int id)
        {
            RiskCat cat = db.RiskCats.Single(p => p.RiskCatId == id);
            db.RiskCats.DeleteObject(cat);
            db.SaveChanges();
            return RedirectToAction("RiskCatList");
        }

        #endregion

        #region RiskGroup
        
        public ActionResult RiskGroupList(int catId)
        {
            RiskGroupViewModel vm = new RiskGroupViewModel();
            vm.RiskCat = db.RiskCats.Single(p => p.RiskCatId == catId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult RiskGroupNew(int catId)
        {
            RiskGroupViewModel vm = new RiskGroupViewModel();
            vm.RiskCat = db.RiskCats.Single(p => p.RiskCatId == catId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult RiskGroupNew(RiskGroupViewModel vm, int catId)
        {
            if (ModelState.IsValid)
            {
                vm.RiskGroup.RiskCatId = catId;
                db.RiskGroups.AddObject(vm.RiskGroup);
                db.SaveChanges();
                return RedirectToAction("RiskGroupList", new { catId = catId });
            }
            vm.RiskCat = db.RiskCats.Single(p => p.RiskCatId == catId);
            return View(vm);
        }

        public ActionResult RiskGroupEdit(int id)
        {
            RiskGroupViewModel vm = new RiskGroupViewModel();
            vm.RiskGroup = db.RiskGroups.Single(p => p.RiskGroupId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult RiskGroupEdit(RiskGroupViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.RiskGroups.Attach(vm.RiskGroup);
                db.ObjectStateManager.ChangeObjectState(vm.RiskGroup, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("RiskGroupList", new { catId = vm.RiskGroup.RiskCatId });
            }
            return View(vm);
        }

        public ActionResult RiskGroupDelete(int id)
        {
            RiskGroupViewModel vm = new RiskGroupViewModel();
            vm.RiskGroup = db.RiskGroups.Single(p => p.RiskGroupId == id);
            return View(vm);
        }

        [HttpPost, ActionName("RiskGroupDelete")]
        public ActionResult RiskGroupDeleteConfirmed(int id)
        {
            RiskGroup group = db.RiskGroups.Single(p => p.RiskGroupId == id);
            var catId = group.RiskCatId;
            db.RiskGroups.DeleteObject(group);
            db.SaveChanges();
            return RedirectToAction("RiskGroupList", new { catId = catId });
        }

        #endregion

        public ActionResult RiskTypeList(int groupId)
        {
            RiskTypeViewModel vm = new RiskTypeViewModel();
            vm.RiskGroup = db.RiskGroups.Single(p => p.RiskGroupId == groupId);
            vm.RiskTypes = db.RiskTypes.Where(p => p.RiskGroupId == groupId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult RiskTypeNew(int groupId)
        {
            RiskTypeViewModel vm = new RiskTypeViewModel();
            vm.RiskGroup = db.RiskGroups.Single(p => p.RiskGroupId == groupId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult RiskTypeNew(RiskTypeViewModel vm, int groupId)
        {
            if (ModelState.IsValid)
            {
                vm.RiskType.RiskGroupId = groupId;
                db.RiskTypes.AddObject(vm.RiskType);
                db.SaveChanges();
                return RedirectToAction("RiskTypeList", new { groupId = groupId });
            }
            vm.RiskGroup = db.RiskGroups.Single(p => p.RiskGroupId == groupId);
            return View(vm);
        }

        public ActionResult RiskTypeEdit(int id)
        {
            RiskTypeViewModel vm = new RiskTypeViewModel();
            vm.RiskType = db.RiskTypes.Single(p => p.RiskTypeId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult RiskTypeEdit(RiskTypeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.RiskTypes.Attach(vm.RiskType);
                db.ObjectStateManager.ChangeObjectState(vm.RiskType, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("RiskTypeList", new { groupId = vm.RiskType.RiskGroupId });
            }
            return View(vm);
        }

        public ActionResult RiskTypeDelete(int id)
        {
            RiskTypeViewModel vm = new RiskTypeViewModel();
            vm.RiskType = db.RiskTypes.Single(p => p.RiskTypeId == id);
            return View(vm);
        }

        [HttpPost, ActionName("RiskTypeDelete")]
        public ActionResult RiskTypeDeleteConfirmed(int id)
        {
            RiskType type = db.RiskTypes.Single(p => p.RiskTypeId == id);
            var groupId = type.RiskGroupId;
            db.RiskTypes.DeleteObject(type);
            db.SaveChanges();
            return RedirectToAction("RiskTypeList", new { groupId = groupId });
        }

        public ActionResult TreeView()
        {
            return View(db.RiskCats);
        }

        public ActionResult TableView()
        {
            return View(db.RiskTypes);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
