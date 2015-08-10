using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Areas.Admin.Models.UserMaintenance;
using System.Web.Security;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserMaintenanceController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        public ActionResult Index()
        {
            UserViewModel vm = new UserViewModel();
            vm.UserList = new List<AskrindoUser>();

            var users = db.UserInfos.OrderBy(p => p.FullName);
            foreach (var u in users)
            {
                MembershipUser usr = Membership.GetUser(u.aspnet_User.UserName);
                if (usr != null)
                {
                    vm.UserList.Add(new AskrindoUser() { UserId = u.UserId, 
                        UserName = u.aspnet_User.UserName, 
                        FullName = u.FullName, 
                        LastLoginDate = u.aspnet_User.aspnet_Membership.LastLoginDate,
                        IsRiskOwner = u.IsRiskOwner,
                        IsLocked = usr.IsLockedOut });
                }
            }
            return View(vm);
        }

        public ActionResult UnlockUser(Guid userId)
        {
            MembershipUser usr = Membership.GetUser(userId);
            if (usr == null)
            {
                ViewBag.Message = "Data user tidak ditemukan";
                return View("Error");
            }
            return View(db.UserInfos.Single(p => p.UserId == userId));
        }

        [HttpPost, ActionName("UnlockUser")]
        public ActionResult UnlockUserConfirmed(Guid userId)
        {
            MembershipUser usr = Membership.GetUser(userId);
            if (usr == null)
            {
                ViewBag.Message = "Tidak bisa meng-unlock user. Data user tidak ditemukan";
                return View("Error");
            }
            if (usr.UnlockUser())
            {
                Membership.UpdateUser(usr);
                ViewBag.Message = "User telah di-unlock";
                return View("Info");
            }
            else
            {
                ViewBag.Message = "Gagal meng-unlock user";
                return View("Error");
            }
        }
    }
}
