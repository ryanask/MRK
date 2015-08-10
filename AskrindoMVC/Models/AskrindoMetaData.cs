using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AskrindoMVC.Models
{
    #region Dept
    
    [MetadataType(typeof(DeptMetaData))]
    public partial class Dept
    {
    }

    public class DeptMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object DeptId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object DeptName { get; set; }
    }

    #endregion

    #region SubDept
    
    [MetadataType(typeof(SubDeptMetaData))]
    public partial class SubDept
    {
    }

    public class SubDeptMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object SubDeptId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object SubDeptName { get; set; }
    }

    #endregion

    #region Division

    [MetadataType(typeof(DivisionMetaData))]
    public partial class Division
    {
    }

    public class DivisionMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object DivisionId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object DivisionName { get; set; }

        [Display(Name = "Supporting")]
        public object IsSupporting { get; set; }
    } 

    #endregion

    #region Branch

    [MetadataType(typeof(BranchMetaData))]
    public partial class Branch
    {
    }

    public class BranchMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object BranchId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object BranchName { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        public object ClassId { get; set; }

        [StringLength(2, ErrorMessage = "Maksimum 2 karakter")]
        public object BranchCode { get; set; }
    } 

    #endregion

    #region SubBranch

    [MetadataType(typeof(SubBranchMetaData))]
    public partial class SubBranch
    {
    }

    public class SubBranchMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object SubBranchId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object SubBranchName { get; set; }
    } 

    #endregion

    #region SubDiv

    [MetadataType(typeof(SubDivMetaData))]
    public partial class SubDiv
    {
    }

    public class SubDivMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object SubDivId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object SubDivName { get; set; }
    } 

    #endregion

    #region BizUnit

    [MetadataType(typeof(BizUnitMetaData))]
    public partial class BizUnit
    {
    }

    public class BizUnitMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object BizUnitId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object BizUnitName { get; set; }

        [StringLength(2, ErrorMessage = "Maksimum 2 karakter")]
        public object BizUnitCode { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        public object IsSupporting { get; set; }
    } 

    #endregion

    #region CauseGroup

    [MetadataType(typeof(CauseGroupMetaData))]
    public partial class CauseGroup
    {
    }

    public class CauseGroupMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object CauseGroupId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object CauseGroupName { get; set; }
    }

    #endregion

    #region CauseType

    [MetadataType(typeof(CauseTypeMetaData))]
    public partial class CauseType
    {
    }

    public class CauseTypeMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object CauseTypeId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object CauseTypeName { get; set; }
    }

    #endregion

    #region Cause

    [MetadataType(typeof(CauseMetaData))]
    public partial class Cause
    {
    }

    public class CauseMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object CauseId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object CauseName { get; set; }
    }

    #endregion

    #region EffectGroup
    
    [MetadataType(typeof(EffectGroupMetaData))]
    public partial class EffectGroup
    {
    }

    public class EffectGroupMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object EffectGroupId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object EffectGroupName { get; set; }
    }

    #endregion

    #region EffectType
    
    [MetadataType(typeof(EffectTypeMetaData))]
    public partial class EffectType
    {
    }

    public class EffectTypeMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object EffectTypeId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object EffectTypeName { get; set; }
    }

    #endregion

    #region Effect
    
    [MetadataType(typeof(EffectMetaData))]
    public partial class Effect
    {
    }

    public class EffectMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object EffectId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object EffectName { get; set; }
    }

    #endregion

    #region RiskCat
    
    [MetadataType(typeof(RiskCatMetaData))]
    public partial class RiskCat
    {
    }

    public class RiskCatMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object RiskCatId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object RiskCatName { get; set; }
    }

    #endregion

    #region RiskGroup
    
    [MetadataType(typeof(RiskGroupMetaData))]
    public partial class RiskGroup
    {
    }

    public class RiskGroupMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object RiskGroupId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object RiskGroupName { get; set; }
    }

    #endregion

    #region RiskType
    
    [MetadataType(typeof(RiskTypeMetaData))]
    public partial class RiskType
    {
    }

    public class RiskTypeMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object RiskTypeId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maksimum 200 karakter")]
        public object RiskTypeName { get; set; }
    }

    #endregion

    #region ProbLevel
    
    [MetadataType(typeof(ProbLevelMetaData))]
    public partial class ProbLevel
    {
    }

    public class ProbLevelMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object ProbLevelName { get; set; }

        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object PctMin { get; set; }

        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object PctMax { get; set; }

        [DataType(DataType.MultilineText)]
        public object DescriptionGeneral { get; set; }
    }

    #endregion

    #region ImpactLevel
    
    [MetadataType(typeof(ImpactLevelMetaData))]
    public partial class ImpactLevel
    {
    }

    public class ImpactLevelMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object ImpactLevelName { get; set; }

        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object PctMin { get; set; }

        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object PctMax { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public object MoneyMin { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public object MoneyMax { get; set; }
    }

    #endregion

    #region ImpactRef
    
    [MetadataType(typeof(ImpactRefMetaData))]
    public partial class ImpactRef
    {
    }

    public class ImpactRefMetaData
    {
        [DisplayFormat(DataFormatString="{0:#,##0.##}")]
        public object MaxMoney { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object HQPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object Branch1Pct { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object Branch2Pct { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object Branch3Pct { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object BizUnitPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object SupportingHQPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object SupportingBranchPct { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0, 100, ErrorMessage = "Min 0, maks 100")]
        public object SupportingBizUnitPct { get; set; }
    }

    #endregion

    #region ImpactCat
    
    [MetadataType(typeof(ImpactCatMetaData))]
    public partial class ImpactCat
    {
    }

    public class ImpactCatMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object ImpactCatId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(50, ErrorMessage = "Maks 50 karakter")]
        public object ImpactCatName { get; set; }
    }

    #endregion

    #region ImpactType
    
    [MetadataType(typeof(ImpactTypeMetaData))]
    public partial class ImpactType
    {
    }

    public class ImpactTypeMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object ImpactTypeId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public object ImpactTypeName { get; set; }

        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public object Notes { get; set; }
    }

    #endregion

    #region MitigationCat
    
    [MetadataType(typeof(MitigationCatMetaData))]
    public partial class MitigationCat
    {
    }

    public class MitigationCatMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object MitigationCatId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public object MitigationCatName { get; set; }
    }

    #endregion

    #region MitigationType

    [MetadataType(typeof(MitigationTypeMetaData))]
    public partial class MitigationType
    {
    }

    public class MitigationTypeMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object MitigationTypeId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public object MitigationTypeName { get; set; }
    }

    #endregion

    #region Risk
    
    [MetadataType(typeof(RiskMetaData))]
    public partial class Risk
    {
    }

    public class RiskMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(20, ErrorMessage = "Maks 20 karakter")]
        public object RiskCode { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(2000, ErrorMessage = "Maks 2000 karakter")]
        [DataType(DataType.MultilineText)]
        public object RiskName { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object RiskDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object ApprovalDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object CloseDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public object ImpactMoney { get; set; }
    }

    #endregion

    #region RiskProb
    
    [MetadataType(typeof(RiskProbMetaData))]
    public partial class RiskProb
    {
    }

    public class RiskProbMetaData
    {
        [Range(0, 100)]
        public object Approx1 { get; set; }

        [Range(0, 100)]
        public object Approx2 { get; set; }

        [Range(0, 100)]
        public object Approx3 { get; set; }

        [Range(0, 100)]
        public object Compare { get; set; }
    }

    #endregion

    #region RiskImpact
    
    [MetadataType(typeof(RiskImpactMetaData))]
    public partial class RiskImpact
    {
    }

    public class RiskImpactMetaData
    {
        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Tidak boleh kecil dari nol")]
        public object MoneyDirect { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        [Range(0.0, Double.MaxValue, ErrorMessage = "Tidak boleh kecil dari nol")]
        public object MoneyIndirect { get; set; }
    }

    #endregion

    #region RiskAttachment

    [MetadataType(typeof(RiskAttachmentMetaData))]
    public partial class RiskAttachment
    {
    }

    public class RiskAttachmentMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object AttachName { get; set; }

        [DataType(DataType.MultilineText)]
        public object Notes { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public object ContentLength { get; set; }
    }

    #endregion

    #region RiskApproval

    [MetadataType(typeof(RiskApprovalMetaData))]
    public partial class RiskApproval
    {
    }

    public class RiskApprovalMetaData
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object LimitDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object ApprovalDate { get; set; }
    }
 
    #endregion

    #region RiskMitigation

    [MetadataType(typeof(RiskMitigationMetaData))]
    public partial class RiskMitigation
    {
    }

    public class RiskMitigationMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        [DataType(DataType.MultilineText)]
        public object MitigationName { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object MitigationDate { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object InputDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object ApprovalDate { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        public object MitigationCatId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        public object MitigationTypeId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        public object ProbLevelId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        public object ImpactLevelId { get; set; }
    } 

    #endregion

    #region RiskState

    [MetadataType(typeof(RiskStateMetaData))]
    public partial class RiskState
    {
    }

    public class RiskStateMetaData
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public object StateDate { get; set; }
    } 

    #endregion

    #region RiskLevel

    [MetadataType(typeof(RiskLevelMetaData))]
    public partial class RiskLevel
    {
    }

    public class RiskLevelMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object LevelId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(50, ErrorMessage = "Maks 50 karakter")]
        public object LevelName { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Range(1, 25, ErrorMessage = "Min 1, maks 25")]
        public object MinValue { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [Range(1, 25, ErrorMessage = "Min 1, maks 25")]
        public object MaxValue { get; set; }

        [StringLength(100, ErrorMessage = "Maks 100 karakter")]
        public object Action { get; set; }

        [StringLength(10, ErrorMessage = "Maks 10 karakter")]
        public object BackColor { get; set; }

        [StringLength(10, ErrorMessage = "Maks 10 karakter")]
        public object ForeColor { get; set; }
    } 

    #endregion

    #region HelpMenu

    [MetadataType(typeof(HelpMenuMetaData))]
    public partial class HelpMenu
    {
    }

    public class HelpMenuMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object MenuId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(100, ErrorMessage = "Maks 100 karakter")]
        public object MenuName { get; set; }

        [DataType(DataType.MultilineText)]
        public object Description { get; set; }
    } 

    #endregion

    #region HelpDoc

    [MetadataType(typeof(HelpDocMetaData))]
    public partial class HelpDoc
    {
    }

    public class HelpDocMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        public object DocId { get; set; }

        [Required(ErrorMessage = "Harus diisi")]
        [StringLength(200, ErrorMessage = "Maks 200 karakter")]
        public object DocName { get; set; }

        [DataType(DataType.MultilineText)]
        public object Description { get; set; }

        [StringLength(100, ErrorMessage = "Maks 100 karakter")]
        public object DocInfo { get; set; }
    } 

    #endregion

    #region RiskEvent

    [MetadataType(typeof(RiskEventMetaData))]
    public partial class RiskEvent
    {
    }

    public class RiskEventMetaData
    {
        [Required(ErrorMessage = "Harus diisi")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Isi dengan angka")]
        public object RiskEventID { get; set; }

        public object RiskEvent1 { get; set; }
    }

    #endregion
}