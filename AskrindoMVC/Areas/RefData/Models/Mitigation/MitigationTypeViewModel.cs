using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.RefData.Models.Mitigation
{
    public class MitigationTypeViewModel
    {
        public MitigationType MitigationType { get; set; }
        public IEnumerable<MitigationType> MitigationTypes { get; set; }
        public MitigationCat MitigationCat { get; set; }
    }
}