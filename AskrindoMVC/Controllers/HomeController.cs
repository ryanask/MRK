using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace AskrindoMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult MyChart()
        {
            var bytes = new Chart(width: 600, height: 300)
                .AddTitle("RISK MANAGEMENT")
                .AddLegend()
                .SetXAxis("Mata Pelajaran")
                .SetYAxis("Nilai Ujian Akhir")
                .AddSeries(name: "Class A", 
                    chartType: "column", 
                    xValue: new[] {"Math", "English", "Computer", "Physics"}, 
                    yValues: new[] {60, 70, 68, 88})
                .AddSeries(name: "Class B",
                    chartType: "column",
                    yValues: new[] { 68, 56, 85, 70 })
                .GetBytes("png");
            return File(bytes, "image/png");
        }
    }
}
