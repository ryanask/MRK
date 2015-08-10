using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using AskrindoMVC.Areas.Admin.Models;
using AskrindoMVC.Helpers;
using System.Web.Security;
using System.Transactions;

namespace AskrindoMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList(bool? showjobTitle)
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
                using (TransactionScope trans = new TransactionScope())
                {
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
                        trans.Complete();
                        return RedirectToAction("UserList");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }
                }
            }
            GetOrgPosInfo(divisionId, subDivId, subDeptId, branchId, subBranchId, bizUnitId);
            return View(model);
        }

        public ActionResult UserEdit(Guid userId)
        {
            MembershipUser usr = Membership.GetUser(userId);
            UserInfo nfo = db.UserInfos.Single(p => p.UserId == userId);
            EditUserModel model = new EditUserModel();
            model.UserName = nfo.aspnet_User.UserName;
            model.FullName = nfo.FullName;
            model.JobTitle = nfo.JobTitle;
            model.Email = usr.Email;
            model.IsRCP = nfo.IsRiskOwner;

            GetOrgPosInfo(nfo);
            return View(model);
        }

        [HttpPost]
        public ActionResult UserEdit(EditUserModel model, Guid userId)
        {
            UserInfo nfo;
            if (ModelState.IsValid)
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    MembershipUser usr = Membership.GetUser(userId);
                    usr.Email = model.Email;
                    Membership.UpdateUser(usr);

                    nfo = db.UserInfos.Single(p => p.UserId == userId);
                    nfo.FullName = model.FullName;
                    nfo.JobTitle = model.JobTitle;
                    db.SaveChanges();
                    
                    trans.Complete();
                    return RedirectToAction("UserList");
                }
            }
            nfo = db.UserInfos.Single(p => p.UserId == userId);
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
                    //||
                    //string.Compare(usr.UserName.ToLower(), Utils.S_ADMINMR.ToLower()) == 0)
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
                    return RedirectToAction("UserList");
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
                return RedirectToAction("UserList");
            }

            UserInfo nfo = db.UserInfos.Single(p => p.UserId == userId);
            GetOrgPosInfo(nfo);
            return View(model);
        }

        //public ActionResult UnlockUser()
        //{
        //    UserViewModel vm = new UserViewModel();
        //    vm.UserList = new List<Models.AskrindoUser>();

        //    var users = db.UserInfos.OrderBy(p => p.FullName);
        //    foreach (var u in users)
        //    {
        //        MembershipUser usr = Membership.GetUser(u.aspnet_User.UserName);
        //        if (usr != null)
        //        {
        //            vm.UserList.Add(new AskrindoUser() { UserId = u.UserId, UserName = u.aspnet_User.UserName, FullName = u.FullName, IsLocked = usr.IsLockedOut });
        //        }
        //    }
        //    return View(vm);
        //}

        //public ActionResult AskrindoUserList()
        //{
        //    UserViewModel vm = new UserViewModel();
        //    vm.UserList = new List<Models.AskrindoUser>();

        //    var users = db.UserInfos.OrderBy(p => p.FullName);
        //    foreach (var u in users)
        //    {
        //        MembershipUser usr = Membership.GetUser(u.aspnet_User.UserName);
        //        if (usr != null)
        //        {
        //            vm.UserList.Add(new AskrindoUser() { UserId = u.UserId, UserName = u.aspnet_User.UserName, FullName = u.FullName, IsLocked = usr.IsLockedOut });
        //        }
        //    }
        //    return View(vm);
        //}

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
    }
}
