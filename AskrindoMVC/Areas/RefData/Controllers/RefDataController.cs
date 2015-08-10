using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskrindoMVC.Areas.RefData.Controllers
{
    [Authorize]
    public class RefDataController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
