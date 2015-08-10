using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskrindoMVC.Areas.Admin.Models.UserMaintenance
{
    public class AskrindoUser
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsRiskOwner { get; set; }
        public bool IsLocked { get; set; }
    }

    public class UserViewModel
    {
        public List<AskrindoUser> UserList = new List<AskrindoUser>();
    }
}