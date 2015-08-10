using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskrindoMVC.Models;

namespace AskrindoMVC.Areas.Admin.Models.DocMgr
{
    public class DocMgrVM
    {
        public HelpMenu HelpMenu { get; set; }
        public HelpDoc HelpDoc { get; set; }
        public IEnumerable<HelpDoc> HelpDocs { get; set; }
    }
}