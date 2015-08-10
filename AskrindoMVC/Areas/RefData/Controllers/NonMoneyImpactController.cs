using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using System.Data;
using AskrindoMVC.Areas.RefData.Models.NonMoneyImpact;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Areas.RefData.Controllers
{
    [Authorize]
    public class NonMoneyImpactController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();
        UserData userData = Utils.LoadUserDataFromSession();

        public ActionResult Index()
        {
            return View();
        }

        #region ImpactCat
        
        public ActionResult ImpactCatList()
        {
            ViewBag.CanModify = userData.IsAdmin;
            return View(db.ImpactCats);
        }

        public ActionResult ImpactCatNew()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImpactCatNew(ImpactCat cat)
        {
            if (ModelState.IsValid)
            {
                db.ImpactCats.AddObject(cat);
                db.SaveChanges();
                return RedirectToAction("ImpactCatList");
            }
            return View(cat);
        }

        public ActionResult ImpactCatEdit(int id)
        {
            return View(db.ImpactCats.Single(p => p.ImpactCatId == id));
        }

        [HttpPost]
        public ActionResult ImpactCatEdit(ImpactCat cat)
        {
            if (ModelState.IsValid)
            {
                db.ImpactCats.Attach(cat);
                db.ObjectStateManager.ChangeObjectState(cat, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("ImpactCatList");
            }
            return View(cat);
        }

        public ActionResult ImpactCatDelete(int id)
        {
            return View(db.ImpactCats.Single(p => p.ImpactCatId == id));
        }

        [HttpPost, ActionName("ImpactCatDelete")]
        public ActionResult ImpactCatDeleteConfirmed(int id)
        {
            ImpactCat cat = db.ImpactCats.Single(p => p.ImpactCatId == id);
            db.ImpactCats.DeleteObject(cat);
            db.SaveChanges();
            return RedirectToAction("ImpactCatList");
        }

        #endregion

        #region ImpactType
        
        public ActionResult ImpactTypeList(int catId)
        {
            ImpactTypeViewModel vm = new ImpactTypeViewModel();
            vm.ImpactCat = db.ImpactCats.Single(p => p.ImpactCatId == catId);
            vm.ImpactTypes = db.ImpactTypes.Where(p => p.ImpactCatId == catId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult ImpactTypeNew(int catId)
        {
            ImpactTypeViewModel vm = new ImpactTypeViewModel();
            vm.ImpactCat = db.ImpactCats.Single(p => p.ImpactCatId == catId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ImpactTypeNew(ImpactTypeViewModel vm, int catId)
        {
            if (ModelState.IsValid)
            {
                vm.ImpactType.ImpactCatId = catId;
                db.ImpactTypes.AddObject(vm.ImpactType);
                db.SaveChanges();

                InsertImpactDetails(vm.ImpactType);

                return RedirectToAction("ImpactTypeList", new { catId = catId });
            }
            vm.ImpactCat = db.ImpactCats.Single(p => p.ImpactCatId == catId);
            return View(vm);
        }

        private void InsertImpactDetails(ImpactType type)
        {
            var typeId = type.ImpactTypeId;
            int detailId;
            foreach (var level in db.ImpactLevels)
            {
                string s = typeId.ToString() + level.ImpactLevelId.ToString();
                detailId = int.Parse(s);

                ImpactDetail detail = db.ImpactDetails.Where(p => p.ImpactTypeId == typeId && p.ImpactLevelId == level.ImpactLevelId).FirstOrDefault();
                if (detail == null || detail.ImpactDetailId != detailId)
                {
                    if (detail != null && detail.ImpactDetailId != detailId)
                        db.ImpactDetails.DeleteObject(detail);
                    detail = new ImpactDetail();
                    detail.ImpactDetailId = detailId;
                    detail.ImpactTypeId = typeId;
                    detail.ImpactLevelId = level.ImpactLevelId;
                    detail.ImpactDetailName = "...";
                    db.ImpactDetails.AddObject(detail);
                }
            }
            db.SaveChanges();
        }

        public ActionResult ImpactTypeEdit(int id)
        {
            ImpactTypeViewModel vm = new ImpactTypeViewModel();
            vm.ImpactType = db.ImpactTypes.Single(p => p.ImpactTypeId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ImpactTypeEdit(ImpactTypeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.ImpactTypes.Attach(vm.ImpactType);
                db.ObjectStateManager.ChangeObjectState(vm.ImpactType, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("ImpactTypeList", new { catId = vm.ImpactType.ImpactCatId });
            }
            return View(vm);
        }

        public ActionResult ImpactTypeDelete(int id)
        {
            ImpactTypeViewModel vm = new ImpactTypeViewModel();
            vm.ImpactType = db.ImpactTypes.Single(p => p.ImpactTypeId == id);
            return View(vm);
        }

        [HttpPost, ActionName("ImpactTypeDelete")]
        public ActionResult ImpactTypeDeleteConfirmed(int id)
        {
            ImpactType type = db.ImpactTypes.Single(p => p.ImpactTypeId == id);
            var catId = type.ImpactCatId;
            db.ImpactTypes.DeleteObject(type);
            db.SaveChanges();
            return RedirectToAction("ImpactTypeList", new { catId = catId });
        }

        #endregion

        #region ImpactDetail
        
        public ActionResult ImpactDetailList(int typeId)
        {
            ImpactDetailViewModel vm = new ImpactDetailViewModel();
            vm.ImpactType = db.ImpactTypes.Single(p => p.ImpactTypeId == typeId);
            vm.ImpactDetails = db.ImpactDetails.Where(p => p.ImpactTypeId == typeId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult ImpactDetailEdit(int id)
        {
            ImpactDetailViewModel vm = new ImpactDetailViewModel();
            vm.ImpactDetail = db.ImpactDetails.Single(p => p.ImpactDetailId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ImpactDetailEdit(ImpactDetailViewModel vm)
        {
            if (ModelState.IsValid)
            {
                db.ImpactDetails.Attach(vm.ImpactDetail);
                db.ObjectStateManager.ChangeObjectState(vm.ImpactDetail, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("ImpactDetailList", new { typeId = vm.ImpactDetail.ImpactTypeId });
            }
            return View(vm);
        }

        public ActionResult TreeView()
        {
            //IQueryable<ImpactDetail> details = db.ImpactDetails.OrderBy(p => p.ImpactTypeId).ThenBy(p => p.ImpactDetailId);
            return View(db.ImpactCats);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
