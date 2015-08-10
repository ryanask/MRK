using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AskrindoMVC.Areas.RiskData.Models.RiskData
{
    public class ImpactSelectViewModel
    {
        public int RiskId { get; set; }
        public Risk Risk { get; set; }
        [Required(ErrorMessage = "Harus diisi")]
        public int ImpactTypeId { get; set; }
        [Required(ErrorMessage = "Harus diisi")]
        public int ImpactLeveliId { get; set; }

        public IEnumerable<ImpactCat> ImpactCats { get; set; }
        public IEnumerable<ImpactLevel> ImpactLevels { get; set; }
        public IEnumerable<ImpactDetail> ImpactDetails { get; set; }
    }
}