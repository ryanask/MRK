using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskrindoMVC.Areas.Report.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
