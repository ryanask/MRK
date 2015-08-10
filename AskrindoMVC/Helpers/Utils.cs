using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using AskrindoMVC.Models;
using System.Globalization;
using System.Transactions;
using System.Web.Configuration;

namespace AskrindoMVC.Helpers
{
    public class UserData
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
        public int OrgPos { get; set; }
        public int OrgPosId { get; set; }
        public bool IsRiskOwner { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string OrgPosName { get; set; }
        public int? DeptId { get; set; }
        public int? SubDeptId { get; set; }
        public int? DivisionId { get; set; }
        public int? SubDivId { get; set; }
        public int? BranchId { get; set; }
        public int? SubBranchId { get; set; }
        public int? BizUnitId { get; set; }
        public int? BranchClassId { get; set; }
    }

    public class Utils
    {
        public const string S_ADMIN = "Admin";

        public const string S_ISADMIN = "IsAdmin";
        public const string S_ISADMINMR = "IsAdminMR";
        public const string S_ORGPOS = "OrgPos";
        public const string S_FULLNAME = "FullName";
        public const string S_JOBTITLE = "JobTitle";
        public const string S_POS1 = "Pos1";
        public const string S_POS2 = "Pos2";

        public const int ORGPOS_DEPT = 1;
        public const int ORGPOS_SUBDEPT = 2;
        public const int ORGPOS_DIVISION = 3;
        public const int ORGPOS_SUBDIV = 4;
        public const int ORGPOS_BRANCH = 5;
        public const int ORGPOS_SUBBRANCH = 6;
        public const int ORGPOS_BIZUNIT = 7;

        public const int BRANCHCLASS1 = 1;
        public const int BRANCHCLASS2 = 2;
        public const int BRANCHCLASS3 = 3;

        public const int PROBLEVEL1 = 1;
        public const int PROBLEVEL5 = 5;

        public const int IMPACTLEVEL1 = 1;
        public const int IMPACTLEVEL2 = 2;
        public const int IMPACTLEVEL3 = 3;
        public const int IMPACTLEVEL4 = 4;
        public const int IMPACTLEVEL5 = 5;

        public const int IMPACTPOS_HQ = 1;
        public const int IMPACTPOS_BRANCH1 = 2;
        public const int IMPACTPOS_BRANCH2 = 3;
        public const int IMPACTPOS_BRANCH3 = 4;
        public const int IMPACTPOS_BIZUNIT = 5;
        public const int IMPACTPOS_SUPPORTINGHQ = 6;
        public const int IMPACTPOS_SUPPORTINGBRANCH = 7;
        public const int IMPACTPOS_SUPPORTINGBIZUNIT = 8;

        public const int FREQUENCY1 = 1;

        public const int MAX_LIMIT_APPROVAL_DAYS = 3;

        public const int PROBOPTION_POISSON = 1;
        public const int PROBOPTION_BINOMIAL = 2;
        public const int PROBOPTION_APPROXIMATION = 3;
        public const int PROBOPTION_COMPARISON = 4;
        public const int PROBOPTION_FREQUENCY = 5;

        public static string OrgPosLabel(int orgPos)
        {
            switch (orgPos)
            {
                case ORGPOS_DEPT:
                    return "Direktorat";
                case ORGPOS_SUBDEPT:
                    return "Bagian";
                case ORGPOS_DIVISION:
                    return "Divisi";
                case ORGPOS_SUBDIV:
                    return "Bagian";
                case ORGPOS_BRANCH:
                    return "Cabang";
                case ORGPOS_SUBBRANCH:
                    return "Bagian";
                case ORGPOS_BIZUNIT:
                    return "KUP";
                default:
                    return string.Empty;
            }
        }

        private static bool GetUserDataFromDb(string userName, UserData data)
        {
            bool isUserValid = true;
            data.IsRiskOwner = false;
            data.IsAdmin = string.Compare(userName.ToLower(), Utils.S_ADMIN.ToLower()) == 0;
            if (data.IsAdmin)
            {
                data.UserName = userName;
                data.FullName = "Administrator";
            }
            else
            {
                MembershipUser usr = Membership.GetUser(userName);
                if (usr != null)
                {
                    Guid userId = (Guid)usr.ProviderUserKey;
                    AskrindoMVCEntities db = new AskrindoMVCEntities();
                    UserInfo nfo = db.UserInfos.Where(p => p.UserId == userId).FirstOrDefault();
                    isUserValid = nfo != null;
                    if (isUserValid)
                    {
                        data.UserId = userId;
                        data.UserName = nfo.aspnet_User.UserName;
                        data.FullName = nfo.FullName;
                        data.JobTitle = nfo.JobTitle;
                        data.OrgPos = nfo.OrgPos;
                        data.IsRiskOwner = nfo.IsRiskOwner;

                        switch (data.OrgPos)
                        {
                            case ORGPOS_DIVISION:
                                data.OrgPosId = (int)nfo.DivisionId;
                                Division div = db.Divisions.Single(p => p.DivisionId == nfo.DivisionId);
                                data.OrgPosName = string.Format("Direktorat: {0}, Divisi: {1}",
                                    div.Dept.DeptName, div.DivisionName);
                                data.DivisionId = div.DivisionId;
                                data.DeptId = div.DeptId;
                                break;
                            case ORGPOS_SUBDEPT:
                                data.OrgPosId = (int)nfo.SubDeptId;
                                SubDept subDept = db.SubDepts.Single(p => p.SubDeptId == nfo.SubDeptId);
                                data.OrgPosName = string.Format("Direktorat: {0}, Bagian: {1}",
                                    subDept.Dept.DeptName, subDept.SubDeptName);
                                data.SubDeptId = subDept.SubDeptId;
                                data.DeptId = subDept.DeptId;
                                break;
                            case ORGPOS_SUBDIV:
                                data.OrgPosId = (int)nfo.SubDivId;
                                SubDiv subDiv = db.SubDivs.Single(p => p.SubDivId == nfo.SubDivId);
                                data.OrgPosName = string.Format("Divisi: {0}, Bagian: {1}",
                                    subDiv.Division.DivisionName, subDiv.SubDivName);
                                data.SubDivId = subDiv.SubDivId;
                                data.DivisionId = subDiv.DivisionId;
                                data.DeptId = subDiv.Division.DeptId;
                                break;
                            case ORGPOS_BRANCH:
                                data.OrgPosId = (int)nfo.BranchId;
                                Branch branch = db.Branches.Single(p => p.BranchId == nfo.BranchId);
                                data.OrgPosName = string.Format("Cabang: {0}", branch.BranchName);
                                data.BranchId = branch.BranchId;
                                data.BranchClassId = branch.ClassId;
                                break;
                            case ORGPOS_SUBBRANCH:
                                data.OrgPosId = (int)nfo.SubBranchId;
                                SubBranch subBranch = db.SubBranches.Single(p => p.SubBranchId == nfo.SubBranchId);
                                data.OrgPosName = string.Format("Cabang: {0}, Bagian: {1}",
                                    subBranch.Branch.BranchName, subBranch.SubBranchName);
                                data.SubBranchId = subBranch.SubBranchId;
                                data.BranchId = subBranch.BranchId;
                                data.BranchClassId = subBranch.Branch.ClassId;
                                break;
                            case ORGPOS_BIZUNIT:
                                data.OrgPosId = (int)nfo.BizUnitId;
                                BizUnit biz = db.BizUnits.Single(p => p.BizUnitId == nfo.BizUnitId);
                                data.OrgPosName = string.Format("Cabang: {0}, KUP: {1}",
                                    biz.Branch.BranchName, biz.BizUnitName);
                                data.BizUnitId = biz.BizUnitId;
                                data.BranchId = biz.BranchId;
                                data.BranchClassId = biz.Branch.ClassId;
                                break;
                        }
                    }
                }
                else
                    isUserValid = false;
            }
            if (isUserValid)
                HttpContext.Current.Session["Data"] = data;
            else
            {
                FormsAuthentication.SignOut();
                HttpContext.Current.Session["Data"] = null;
            }
            return isUserValid;
        }

        public static UserData LoadUserDataFromSession()
        {
            UserData data = HttpContext.Current.Session["Data"] as UserData;
            if (data == null)
            {
                data = new UserData();
                GetUserDataFromDb(HttpContext.Current.User.Identity.Name, data);
            }
            return data;
        }

        private static void ValidateSerialNumber()
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            SerialNumber sn = null;
            if (db.SerialNumbers.Count() == 0)
            {
                sn = new SerialNumber();
                sn.Year = DateTime.Now.Year;
                sn.SN = 1;
                db.SerialNumbers.AddObject(sn);
                db.SaveChanges();
            }
            else
            {
                sn = db.SerialNumbers.First();
                if (sn.Year != DateTime.Now.Year)
                {
                    sn.Year = DateTime.Now.Year;
                    sn.SN = 1;
                    db.SaveChanges();
                }
            }
        }

        public static string GetFormattedSerialNumber(UserData data)
        {
            ValidateSerialNumber();
            AskrindoMVCEntities db = new AskrindoMVCEntities();

            string branchCode = "99";
            string bizCode = "00";

            if (data.BranchClassId != null)
            {
                Branch branch = db.Branches.Single(p => p.BranchId == data.BranchId);
                branchCode = branch.BranchCode.Trim().PadLeft(2, '0');
            }
            if (data.BizUnitId != null)
            {
                BizUnit biz = db.BizUnits.Single(p => p.BizUnitId == data.BizUnitId);
                bizCode = biz.BizUnitCode.Trim().PadLeft(2, '0');
            }

            string year = DateTime.Now.Year.ToString().PadLeft(4, '0');
            string month = DateTime.Now.Month.ToString().PadLeft(2, '0');

            SerialNumber sn = db.SerialNumbers.First();

            return branchCode + "." + bizCode + "." + year + "." + month + "." + sn.SN.ToString().PadLeft(4, '0');
        }

        public static void IncrementSerialNumber(AskrindoMVCEntities db)
        {
            var sn = db.SerialNumbers.First();
            sn.SN++;
            db.SaveChanges();
        }

        public static int GetProbLevelFromValue(int pct)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            int maxValue = int.MinValue;
            foreach (var level in db.ProbLevels)
            {
                if (pct >= level.PctMin && pct < level.PctMax)
                    return level.ProbLevelId;

                if (maxValue < level.PctMax)
                    maxValue = level.PctMax;
            }
            if (pct >= maxValue)
                return PROBLEVEL5;
            else
                return PROBLEVEL1;
        }

        public static int GetProbLevelFromValue(decimal pct)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            int maxValue = int.MinValue;
            foreach (var level in db.ProbLevels)
            {
                if (pct >= level.PctMin && pct < level.PctMax)
                    return level.ProbLevelId;

                if (maxValue < level.PctMax)
                    maxValue = level.PctMax;
            }
            if (pct >= maxValue)
                return PROBLEVEL5;
            else
                return PROBLEVEL1;
        }

        public static int GetImpactLevelFromMoney(decimal value, int impactPos = IMPACTPOS_HQ)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            var impactRef = db.ImpactRefs.First();
            decimal pct = 0M;
            switch (impactPos)
            {
                case IMPACTPOS_HQ:
                    pct = impactRef.HQPct;
                    break;
                case IMPACTPOS_BRANCH1:
                    pct = impactRef.Branch1Pct;
                    break;
                case IMPACTPOS_BRANCH2:
                    pct = impactRef.Branch2Pct;
                    break;
                case IMPACTPOS_BRANCH3:
                    pct = impactRef.Branch3Pct;
                    break;
                case IMPACTPOS_BIZUNIT:
                    pct = impactRef.BizUnitPct;
                    break;
                case IMPACTPOS_SUPPORTINGHQ:
                    pct = impactRef.SupportingHQPct;
                    break;
                case IMPACTPOS_SUPPORTINGBRANCH:
                    pct = impactRef.SupportingBranchPct;
                    break;
                case IMPACTPOS_SUPPORTINGBIZUNIT:
                    pct = impactRef.SupportingBizUnitPct;
                    break;
            }

            decimal maxValue = decimal.MinValue;
            decimal value1;
            decimal value2;
            foreach (var level in db.ImpactLevels)
            {
                value1 = level.MoneyMin * pct / 100M;
                value2 = level.MoneyMax * pct / 100M;
                if (value >= value1 && value < value2)
                    return level.ImpactLevelId;

                if (maxValue < value2)
                    maxValue = value2;
            }
            if (value >= maxValue)
                return IMPACTLEVEL5;
            else
                return IMPACTLEVEL1;
        }

        //public static void CreateApprovalSchedule(int riskId)
        //{
        //    AskrindoMVCEntities db = new AskrindoMVCEntities();
        //    var risk = db.Risks.Single(p => p.RiskId == riskId);
        //    if (risk == null)
        //        return;
        //    int nextPos;
        //    switch (risk.OrgPos)
        //    {
        //        case ORGPOS_DIVISION
        //    }
        //}

        public static void CreateFirstApprovalSchedule(int riskId)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            CreateFirstApprovalSchedule(riskId, db);
            db.SaveChanges();
        }

        public static void CreateFirstApprovalSchedule(int riskId, AskrindoMVCEntities db)
        {
            var risk = db.Risks.Single(p => p.RiskId == riskId);
            RiskApproval app;
            app = db.RiskApprovals.Where(p => p.RiskId == riskId && p.OrgPos == risk.OrgPos).FirstOrDefault();
            if (app == null)
            {
                app = new RiskApproval();
                app.RiskId = riskId;
                app.OrgPos = risk.OrgPos;
                switch (app.OrgPos)
                {
                    case ORGPOS_DEPT:
                        app.DeptId = risk.DeptId;
                        app.OrgName = risk.Dept.DeptName;
                        break;
                    case ORGPOS_SUBDEPT:
                        app.SubDeptId = risk.SubDeptId;
                        app.OrgName = risk.SubDept.SubDeptName;
                        break;
                    case ORGPOS_DIVISION:
                        app.DivisionId = risk.DivisionId;
                        app.OrgName = risk.Division.DivisionName;
                        break;
                    case ORGPOS_SUBDIV:
                        app.SubDivId = risk.SubDivId;
                        app.OrgName = risk.SubDiv.SubDivName;
                        break;
                    case ORGPOS_BRANCH:
                        app.BranchId = risk.BranchId;
                        app.OrgName = risk.Branch.BranchName;
                        break;
                    case ORGPOS_SUBBRANCH:
                        app.SubBranchId = risk.SubBranchId;
                        app.OrgName = risk.SubBranch.SubBranchName;
                        break;
                    case ORGPOS_BIZUNIT:
                        app.BizUnitId = risk.BizUnitId;
                        app.OrgName = risk.BizUnit.BizUnitName;
                        break;
                }

                app.LimitDate = risk.RiskDate.AddDays(MAX_LIMIT_APPROVAL_DAYS);
                app.LastApproval = app.OrgPos == ORGPOS_SUBDEPT || app.OrgPos == ORGPOS_DIVISION || app.OrgPos == ORGPOS_BRANCH;
                app.IsReadOnly = false;

                db.RiskApprovals.AddObject(app);
            }
        }

        public static string GetRiskOrgName(Risk r)
        {
            switch (r.OrgPos)
            {
                case ORGPOS_DEPT:
                    return r.Dept.DeptName;
                case ORGPOS_SUBDEPT:
                    return r.SubDept.SubDeptName;
                case ORGPOS_DIVISION:
                    return r.Division.DivisionName;
                case ORGPOS_SUBDIV:
                    return r.SubDiv.SubDivName;
                case ORGPOS_BRANCH:
                    return r.Branch.BranchName;
                case ORGPOS_SUBBRANCH:
                    return r.SubBranch.SubBranchName;
                case ORGPOS_BIZUNIT:
                    return r.BizUnit.BizUnitName;
                default:
                    return string.Empty;
            }
        }

        public static void CreateFirstMitigationApprovalSchedule(int mitigationId)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            CreateFirstMitigationApprovalSchedule(mitigationId, db);
            db.SaveChanges();
        }

        public static void CreateFirstMitigationApprovalSchedule(int mitigationId, AskrindoMVCEntities db)
        {
            var mg = db.RiskMitigations.Single(p => p.MitigationId == mitigationId);
            MitigationApproval apr = new MitigationApproval();
            apr.MitigationId = mitigationId;
            apr.OrgPos = mg.OrgPos;
            switch (apr.OrgPos)
            {
                case ORGPOS_DEPT:
                    apr.DeptId = mg.DeptId;
                    break;
                case ORGPOS_SUBDEPT:
                    apr.SubDeptId = mg.SubDeptId;
                    break;
                case ORGPOS_DIVISION:
                    apr.DivisionId = mg.DivisionId;
                    break;
                case ORGPOS_SUBDIV:
                    apr.SubDivId = mg.SubDivId;
                    break;
                case ORGPOS_BRANCH:
                    apr.BranchId = mg.BranchId;
                    break;
                case ORGPOS_SUBBRANCH:
                    apr.SubBranchId = mg.SubBranchId;
                    break;
                case ORGPOS_BIZUNIT:
                    apr.BizUnitId = mg.BizUnitId;
                    break;
            }
            apr.LastApproval = apr.OrgPos == ORGPOS_SUBDEPT || apr.OrgPos == ORGPOS_DIVISION || apr.OrgPos == ORGPOS_BRANCH;
            apr.IsReadOnly = false;
            db.MitigationApprovals.AddObject(apr);
        }

        public static void GetOrgApprovalUser(int orgPos, int orgPosId, UserInfo nfo)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            GetOrgApprovalUser(orgPos, orgPosId, nfo, db);
            db.SaveChanges();
        }

        public static void GetOrgApprovalUser(int orgPos, int orgPosId, UserInfo nfo, AskrindoMVCEntities db)
        {
            nfo = null;
            switch (orgPos)
            {
                case ORGPOS_DEPT:
                    nfo = db.UserInfos.First(p => p.DeptId == orgPosId);
                    break;
                case ORGPOS_SUBDEPT:
                    nfo = db.UserInfos.First(p => p.SubDeptId == orgPosId);
                    break;
                case ORGPOS_DIVISION:
                    nfo = db.UserInfos.First(p => p.DivisionId == orgPosId);
                    break;
                case ORGPOS_SUBDIV:
                    nfo = db.UserInfos.First(p => p.SubDivId == orgPosId);
                    break;
                case ORGPOS_BRANCH:
                    nfo = db.UserInfos.First(p => p.BranchId == orgPosId);
                    break;
                case ORGPOS_SUBBRANCH:
                    nfo = db.UserInfos.First(p => p.SubBranchId == orgPosId);
                    break;
                case ORGPOS_BIZUNIT:
                    nfo = db.UserInfos.First(p => p.BizUnitId == orgPosId);
                    break;
            }
        }

        public static string GetProbOptionName(int probOption)
        {
            switch (probOption)
            {
                case PROBOPTION_POISSON:
                    return "Statistik Poisson (data tersedia)";
                case PROBOPTION_BINOMIAL:
                    return "Statistik Binomial (data tersedia)";
                case PROBOPTION_APPROXIMATION:
                    return "Aproksimasi (data tidak tersedia)";
                case PROBOPTION_COMPARISON:
                    return "Perbandingan (data tidak tersedia)";
                case PROBOPTION_FREQUENCY:
                    return "Frekuensi (data tidak tersedia)";
                default:
                    return string.Empty;
            }
        }

        public static int GetImpactPos(UserData data)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();

            switch (data.OrgPos)
            {
                case ORGPOS_SUBDEPT:
                    var subDept = db.SubDepts.Single(p => p.SubDeptId == data.SubDeptId);
                    if (subDept.IsSupporting) 
                        return IMPACTPOS_SUPPORTINGHQ;
                    else
                        return IMPACTPOS_HQ;
                case ORGPOS_DIVISION:
                    var div = db.Divisions.Single(p => p.DivisionId == data.DivisionId);
                    if (div.IsSupporting) 
                        return IMPACTPOS_SUPPORTINGHQ;
                    else 
                        return IMPACTPOS_HQ;
                case ORGPOS_SUBDIV:
                    var subDiv = db.SubDivs.Single(p => p.SubDivId == data.SubDivId);
                    if (subDiv.IsSupporting)
                        return IMPACTPOS_SUPPORTINGHQ;
                    else
                        return IMPACTPOS_HQ;
                case ORGPOS_BRANCH:
                    var branch = db.Branches.Single(p => p.BranchId == data.BranchId);
                    if (branch.IsSupporting)
                        return IMPACTPOS_SUPPORTINGBRANCH;
                    else
                    {
                        if (branch.ClassId == BRANCHCLASS1)
                            return IMPACTPOS_BRANCH1;
                        else if (branch.ClassId == BRANCHCLASS2)
                            return IMPACTPOS_BRANCH2;
                        else
                            return IMPACTPOS_BRANCH3;
                    }
                case ORGPOS_SUBBRANCH:
                    var subBranch = db.SubBranches.Single(p => p.SubBranchId == data.SubBranchId);
                    if (subBranch.IsSupporting)
                        return IMPACTPOS_SUPPORTINGBRANCH;
                    else
                    {
                        if (subBranch.Branch.ClassId == BRANCHCLASS1)
                            return IMPACTPOS_BRANCH1;
                        else if (subBranch.Branch.ClassId == BRANCHCLASS2)
                            return IMPACTPOS_BRANCH2;
                        else
                            return IMPACTPOS_BRANCH3;
                    }
                case ORGPOS_BIZUNIT:
                    var biz = db.BizUnits.Single(p => p.BizUnitId == data.BizUnitId);
                    if (biz.IsSupporting)
                        return IMPACTPOS_SUPPORTINGBIZUNIT;
                    else
                        return IMPACTPOS_BIZUNIT;
                default:
                    return IMPACTPOS_HQ;
            }
        }

        public static void CalcRiskLevel(Risk risk)
        {
            if (risk.ProbLevelId == null || risk.ImpactLevelId == null)
                risk.RiskLevel = null;
            else
                risk.RiskLevel = (int)risk.ProbLevelId * (int)risk.ImpactLevelId;
        }

        public static List<KeyValuePair<string, string>> GetOrgPosNames(UserInfo nfo, AskrindoMVCEntities db)
        {
            //List<KeyValuePair<string, string>> orgList;// = new List<KeyValuePair<string, string>>();
            switch (nfo.OrgPos)
            {
                case Utils.ORGPOS_DEPT:
                    return GetDeptNames(nfo.DeptId, db);
                case Utils.ORGPOS_DIVISION:
                    return GetDivisionNames(nfo.DivisionId, db);
                case Utils.ORGPOS_SUBDIV:
                    return GetSubDivNames(nfo.SubDivId, db);
                case Utils.ORGPOS_SUBDEPT:
                    return GetSubDeptNames(nfo.SubDeptId, db);
                case Utils.ORGPOS_BRANCH:
                    return GetBranchNames(nfo.BranchId, db);
                case Utils.ORGPOS_SUBBRANCH:
                    return GetSubBranchNames(nfo.SubBranchId, db);
                case Utils.ORGPOS_BIZUNIT:
                    return GetBizUnitNames(nfo.BizUnitId, db);
                default:
                    return null;
            }
        }

        private static List<KeyValuePair<string, string>> GetBizUnitNames(int? bizUnitId, AskrindoMVCEntities db)
        {
            var orgList = new List<KeyValuePair<string, string>>();
            BizUnit biz = db.BizUnits.Single(p => p.BizUnitId == bizUnitId);
            orgList.Insert(0, new KeyValuePair<string, string>("KUP", biz.BizUnitName));
            orgList.Insert(0, new KeyValuePair<string, string>("Cabang", biz.Branch.BranchName));
            return orgList;
        }

        private static List<KeyValuePair<string, string>> GetSubBranchNames(int? subBranchId, AskrindoMVCEntities db)
        {
            var orgList = new List<KeyValuePair<string, string>>();
            SubBranch sub = db.SubBranches.Single(p => p.SubBranchId == subBranchId);
            orgList.Insert(0, new KeyValuePair<string, string>("Bagian", sub.SubBranchName));
            orgList.Insert(0, new KeyValuePair<string, string>("Cabang", sub.Branch.BranchName));
            return orgList;
        }

        private static List<KeyValuePair<string, string>> GetBranchNames(int? branchId, AskrindoMVCEntities db)
        {
            var orgList = new List<KeyValuePair<string, string>>();
            Branch branch = db.Branches.Single(p => p.BranchId == branchId);
            orgList.Insert(0, new KeyValuePair<string, string>("Cabang", branch.BranchName));
            return orgList;
        }

        private static List<KeyValuePair<string, string>> GetSubDeptNames(int? subDeptId, AskrindoMVCEntities db)
        {
            var orgList = new List<KeyValuePair<string, string>>();
            SubDept sub = db.SubDepts.Single(p => p.SubDeptId == subDeptId);
            orgList.Insert(0, new KeyValuePair<string, string>("Bagian", sub.SubDeptName));
            orgList.Insert(0, new KeyValuePair<string, string>("Direktorat", sub.Dept.DeptName));
            return orgList;
        }

        private static List<KeyValuePair<string, string>> GetSubDivNames(int? subDivId, AskrindoMVCEntities db)
        {
            var orgList = new List<KeyValuePair<string, string>>();
            SubDiv sub = db.SubDivs.Single(p => p.SubDivId == subDivId);
            orgList.Insert(0, new KeyValuePair<string, string>("Bagian", sub.SubDivName));
            orgList.Insert(0, new KeyValuePair<string, string>("Divisi", sub.Division.DivisionName));
            orgList.Insert(0, new KeyValuePair<string, string>("Direktorat", sub.Division.Dept.DeptName));
            return orgList;
        }

        private static List<KeyValuePair<string, string>> GetDivisionNames(int? divisionId, AskrindoMVCEntities db)
        {
            var orgList = new List<KeyValuePair<string, string>>();
            Division div = db.Divisions.Single(p => p.DivisionId == divisionId);
            orgList.Insert(0, new KeyValuePair<string, string>("Divisi", div.DivisionName));
            orgList.Insert(0, new KeyValuePair<string, string>("Direktorat", div.Dept.DeptName));
            return orgList;
        }

        private static List<KeyValuePair<string, string>> GetDeptNames(int? deptId, AskrindoMVCEntities db)
        {
            var orgList = new List<KeyValuePair<string, string>>();
            Dept dept = db.Depts.Single(p => p.DeptId == deptId);
            orgList.Insert(0, new KeyValuePair<string, string>("Direktorat", dept.DeptName));
            return orgList;
        }

        public static bool IsRiskDataCompleted(Risk risk)
        {
            return risk.CauseId != null && risk.EffectId != null && risk.RiskTypeId != null && risk.ProbLevelId != null && risk.ImpactLevelId != null;
        }

        public static string CreateNewMiticationCode(int riskId)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            var risk = db.Risks.FirstOrDefault(p => p.RiskId == riskId);
            var riskCode = risk.RiskCode;
            var mitigations = db.RiskMitigations.Where(p => p.RiskId == riskId);
            int i = 1;
            string s;
            while (true)
            {
                s = risk.RiskCode + "." + i.ToString().PadLeft(2, '0');
                var m = mitigations.FirstOrDefault(p => p.MitigationCode == s);
                if (m == null)
                    return s;
                i++;
            }
        }

        public static string GetDatePattern()
        {
            CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            return newCulture.DateTimeFormat.ShortDatePattern;
        }

        //public static void ApproveMitigation(int mitigationId, AskrindoMVCEntities db)
        //{
        //    UserData data = Utils.LoadUserDataFromSession();
        //    RiskMitigation mitigation = db.RiskMitigations.Where(p => p.MitigationId == mitigationId).FirstOrDefault();
        //    using (TransactionScope trans = new TransactionScope())
        //    {
        //        mitigation.ApprovalDate = DateTime.Now;
        //        mitigation.UserId = data.UserId;
        //        mitigation.JobTitle = data.JobTitle;

        //        RiskState rs = new RiskState();
        //        rs.RiskId = mitigation.RiskId;
        //        rs.MitigationId = mitigation.MitigationId;
        //        rs.StateDate = (DateTime)mitigation.ApprovalDate;
        //        rs.ProbLevelId = (int)mitigation.ProbLevelId;
        //        rs.ImpactLevelId = (int)mitigation.ImpactLevelId;
        //        rs.RiskLevel = (int)mitigation.RiskLevel;
        //        db.RiskStates.AddObject(rs);

        //        db.SaveChanges();
        //        trans.Complete();
        //    }
        //}

        //public static void CancelMitigationApproval(int mitigationId, AskrindoMVCEntities db)
        //{
        //    UserData data = Utils.LoadUserDataFromSession();
        //    RiskMitigation mitigation = db.RiskMitigations.Where(p => p.MitigationId == mitigationId).FirstOrDefault();
        //    using (TransactionScope trans = new TransactionScope())
        //    {
        //        mitigation.ApprovalDate = null;
        //        mitigation.UserId = null;
        //        mitigation.JobTitle = null;

        //        RiskState rs = db.RiskStates.Where(p => p.RiskId == mitigation.RiskId && p.MitigationId == mitigation.MitigationId).FirstOrDefault();
        //        if (rs != null)
        //            db.RiskStates.DeleteObject(rs);

        //        db.SaveChanges();
        //        trans.Complete();
        //    }
        //}

        public static void ApproveMitigation(int approvalId, AskrindoMVCEntities db)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                UserData data = LoadUserDataFromSession();
                MitigationApproval apr = db.MitigationApprovals.Where(p => p.ApprovalId == approvalId).FirstOrDefault();
                apr.ApprovalDate = DateTime.Now;
                apr.UserId = data.UserId;
                apr.JobTitle = data.JobTitle;
                db.SaveChanges();

                apr.RiskMitigation.IsReadOnly = true;
                db.SaveChanges();

                if (apr.LastApproval)
                {
                    // mitigation approval complete
                    apr.RiskMitigation.ApprovalDate = apr.ApprovalDate;
                    db.SaveChanges();

                    MitigationApproval prevApr = db.MitigationApprovals
                        .Where(p => p.MitigationId == apr.MitigationId && p.ApprovalId != apr.ApprovalId && p.ApprovalDate != null)
                        .FirstOrDefault();
                    if (prevApr != null)
                        prevApr.IsReadOnly = true;
                    db.SaveChanges();

                    RiskState rs = new RiskState();
                    rs.RiskId = apr.RiskMitigation.RiskId;
                    rs.MitigationId = apr.MitigationId;
                    rs.StateDate = (DateTime)apr.ApprovalDate;
                    rs.ProbLevelId = (int)apr.RiskMitigation.ProbLevelId;
                    rs.ImpactLevelId = (int)apr.RiskMitigation.ImpactLevelId;
                    rs.RiskLevel = (int)apr.RiskMitigation.RiskLevel;
                    db.RiskStates.AddObject(rs);
                    db.SaveChanges();
                }
                else
                {
                    // create next approval schedule
                    MitigationApproval nextApr = new MitigationApproval();
                    nextApr.MitigationId = apr.MitigationId;
                    nextApr.LimitDate = DateTime.Now.AddDays(Utils.MAX_LIMIT_APPROVAL_DAYS);
                    nextApr.LastApproval = true;

                    switch (apr.OrgPos)
                    {
                        case Utils.ORGPOS_SUBDIV:
                            SubDiv subDiv = db.SubDivs.Single(p => p.SubDivId == apr.SubDivId);
                            nextApr.OrgPos = Utils.ORGPOS_DIVISION;
                            nextApr.DivisionId = subDiv.DivisionId;
                            break;
                        case Utils.ORGPOS_SUBBRANCH:
                            SubBranch subBranch = db.SubBranches.Single(p => p.SubBranchId == apr.SubBranchId);
                            nextApr.OrgPos = Utils.ORGPOS_BRANCH;
                            nextApr.BranchId = subBranch.BranchId;
                            break;
                        case Utils.ORGPOS_BIZUNIT:
                            BizUnit bizUnit = db.BizUnits.Single(p => p.BizUnitId == apr.BizUnitId);
                            nextApr.OrgPos = Utils.ORGPOS_BRANCH;
                            nextApr.BranchId = bizUnit.BranchId;
                            break;
                    }
                    db.MitigationApprovals.AddObject(nextApr);
                    db.SaveChanges();
                }
                trans.Complete();
            }
        }

        public static void CancelMitigationApproval(int approvalId, AskrindoMVCEntities db)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                UserData data = Utils.LoadUserDataFromSession();
                MitigationApproval apr = db.MitigationApprovals.Where(p => p.ApprovalId == approvalId).FirstOrDefault();
                MitigationApproval nextApr = db.MitigationApprovals
                    .Where(p => p.MitigationId == apr.MitigationId && p.ApprovalDate == null && p.ApprovalId != apr.ApprovalId)
                    .FirstOrDefault();
                if (nextApr != null)
                    db.MitigationApprovals.DeleteObject(nextApr);
                apr.UserId = null;
                apr.JobTitle = null;
                apr.ApprovalDate = null;
                db.SaveChanges();

                if (apr.LastApproval)
                {
                    apr.RiskMitigation.ApprovalDate = null;
                    RiskState rs = db.RiskStates.Where(p => p.RiskId == apr.RiskMitigation.RiskId && p.MitigationId == apr.MitigationId).FirstOrDefault();
                    if (rs != null)
                    {
                        db.RiskStates.DeleteObject(rs);
                        db.SaveChanges();
                    }
                }

                MitigationApproval prevApr = db.MitigationApprovals
                    .Where(p => p.MitigationId == apr.MitigationId && p.ApprovalDate != null && p.ApprovalId != apr.ApprovalId)
                    .FirstOrDefault();
                if (prevApr == null)
                    apr.RiskMitigation.IsReadOnly = false;
                else
                    prevApr.IsReadOnly = false;
                db.SaveChanges();
                trans.Complete();
            }
        }

        public static string GetProbLevelText(int probLevelId)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            var data = db.ProbLevels.Single(p => p.ProbLevelId == probLevelId);
            return probLevelId.ToString() + " - " + data.ProbLevelName;
        }

        public static string GetImpactLevelText(int impactLevelId)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            var data = db.ImpactLevels.Single(p => p.ImpactLevelId == impactLevelId);
            return impactLevelId.ToString() + " - " + data.ImpactLevelName;
        }

        public static void GetRiskLevelColors(int riskLevelId, out string backColor, out string foreColor)
        {
            backColor = "White";
            foreColor = "Black";

            AskrindoMVCEntities db = new AskrindoMVCEntities();
            var levels = db.RiskLevels;
            foreach (var item in levels)
            {
                if (riskLevelId >= item.MinValue && riskLevelId <= item.MaxValue)
                {
                    backColor = item.BackColor;
                    foreColor = item.ForeColor;
                    return;
                }
            }
        }

        public static string GetBranchName(int branchId)
        {
            AskrindoMVCEntities db = new AskrindoMVCEntities();
            var branch = db.Branches.Where(p => p.BranchId == branchId).FirstOrDefault();
            if (branch == null)
                return string.Empty;
            else
                return branch.BranchName + " (Kelas " + branch.BranchClass.ClassName + ")";
        }

        public static string GetMaxRequestLengthAsText()
        {
            HttpRuntimeSection aSection = (HttpRuntimeSection)WebConfigurationManager.GetSection("system.web/httpRuntime");
            int maxRequestLength = aSection.MaxRequestLength;
            if (maxRequestLength < 1024)
                return string.Format("{0:#,##0} KB", maxRequestLength);
            else
                return string.Format("{0:#,##0.##} MB", maxRequestLength / 1024M);
        }

        public static string GetByteSizeAsText(int size)
        {
            if (size < 1024)
                return string.Format("{0:#,##0} Bytes", size);
            decimal kb = size / 1024M;
            if (kb < 1024)
                return string.Format("{0:#,##0.##} KB", kb);
            return string.Format("{0:#,##0.##} MB", kb / 1024M);
        }
    }

    public static class Extensions
    {
        public static string MaxLength(this string s, int length)
        {
            if (s.Length > length)
            {
                if (s.Length > 2)
                    return s.Substring(0, length - 3) + "...";
                else
                    return s.Substring(0, length);
            }
            else
                return s;
        }
    }
}