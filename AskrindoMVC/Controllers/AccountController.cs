using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AskrindoMVC.Models;
using AskrindoMVC.Helpers;

namespace AskrindoMVC.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //bool isUserValid = false;
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    //UserData data = new UserData();
                    //isUserValid = Utils.GetUserInfo(model.UserName, data);

                    //if (isUserValid)
                    if (isUserExist(model.UserName))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        //Session["Data"] = data;

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Nama atau password salah.");
                }
            }
            return View(model);
        }

        private bool isUserExist(string userName)
        {
            if (userName.ToLower() == Utils.S_ADMIN.ToLower())
                return true;
            else
            {
                AskrindoMVCEntities db = new AskrindoMVCEntities();
                var nfo = db.UserInfos.Where(p => p.aspnet_User.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
                return nfo != null;
            }
        }

        //private bool GetUserInfo(string userName, SessionData data)
        //{
        //    bool isUserValid = true;
        //    data.IsRiskOwner = false;
        //    data.IsAdmin = string.Compare(userName.ToLower(), Utils.S_ADMIN.ToLower()) == 0;
        //    data.IsAdminMR = string.Compare(userName.ToLower(), Utils.S_ADMINMR.ToLower()) == 0;
        //    if (data.IsAdmin)
        //        data.FullName = "Administrator";
        //    else if (data.IsAdminMR)
        //        data.FullName = "Administrator MR";
        //    else
        //    {
        //        MembershipUser usr = Membership.GetUser(userName);
        //        Guid userId = (Guid)usr.ProviderUserKey;
        //        AskrindoMVCEntities db = new AskrindoMVCEntities();
        //        UserInfo nfo = db.UserInfos.Single(p => p.UserId == userId);
        //        isUserValid = nfo != null;
        //        if (isUserValid)
        //        {
        //            data.FullName = nfo.FullName;
        //            data.JobTitle = nfo.JobTitle;
        //            data.OrgPos = nfo.OrgPos;
        //            data.IsRiskOwner = nfo.IsRiskOwner;

        //            switch(data.OrgPos){
        //                case Utils.ORGPOS_SUBDEPT:
        //                    data.OrgPosId = (int)nfo.SubDeptId;
        //                    SubDept subDept = db.SubDepts.Single(p => p.SubDeptId == nfo.SubDeptId);
        //                    data.OrgPosName = string.Format("Direktorat: {0}, Bagian: {1}",
        //                        subDept.Dept.DeptName, subDept.SubDeptName);
        //                    break;
        //                case Utils.ORGPOS_SUBDIV:
        //                    data.OrgPosId = (int)nfo.SubDivId;
        //                    SubDiv subDiv = db.SubDivs.Single(p => p.SubDivId == nfo.SubDivId);
        //                    data.OrgPosName = string.Format("Divisi: {0}, Bagian: {1}",
        //                        subDiv.Division.DivisionName, subDiv.SubDivName);
        //                    break;
        //                case Utils.ORGPOS_BRANCH:
        //                    data.OrgPosId = (int)nfo.BranchId;
        //                    Branch branch = db.Branches.Single(p => p.BranchId == nfo.BranchId);
        //                    data.OrgPosName = string.Format("Cabang: {0}", branch.BranchName);
        //                    break;
        //                case Utils.ORGPOS_SUBBRANCH:
        //                    data.OrgPosId = (int)nfo.SubBranchId;
        //                    SubBranch subBranch = db.SubBranches.Single(p => p.SubBranchId == nfo.SubBranchId);
        //                    data.OrgPosName = string.Format("Cabang: {0}, Bagian: {1}",
        //                        subBranch.Branch.BranchName, subBranch.SubBranchName);
        //                    break;
        //                case Utils.ORGPOS_BIZUNIT:
        //                    data.OrgPosId = (int)nfo.BizUnitId;
        //                    BizUnit biz = db.BizUnits.Single(p => p.BizUnitId == nfo.BizUnitId);
        //                    data.OrgPosName = string.Format("Cabang: {0}, KUP: {1}",
        //                        biz.Branch.BranchName, biz.BizUnitName);
        //                    break;
        //            }
        //        }
        //    }

        //    return isUserValid;
        //}

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["Data"] = null;
            return RedirectToAction("Index", "Home");
            //VirtualPathUtility.ToAbsolute()
        }

        //public ActionResult LogOn()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult LogOn(LogOnModel model, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (Membership.ValidateUser(model.UserName, model.Password))
        //        {
        //            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
        //            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
        //                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
        //            {
        //                return Redirect(returnUrl);
        //            }
        //            else
        //            {
        //                return RedirectToAction("Index", "Home");
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "The user name or password provided is incorrect.");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/LogOff

        //public ActionResult LogOff()
        //{
        //    FormsAuthentication.SignOut();

        //    return RedirectToAction("Index", "Home");
        //}

        //public ActionResult Register()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        MembershipCreateStatus createStatus;
        //        Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

        //        if (createStatus == MembershipCreateStatus.Success)
        //        {
        //            FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", ErrorCodeToString(createStatus));
        //        }
        //    }

        //}

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Password lama salah atau password baru tidak valid.");
                }
            }
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        //private void Test()
        //{
        //    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
        //    currentUser.is
        //}

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
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
    }
}
