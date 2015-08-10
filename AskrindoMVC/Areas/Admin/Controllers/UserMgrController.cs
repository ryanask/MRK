using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using AskrindoMVC.Models;
using System.Web.Security;
using AskrindoMVC.Helpers;
using AskrindoMVC.Areas.Admin.Models.UserMgr;
using System.Transactions;

namespace AskrindoMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserMgrController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index(bool? showjobTitle)
        {
            return View(db.Depts);
        }

        public ActionResult UserNew(int orgPos, int? divisionId, int? subDivId, int? subDeptId, int? branchId, int? subBranchId, int? bizUnitId)
        {
            GetOrgPosInfo(divisionId, subDivId, subDeptId, branchId, subBranchId, bizUnitId);
            return View();
        }

        [HttpPost]
        public ActionResult UserNew(UserModel model, int orgPos, int? divisionId, int? subDivId, int? subDeptId, int? branchId, int? subBranchId, int? bizUnitId)
        {
            if (ModelState.IsValid)
            {
                //using (TransactionScope trans = new TransactionScope())
                //{
                    MembershipCreateStatus createStatus;
                    Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        MembershipUser usr = Membership.GetUser(model.UserName);
                        Guid userId = (Guid)usr.ProviderUserKey;

                        UserInfo nfo = new UserInfo();
                        nfo.UserId = userId;
                        nfo.FullName = model.FullName;
                        nfo.JobTitle = model.JobTitle;
                        nfo.IsRiskOwner = model.IsRCP;
                        nfo.OrgPos = orgPos;
                        switch (orgPos)
                        {
                            case Utils.ORGPOS_DIVISION:
                                Division div = db.Divisions.Single(p => p.DivisionId == divisionId);
                                nfo.DivisionId = divisionId;
                                nfo.DeptId = div.DeptId;
                                break;
                            case Utils.ORGPOS_SUBDEPT:
                                SubDept subDept = db.SubDepts.Single(p => p.SubDeptId == subDeptId);
                                nfo.SubDeptId = subDeptId;
                                nfo.DeptId = subDept.DeptId;
                                break;
                            case Utils.ORGPOS_SUBDIV:
                                SubDiv subDiv = db.SubDivs.Single(p => p.SubDivId == subDivId);
                                nfo.SubDivId = subDivId;
                                nfo.DivisionId = subDiv.DivisionId;
                                nfo.DeptId = subDiv.Division.DeptId;
                                break;
                            case Utils.ORGPOS_BRANCH:
                                Branch branch = db.Branches.Single(p => p.BranchId == branchId);
                                nfo.BranchId = branchId;
                                nfo.DeptId = branch.DeptId;
                                break;
                            case Utils.ORGPOS_SUBBRANCH:
                                SubBranch subBranch = db.SubBranches.Single(p => p.SubBranchId == subBranchId);
                                nfo.SubBranchId = subBranchId;
                                nfo.BranchId = subBranch.BranchId;
                                nfo.DeptId = subBranch.Branch.DeptId;
                                break;
                            case Utils.ORGPOS_BIZUNIT:
                                BizUnit biz = db.BizUnits.Single(p => p.BizUnitId == bizUnitId);
                                nfo.BizUnitId = bizUnitId;
                                nfo.BranchId = biz.BranchId;
                                nfo.DeptId = biz.Branch.DeptId;
                                break;
                        }
                        db.UserInfos.AddObject(nfo);
                        db.SaveChanges();
                        //trans.Complete();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }
                //}
            }
            GetOrgPosInfo(divisionId, subDivId, subDeptId, branchId, subBranchId, bizUnitId);
            return View(model);
        }

        public ActionResult UserEdit(Guid userId)
        {
            MembershipUser usr = Membership.GetUser(userId);
            UserInfo nfo = db.UserInfos.Single(p => p.UserId == userId);
            EditUserModelNew model = new EditUserModelNew();
            model.UserName = nfo.aspnet_User.UserName;
            model.FullName = nfo.FullName;
            model.JobTitle = nfo.JobTitle;
            model.Email = usr.Email;
            model.IsRCP = nfo.IsRiskOwner;
 
            GetOrgPosInfo(nfo);
            return View(model);
        }

        public ActionResult UserDelete(Guid userId)
        {
            MembershipUser usr = Membership.GetUser(userId);
            UserInfo nfo = db.UserInfos.Single(p => p.UserId == userId);
            DeleteUserModel model = new DeleteUserModel();
            model.UserName = nfo.aspnet_User.UserName;
            model.FullName = nfo.FullName;
            model.JobTitle = nfo.JobTitle;
            model.Email = usr.Email;
            model.IsRCP = nfo.IsRiskOwner;

            GetOrgPosInfo(nfo);
            return View(model);
        }

        [HttpPost, ActionName("UserDelete")]
        public ActionResult UserDeleteConfirmed(DeleteUserModel model, Guid userId)
        {
            UserInfo nfo;
            using (TransactionScope trans = new TransactionScope())
            {
                MembershipUser usr = Membership.GetUser(userId);
                if (string.Compare(usr.UserName.ToLower(), Utils.S_ADMIN.ToLower()) == 0)
                {
                    ModelState.AddModelError("", "Tidak bisa menghapus user");
                    nfo = db.UserInfos.Single(p => p.UserId == userId);
                    GetOrgPosInfo(nfo);
                    return View(model);
                }

                try
                {
                    Membership.DeleteUser(usr.UserName);
                    trans.Complete();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Tidak bisa menghapus user. Error: " + e.Message);
                }
            }
            nfo = db.UserInfos.Single(p => p.UserId == userId);
            GetOrgPosInfo(nfo);
            return View(model);
        }

        public ActionResult ChangeUserPassword(Guid userId)
        {
            UserInfo nfo = db.UserInfos.Single(p => p.UserId == userId);
            ChangeUserPasswordModel model = new ChangeUserPasswordModel();
            model.UserName = nfo.aspnet_User.UserName;
            model.FullName = nfo.FullName;
            model.JobTitle = nfo.JobTitle;
            model.IsRCP = nfo.IsRiskOwner;
            MembershipUser usr = Membership.GetUser(userId);
            model.Email = usr.Email;

            GetOrgPosInfo(nfo);
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeUserPassword(ChangeUserPasswordModel model, Guid userId)
        {
            if (ModelState.IsValid)
            {
                MembershipUser usr = Membership.GetUser(userId);
                usr.ChangePassword(usr.ResetPassword(), model.Password);
                Membership.UpdateUser(usr);
                return RedirectToAction("Index");
            }

            UserInfo nfo = db.UserInfos.Single(p => p.UserId == userId);
            GetOrgPosInfo(nfo);
            return View(model);
        }

        #region Status Codes

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion

        #region Helpers

        private void GetOrgPosInfo(int? divisionId, int? subDivId, int? subDeptId, int? branchId, int? subBranchId, int? bizUnitId)
        {
            List<KeyValuePair<string, string>> orgList = new List<KeyValuePair<string, string>>();
            if (divisionId != null)
                GetDivisionInfo(divisionId, orgList);
            else if (subDivId != null)
                GetSubDivInfo(subDivId, orgList);
            else if (subDeptId != null)
                GetSubDeptInfo(subDeptId, orgList);
            else if (branchId != null)
                GetBranchInfo(branchId, orgList);
            else if (subBranchId != null)
                GetSubBranchInfo(subBranchId, orgList);
            else if (bizUnitId != null)
                GetBizUnitInfo(bizUnitId, orgList);
            ViewBag.OrgList = orgList;
        }

        private void GetOrgPosInfo(UserInfo nfo)
        {
            List<KeyValuePair<string, string>> orgList = new List<KeyValuePair<string, string>>();
            switch (nfo.OrgPos)
            {
                case Utils.ORGPOS_DIVISION:
                    GetDivisionInfo(nfo.DivisionId, orgList);
                    break;
                case Utils.ORGPOS_SUBDIV:
                    GetSubDivInfo(nfo.SubDivId, orgList);
                    break;
                case Utils.ORGPOS_SUBDEPT:
                    GetSubDeptInfo(nfo.SubDeptId, orgList);
                    break;
                case Utils.ORGPOS_BRANCH:
                    GetBranchInfo(nfo.BranchId, orgList);
                    break;
                case Utils.ORGPOS_SUBBRANCH:
                    GetSubBranchInfo(nfo.SubBranchId, orgList);
                    break;
                case Utils.ORGPOS_BIZUNIT:
                    GetBizUnitInfo(nfo.BizUnitId, orgList);
                    break;
            }
            ViewBag.OrgList = orgList;
        }

        private void GetBizUnitInfo(int? bizUnitId, List<KeyValuePair<string, string>> orgList)
        {
            BizUnit biz = db.BizUnits.Single(p => p.BizUnitId == bizUnitId);
            orgList.Insert(0, new KeyValuePair<string, string>("KUP", biz.BizUnitName));
            orgList.Insert(0, new KeyValuePair<string, string>("Cabang", biz.Branch.BranchName));
        }

        private void GetSubBranchInfo(int? subBranchId, List<KeyValuePair<string, string>> orgList)
        {
            SubBranch sub = db.SubBranches.Single(p => p.SubBranchId == subBranchId);
            orgList.Insert(0, new KeyValuePair<string, string>("Bagian", sub.SubBranchName));
            orgList.Insert(0, new KeyValuePair<string, string>("Cabang", sub.Branch.BranchName));
        }

        private void GetBranchInfo(int? branchId, List<KeyValuePair<string, string>> orgList)
        {
            Branch branch = db.Branches.Single(p => p.BranchId == branchId);
            orgList.Insert(0, new KeyValuePair<string, string>("Cabang", branch.BranchName));
        }

        private void GetSubDeptInfo(int? subDeptId, List<KeyValuePair<string, string>> orgList)
        {
            SubDept sub = db.SubDepts.Single(p => p.SubDeptId == subDeptId);
            orgList.Insert(0, new KeyValuePair<string, string>("Bagian", sub.SubDeptName));
            orgList.Insert(0, new KeyValuePair<string, string>("Direktorat", sub.Dept.DeptName));
        }

        private void GetSubDivInfo(int? subDivId, List<KeyValuePair<string, string>> orgList)
        {
            SubDiv sub = db.SubDivs.Single(p => p.SubDivId == subDivId);
            orgList.Insert(0, new KeyValuePair<string, string>("Bagian", sub.SubDivName));
            orgList.Insert(0, new KeyValuePair<string, string>("Divisi", sub.Division.DivisionName));
            orgList.Insert(0, new KeyValuePair<string, string>("Direktorat", sub.Division.Dept.DeptName));
        }

        private void GetDivisionInfo(int? divisionId, List<KeyValuePair<string, string>> orgList)
        {
            Division div = db.Divisions.Single(p => p.DivisionId == divisionId);
            orgList.Insert(0, new KeyValuePair<string, string>("Divisi", div.DivisionName));
            orgList.Insert(0, new KeyValuePair<string, string>("Direktorat", div.Dept.DeptName));
        }

        #endregion

        public ActionResult UserEditNew(Guid userId)
        {

            MembershipUser usr = Membership.GetUser(userId);
            UserInfo nfo = db.UserInfos.Single(p => p.UserId == userId);
            EditUserModelNew model = new EditUserModelNew();
            model.UserName = nfo.aspnet_User.UserName;
            model.FullName = nfo.FullName;
            model.JobTitle = nfo.JobTitle;
            model.Email = usr.Email;
            model.IsRCP = nfo.IsRiskOwner;

            model.param = new EditUserParam();
            if (nfo.BranchId.Equals(null))
            {
                model.param.posID = 1;
                model.param.BranchID = null;
                model.param.DeptID = nfo.DeptId;
            } 
            else 
            {
                model.param.posID = 2;
                model.param.BranchID = nfo.BranchId;
                model.param.DeptID = 3;
            }
            
            model.param.DivisionID = nfo.DivisionId;
            model.param.SubDivID = nfo.SubDivId;
            model.param.SubBranchID = nfo.SubBranchId;
            model.param.BizUnitID = nfo.BizUnitId;
            model.param.IsRCP = nfo.IsRiskOwner;

            UpdateUserParam(model.param);
            GetOrgPosInfo(nfo);
            return View(model);
        }

        [HttpPost]
        public ActionResult UserEditNew(EditUserModelNew model, Guid userId, String btn="")
        {
            UserInfo nfo;

            //using (TransactionScope trans = new TransactionScope())
            //{
                if (btn.Equals("Update"))
                {
                    if (ModelState.IsValid)
                    {
                        MembershipUser usr = Membership.GetUser(userId);
                        usr.Email = model.Email;
                        if (model.Password != null)
                        {
                            usr.ChangePassword(usr.ResetPassword(), model.Password);
                        }
          
                        Membership.UpdateUser(usr);

                        nfo = db.UserInfos.Single(p => p.UserId == userId);

                        using (TransactionScope trans = new TransactionScope())
                        {
                            nfo.FullName = model.FullName;
                            nfo.JobTitle = model.JobTitle;
                            nfo.DeptId = model.param.DeptID;
                            nfo.BranchId = model.param.BranchID;
                            nfo.DivisionId = model.param.DivisionID;
                            nfo.SubDivId = model.param.SubDivID;
                            nfo.BranchId = model.param.BranchID;
                            nfo.SubBranchId = model.param.SubBranchID;
                            nfo.BizUnitId = model.param.BizUnitID;
                            nfo.IsRiskOwner = model.param.IsRCP;
                            db.SaveChanges();
                            trans.Complete();

                            return RedirectToAction("Index");
                        }
                    }
                } 
                else
                {
                    nfo = db.UserInfos.Single(p => p.UserId == userId);
                    GetOrgPosInfo(nfo);
                    UpdateUserParam(model.param);
                }
           //}
            return View(model);
        }

        private void UpdateUserParam(EditUserParam param){
            Dictionary<int, string> posList = new Dictionary<int, string>();
            posList.Add(1, "Kantor Pusat");
            posList.Add(2, "Cabang");
            param.Kantor = new SelectList(posList, "Key", "Value", 1);
                        
            Dictionary<int, string> DirektoratList = new Dictionary<int, string>();
            foreach (var Direktorat in db.Depts.OrderBy(m => m.DeptId).ThenBy(m => m.DeptName))
                DirektoratList.Add(Direktorat.DeptId, Direktorat.DeptName);
            param.Direktorat = new SelectList(DirektoratList, "Key", "Value", param.DeptID);
            
            Dictionary<int, string> DivisionList = new Dictionary<int, string>();
            foreach (var Division in db.Divisions.OrderBy(m => m.DivisionId).ThenBy(m => m.DivisionName).Where(m => m.DeptId == param.DeptID))
                DivisionList.Add(Division.DivisionId, Division.DivisionName);
            param.Divisi = new SelectList(DivisionList, "Key", "Value", param.DivisionID);

            Dictionary<int, string> BagianList = new Dictionary<int, string>();
            foreach (var Bagian in db.SubDivs.OrderBy(m => m.SubDivId).ThenBy(m => m.SubDivName).Where(m => m.DivisionId == param.DivisionID))
                BagianList.Add(Bagian.SubDivId, Bagian.SubDivName);
            param.Bagian = new SelectList(BagianList, "Key", "Value", param.SubDivID);

            Dictionary<int, string> CabangList = new Dictionary<int, string>();
            foreach (var Cabang in db.Branches.OrderBy(m => m.BranchId).ThenBy(m => m.BranchName))
                CabangList.Add(Cabang.BranchId, Cabang.BranchName);
            param.Cabang = new SelectList(CabangList, "Key", "Value", param.BranchID);

            Dictionary<int, string> SubCabangList = new Dictionary<int, string>();
            foreach (var SubCabang in db.SubBranches.OrderBy(m => m.SubBranchId).ThenBy(m => m.SubBranchName).Where(m => m.BranchId == param.BranchID))
                SubCabangList.Add(SubCabang.SubBranchId, SubCabang.SubBranchName);
            param.SubCabang = new SelectList(SubCabangList, "Key", "Value", param.SubBranchID);

            Dictionary<int, string> UnitList = new Dictionary<int, string>();
            foreach (var Unit in db.BizUnits.OrderBy(m => m.BizUnitId).ThenBy(m => m.BizUnitName).Where(m => m.BranchId == param.BranchID))
                UnitList.Add(Unit.BizUnitId, Unit.BizUnitName);
            param.Unit = new SelectList(UnitList, "Key", "Value", param.BizUnitID);

            Dictionary<int, string> UserGroupList = new Dictionary<int, string>();
            foreach (var UserGroup in db.UserGroups.OrderBy(m => m.UserGroupID).ThenBy(m => m.UserGroup1))
                UserGroupList.Add(UserGroup.UserGroupID, UserGroup.UserGroup1);
            param.UserGroup = new SelectList(UserGroupList, "Key", "Value", param.UserGroupID);

        }

        #region JSON

        [HttpGet]
        public JsonResult Department(string DeptId)
        {
            int id = string.IsNullOrEmpty(DeptId) ? 0 : Convert.ToInt32(DeptId);
            var Depts = db.Divisions.Where(p => p.DeptId == id).ToList();
            var selectItems = Depts.Select(m => new SelectListItem()
            {
                Text = m.DivisionName,
                Value = m.DivisionId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Division(string DivId)
        {
            int id = string.IsNullOrEmpty(DivId) ? 0 : Convert.ToInt32(DivId);
            var Div = db.SubDivs.Where(p => p.DivisionId == id).ToList();
            var selectItems = Div.Select(p => new SelectListItem()
            {
                Text = p.SubDivName,
                Value = p.SubDivId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Branch(string BranchId)
        {
            int id = string.IsNullOrEmpty(BranchId) ? 0 : Convert.ToInt32(BranchId);
            var Branch = db.SubBranches.Where(p => p.BranchId == id).ToList();
            var selectItems = Branch.Select(p => new SelectListItem()
            {
                Text = p.SubBranchName,
                Value = p.SubBranchId.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        } 

        [HttpGet]
        public JsonResult Unit(string BranchId)
        {
            int id = string.IsNullOrEmpty(BranchId) ? 0 : Convert.ToInt32(BranchId);
            var Branch = db.BizUnits.Where(p => p.BranchId == id).ToList();
            var selectItems = Branch.Select(p => new SelectListItem()
            {
                Text = p.BizUnitName,
                Value = p.BizUnitName.ToString()
            });
            return Json(selectItems, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}