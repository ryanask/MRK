using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using System.Data;
using AskrindoMVC.Areas.RefData.Models.OrgStruct;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Areas.RefData.Controllers
{
    [Authorize]
    public class OrgStructController : Controller
    {
        private AskrindoMVCEntities db = new AskrindoMVCEntities();
        UserData userData = Utils.LoadUserDataFromSession();

        public ActionResult Index()
        {
            return View();
        }

        #region Dept

        public ActionResult DeptList()
        {
            ViewBag.CanModify = userData.IsAdmin;
            return View(db.Depts.ToList());
        }

        public ActionResult DeptEdit(int id)
        {
            Dept dept = db.Depts.Single(p => p.DeptId == id);
            return View(dept);
        }

        [HttpPost]
        public ActionResult DeptEdit(Dept dept)
        {
            if (ModelState.IsValid)
            {
                db.Depts.Attach(dept);
                db.ObjectStateManager.ChangeObjectState(dept, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("DeptList");
            }
            return View(dept);
        }

        public ActionResult DeptNew()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeptNew(Dept dept)
        {
            if (ModelState.IsValid)
            {
                db.Depts.AddObject(dept);
                db.SaveChanges();
                return RedirectToAction("DeptList");
            }
            return View(dept);
        }

        public ActionResult DeptDelete(int id)
        {
            Dept dept = db.Depts.Single(p => p.DeptId == id);
            return View(dept);
        }

        [HttpPost, ActionName("DeptDelete")]
        public ActionResult DeptDeleteConfirmed(int id)
        {
            Dept dept = db.Depts.Single(p => p.DeptId == id);
            db.Depts.DeleteObject(dept);
            db.SaveChanges();
            return RedirectToAction("DeptList");
        }

        #endregion

        #region Division

        public ActionResult DivList(int deptId)
        {
            ViewBag.Dept = db.Depts.Single(p => p.DeptId == deptId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(db.Divisions.Where(p => p.DeptId == deptId).ToList());
        }

        public ActionResult DivNew(int deptId)
        {
            ViewBag.Dept = db.Depts.Single(p => p.DeptId == deptId);
            return View();
        }

        [HttpPost]
        public ActionResult DivNew(Division div, int deptId)
        {
            if (ModelState.IsValid)
            {
                div.DeptId = deptId;
                db.Divisions.AddObject(div);
                db.SaveChanges();
                return RedirectToAction("DivList", new { deptid = deptId });
            }
            return View(new { deptid = deptId });
        }

        public ActionResult DivEdit(int id)
        {
            Division div = db.Divisions.Single(p => p.DivisionId == id);
            return View(div);
        }

        [HttpPost]
        public ActionResult DivEdit(Division div)
        {
            if (ModelState.IsValid)
            {
                db.Divisions.Attach(div);
                db.ObjectStateManager.ChangeObjectState(div, EntityState.Modified);
                db.SaveChanges();

                ViewBag.DeptId = div.DeptId;
                Dept dept = db.Depts.Single(p => p.DeptId == div.DeptId);
                ViewBag.DeptName = dept.DeptName;
                return RedirectToAction("DivList", new { deptid = div.DeptId });
            }
            return View(div);
        }

        public ActionResult DivDelete(int id)
        {
            Division div = db.Divisions.Single(p => p.DivisionId == id);
            ViewBag.DeptId = div.DeptId;
            ViewBag.DeptName = div.Dept.DeptName;
            return View(div);
        }

        [HttpPost, ActionName("DivDelete")]
        public ActionResult DivDeleteConfirmed(int id)
        {
            Division div = db.Divisions.Single(p => p.DivisionId == id);
            ViewBag.DeptId = div.DeptId;
            ViewBag.DeptName = div.Dept.DeptName;
            db.Divisions.DeleteObject(div);
            db.SaveChanges();
            return RedirectToAction("DivList", new { deptid = ViewBag.DeptId });
        }

        #endregion

        #region Branch
        
        public ActionResult BranchList(int deptId)
        {
            ViewBag.Dept = db.Depts.Single(p => p.DeptId == deptId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(db.Branches.Include("BranchClass").Where(p => p.DeptId == deptId));
        }

        public ActionResult BranchNew(int deptId)
        {
            ViewBag.Dept = db.Depts.Single(p => p.DeptId == deptId);
            ViewBag.ClassId = new SelectList(db.BranchClasses, "ClassId", "ClassName");
            return View();
        }

        [HttpPost]
        public ActionResult BranchNew(Branch branch, int deptId)
        {
            if (ModelState.IsValid)
            {
                branch.DeptId = deptId;
                db.Branches.AddObject(branch);
                db.SaveChanges();
                return RedirectToAction("BranchList", new { deptId = deptId });
            }
            //if (branch.ClassId == null)
            //    ModelState.AddModelError("", "Kelas cabang harus diisi");
            
            ViewBag.Dept = db.Depts.Single(p => p.DeptId == deptId);
            ViewBag.ClassId = new SelectList(db.BranchClasses, "ClassId", "ClassName");
            return View(branch);
        }

        public ActionResult BranchEdit(int id)
        {
            Branch branch = db.Branches.Single(p => p.BranchId == id);
            ViewBag.ClassId = new SelectList(db.BranchClasses, "ClassId", "ClassName", branch.ClassId);
            return View(branch);
        }

        [HttpPost]
        public ActionResult BranchEdit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Branches.Attach(branch);
                db.ObjectStateManager.ChangeObjectState(branch, EntityState.Modified);
                db.SaveChanges();

                ViewBag.Dept = db.Depts.Single(p => p.DeptId == branch.DeptId);
                return RedirectToAction("BranchList", new { deptId = branch.DeptId });
            }
            return View(branch);
        }

        public ActionResult BranchDelete(int id)
        {
            Branch branch = db.Branches.Single(p => p.BranchId == id);
            return View(branch);
        }

        [HttpPost, ActionName("BranchDelete")]
        public ActionResult BranchDeleteConfirmed(int id)
        {
            Branch branch = db.Branches.Single(p => p.BranchId == id);
            db.Branches.DeleteObject(branch);
            db.SaveChanges();
            return RedirectToAction("BranchList", new { deptId = branch.DeptId });
        }

        #endregion

        #region SubDiv
        
        public ActionResult SubDivList(int divId)
        {
            SubDivViewModel vm = new SubDivViewModel();
            vm.Division = db.Divisions.Single(p => p.DivisionId == divId);
            vm.SubDivs = db.SubDivs.Where(p => p.DivisionId == divId).ToList();
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult SubDivNew(int divId)
        {
            SubDivViewModel vm = new SubDivViewModel();
            vm.Division = db.Divisions.Single(p => p.DivisionId == divId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SubDivNew(SubDivViewModel vm, int divId)
        {
            if (ModelState.IsValid)
            {
                vm.SubDiv.DivisionId = divId;
                db.SubDivs.AddObject(vm.SubDiv);
                db.SaveChanges();
                return RedirectToAction("SubDivList", new { divId = divId });
            }
            vm.Division = db.Divisions.Single(p => p.DivisionId == divId);
            return View(vm);
        }

        public ActionResult SubDivEdit(int id)
        {
            SubDivViewModel vm = new SubDivViewModel();
            vm.SubDiv = db.SubDivs.Single(p => p.SubDivId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SubDivEdit(SubDiv subDiv)
        {
            if (ModelState.IsValid)
            {
                db.SubDivs.Attach(subDiv);
                db.ObjectStateManager.ChangeObjectState(subDiv, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("SubDivList", new { divId = subDiv.DivisionId });
            }
            SubDivViewModel vm = new SubDivViewModel();
            vm.SubDiv = subDiv;
            return View(vm);
        }

        public ActionResult SubDivDelete(int id)
        {
            SubDivViewModel vm = new SubDivViewModel();
            vm.SubDiv = db.SubDivs.Include("Division").Single(p => p.SubDivId == id);
            return View(vm);
        }

        [HttpPost, ActionName("SubDivDelete")]
        public ActionResult SubDivDeleteConfirmed(int id)
        {
            SubDiv subDiv = db.SubDivs.Single(p => p.SubDivId == id);
            int divId = subDiv.DivisionId;
            db.SubDivs.DeleteObject(subDiv);
            db.SaveChanges();
            return RedirectToAction("SubDivList", new { divId = divId });
        }

        #endregion

        #region BizUnit
        
        public ActionResult BizUnitList(int branchId)
        {
            BizUnitViewModel vm = new BizUnitViewModel();
            vm.BizUnits = db.BizUnits.Where(p => p.BranchId == branchId);
            vm.Branch = db.Branches.Single(p => p.BranchId == branchId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult BizUnitNew(int branchId)
        {
            BizUnitViewModel vm = new BizUnitViewModel();
            vm.Branch = db.Branches.Single(p => p.BranchId == branchId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult BizUnitNew(BizUnitViewModel vm, int branchId)
        {
            if (ModelState.IsValid)
            {
                vm.BizUnit.BranchId = branchId;
                db.BizUnits.AddObject(vm.BizUnit);
                db.SaveChanges();
                return RedirectToAction("BizUnitList", new { branchId = branchId });
            }
            vm.Branch = db.Branches.Single(p => p.BranchId == branchId);
            return View(vm);
        }

        public ActionResult BizUnitEdit(int id)
        {
            BizUnitViewModel vm = new BizUnitViewModel();
            vm.BizUnit = db.BizUnits.Include("Branch").Single(p => p.BizUnitId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult BizUnitEdit(BizUnit bizUnit)
        {
            if (ModelState.IsValid)
            {
                db.BizUnits.Attach(bizUnit);
                db.ObjectStateManager.ChangeObjectState(bizUnit, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("BizUnitList", new { branchId = bizUnit.BranchId });
            }
            BizUnitViewModel vm = new BizUnitViewModel();
            vm.BizUnit = bizUnit;
            return View(vm);
        }

        public ActionResult BizUnitDelete(int id)
        {
            BizUnitViewModel vm = new BizUnitViewModel();
            vm.BizUnit = db.BizUnits.Include("Branch").Single(p => p.BizUnitId == id);
            return View(vm);
        }

        [HttpPost, ActionName("BizUnitDelete")]
        public ActionResult BizUnitDeleteConfirmed(int id)
        {
            BizUnit bizUnit = db.BizUnits.Single(p => p.BizUnitId == id);
            var branchId = bizUnit.BranchId;
            db.BizUnits.DeleteObject(bizUnit);
            db.SaveChanges();
            return RedirectToAction("BizUnitList", new { branchId = branchId });
        }

        #endregion

        #region SubBranch
        
        public ActionResult SubBranchList(int branchId)
        {
            SubBranchViewModel vm = new SubBranchViewModel();
            vm.Branch = db.Branches.Single(p => p.BranchId == branchId);
            vm.SubBranches = db.SubBranches.Where(p => p.BranchId == branchId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult SubBranchNew(int branchId)
        {
            SubBranchViewModel vm = new SubBranchViewModel();
            vm.Branch = db.Branches.Single(p => p.BranchId == branchId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SubBranchNew(SubBranchViewModel vm, int branchId)
        {
            if (ModelState.IsValid)
            {
                vm.SubBranch.BranchId = branchId;
                db.SubBranches.AddObject(vm.SubBranch);
                db.SaveChanges();
                return RedirectToAction("SubBranchList", new { branchId = branchId });
            }
            vm.Branch = db.Branches.Single(p => p.BranchId == branchId);
            return View(vm);
        }

        public ActionResult SubBranchEdit(int id)
        {
            SubBranchViewModel vm = new SubBranchViewModel();
            vm.SubBranch = db.SubBranches.Single(p => p.SubBranchId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SubBranchEdit(SubBranch subBranch, int id)
        {
            if (ModelState.IsValid)
            {
                db.SubBranches.Attach(subBranch);
                db.ObjectStateManager.ChangeObjectState(subBranch, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("SubBranchList", new { branchId = subBranch.BranchId });
            }
            SubBranchViewModel vm = new SubBranchViewModel();
            vm.SubBranch = subBranch;
            return View();
        }

        public ActionResult SubBranchDelete(int id)
        {
            SubBranchViewModel vm = new SubBranchViewModel();
            vm.SubBranch = db.SubBranches.Single(p => p.SubBranchId == id);
            return View(vm);
        }

        [HttpPost, ActionName("SubBranchDelete")]
        public ActionResult SubBranchDeleteConfirmed(int id)
        {
            SubBranch subBranch = db.SubBranches.Single(p => p.SubBranchId == id);
            var branchId=subBranch.BranchId;
            db.SubBranches.DeleteObject(subBranch);
            db.SaveChanges();
            return RedirectToAction("SubBranchList", new { branchId = branchId });
        }

        #endregion

        #region SubDept

        public ActionResult SubDeptList(int deptId)
        {
            SubDeptViewModel vm = new SubDeptViewModel();
            vm.Dept = db.Depts.Single(p => p.DeptId == deptId);
            vm.SubDepts = db.SubDepts.Where(p => p.DeptId == deptId);
            ViewBag.CanModify = userData.IsAdmin;
            return View(vm);
        }

        public ActionResult SubDeptNew(int deptId)
        {
            SubDeptViewModel vm = new SubDeptViewModel();
            vm.Dept = db.Depts.Single(p => p.DeptId == deptId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SubDeptNew(SubDeptViewModel vm, int deptId)
        {
            if (ModelState.IsValid)
            {
                vm.SubDept.DeptId = deptId;
                db.SubDepts.AddObject(vm.SubDept);
                db.SaveChanges();
                return RedirectToAction("SubDeptList", new { deptId = deptId });
            }
            vm.Dept = db.Depts.Single(p => p.DeptId == deptId);
            return View(vm);
        }

        public ActionResult SubDeptEdit(int id)
        {
            SubDeptViewModel vm = new SubDeptViewModel();
            vm.SubDept = db.SubDepts.Single(p => p.SubDeptId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SubDeptEdit(SubDeptViewModel vm, int id)
        {
            if (ModelState.IsValid)
            {
                db.SubDepts.Attach(vm.SubDept);
                db.ObjectStateManager.ChangeObjectState(vm.SubDept, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("SubDeptList", new { deptId = vm.SubDept.DeptId });
            }
            return View(vm);
        }

        public ActionResult SubDeptDelete(int id)
        {
            SubDeptViewModel vm = new SubDeptViewModel();
            vm.SubDept = db.SubDepts.Single(p => p.SubDeptId == id);
            return View(vm);
        }

        [HttpPost, ActionName("SubDeptDelete")]
        public ActionResult SubDeptDeleteConfirmed(int id)
        {
            SubDept sub = db.SubDepts.Single(p => p.SubDeptId == id);
            var deptId = sub.DeptId;
            db.SubDepts.DeleteObject(sub);
            db.SaveChanges();
            return RedirectToAction("SubDeptList", new { deptId = deptId });
        }

        #endregion

        public ActionResult TreeView()
        {
            var depts = db.Depts;
            return View(depts);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
