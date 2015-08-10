using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AskrindoMVC.Areas.RefData.Models.ImpactLevel
{
    public class ImpactValueViewModel
    {
        public List<ImpactValue> ImpactValues { get; set; }
    }

    public class ImpactValue
    {
        public int ImpactLevelId { get; set; }
        public string ImpactLevelName { get; set; }
        [DisplayFormat(DataFormatString="{0:#,##0.##}")]
        public decimal MinValue { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0.##}")]
        public decimal MaxValue { get; set; }
    }
}