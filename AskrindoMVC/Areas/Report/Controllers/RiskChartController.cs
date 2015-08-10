using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using System.ComponentModel.DataAnnotations;
using AskrindoMVC.Areas.Report.Models.RiskChart;
using AskrindoMVC.Helpers;
using System.Web.Helpers;
using System.IO;
//using System.Web.UI.DataVisualization.Charting;

namespace AskrindoMVC.Areas.Report.Controllers
{
    [Authorize]
    public class RiskChartController : Controller
    {
        private AskrindoMVCEntities db = new AskrindoMVCEntities();

        public const int GROUP_NONE = 1;
        public const int GROUP_CAUSE = 2;
        public const int GROUP_EFFECT = 3;
        public const int GROUP_RISKCLASS = 4;
        public const int GROUP_HQ_BRANCH = 5;
        public const int GROUP_CLASSBRANCH = 6;
        public const int GROUP_BRANCH_CLASS1 = 7;
        public const int GROUP_BRANCH_CLASS2 = 8;
        public const int GROUP_BRANCH_CLASS3 = 9;
        public const int GROUP_PROBLEVEL = 10;
        public const int GROUP_IMPACTLEVEL = 11;
        public const int GROUP_RISKLEVEL = 12;

        public const int DATA_COUNT = 1;
        public const int DATA_PROBLEVEL = 2;
        public const int DATA_IMPACTLEVEL = 3;
        public const int DATA_PROBIMPACTLEVEL = 4;
        public const int DATA_RISKLEVEL = 5;

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            RiskChartViewModel vm = new RiskChartViewModel();
            vm.PosId = 1;
            vm.IsApproved = true;
            vm.ReportDate = DateTime.Now;
            vm.ChartTypeId = 1;
            vm.XValueId = 1;
            vm.YValueId = 1;

            UpdateParam(vm);
            var risks = CalcRisks(vm);
            var listData = CalcChartData(risks, vm);
            Session["ListData"] = listData;
            Session["RiskChartViewModel"] = vm;
            ViewBag.DataTypeText = GetDataTypeText(vm.YValueId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(RiskChartViewModel vm)
        {
            UpdateParam(vm);
            var risks = CalcRisks(vm);
            var listData = CalcChartData(risks, vm);
            Session["ListData"] = listData;
            Session["RiskChartViewModel"] = vm;
            ViewBag.DataTypeText = GetDataTypeText(vm.YValueId);
            return View(vm);
        }

        public ActionResult ChartGenerator()
        {
            List<ChartData> listData = Session["ListData"] as List<ChartData>;
            RiskChartViewModel vm = Session["RiskChartViewModel"] as RiskChartViewModel;
            
            List<string> xValues = new List<string>();
            List<decimal> yValues = new List<decimal>();
            List<decimal> yValues2 = new List<decimal>();
            foreach (var data in listData)
            {
                xValues.Add(data.Name);
                yValues.Add(data.Value);
                yValues2.Add(data.Value2);
            }

            string seriesName = string.Empty;
            string seriesName2 = string.Empty;
            if (vm.YValueId == DATA_COUNT)
                seriesName = "Banyak Data";
            else if (vm.YValueId == DATA_PROBLEVEL)
                seriesName = "Tingkat Probabilitas";
            else if (vm.YValueId == DATA_IMPACTLEVEL)
                seriesName = "Tingkat Dampak";
            else if (vm.YValueId == DATA_PROBIMPACTLEVEL)
            {
                seriesName = "Tingkat Probabilitas";
                seriesName2 = "Tingkat Dampak";
            }
            else
                seriesName = "Tingkat Risiko";

            string chartType = "column";
            int chartTypeId = vm.ChartTypeId;
            if (chartTypeId == 2)
                chartType = "bar";
            else if (chartTypeId == 3)
                chartType = "pie";

            string xAxisTitle = GetXAxisTitle(vm.XValueId);
            string yAxisTitle = GetYAxisTitle(vm.YValueId);

            var cht = new Chart(width: 600, height: 400, theme: ChartTheme.Blue)
                .SetXAxis(xAxisTitle)
                .SetYAxis(yAxisTitle)
                .AddSeries(name: seriesName,
                    chartType: chartType, 
                    xValue: xValues,
                    yValues: yValues, markerStep: 1);
            if (!string.IsNullOrEmpty(seriesName2))
            {
                cht = cht
                    .AddSeries(name: seriesName2,
                        chartType: chartType,
                        xValue: xValues,
                        yValues: yValues2)
                    .AddLegend();
            }

            var bytes = cht.GetBytes("jpg");
            return File(bytes, "image/jpg");
        }

        public FileContentResult ExportChart()
        {
            List<ChartData> listData = Session["ListData"] as List<ChartData>;
            RiskChartViewModel vm = Session["RiskChartViewModel"] as RiskChartViewModel;

            List<string> xValues = new List<string>();
            List<decimal> yValues = new List<decimal>();
            List<decimal> yValues2 = new List<decimal>();
            foreach (var data in listData)
            {
                xValues.Add(data.Name);
                yValues.Add(data.Value);
                yValues2.Add(data.Value2);
            }

            string seriesName = string.Empty;
            string seriesName2 = string.Empty;
            if (vm.YValueId == DATA_COUNT)
                seriesName = "Banyak Data";
            else if (vm.YValueId == DATA_PROBLEVEL)
                seriesName = "Tingkat Probabilitas";
            else if (vm.YValueId == DATA_IMPACTLEVEL)
                seriesName = "Tingkat Dampak";
            else if (vm.YValueId == DATA_PROBIMPACTLEVEL)
            {
                seriesName = "Tingkat Probabilitas";
                seriesName2 = "Tingkat Dampak";
            }
            else
                seriesName = "Tingkat Risiko";

            string chartType = "column";
            int chartTypeId = vm.ChartTypeId;
            if (chartTypeId == 2)
                chartType = "bar";
            else if (chartTypeId == 3)
                chartType = "pie";

            string xAxisTitle = GetXAxisTitle(vm.XValueId);
            string yAxisTitle = GetYAxisTitle(vm.YValueId);

            var cht = new Chart(width: 600, height: 400, theme: ChartTheme.Blue)
                .SetXAxis(xAxisTitle)
                .SetYAxis(yAxisTitle)
                .AddSeries(name: seriesName,
                    chartType: chartType,
                    xValue: xValues,
                    yValues: yValues, markerStep: 1);
            if (!string.IsNullOrEmpty(seriesName2))
            {
                cht = cht
                    .AddSeries(name: seriesName2,
                        chartType: chartType,
                        xValue: xValues,
                        yValues: yValues2)
                    .AddLegend();
            }
            var bytes = cht.GetBytes("jpg");
            return File(bytes, "image/jpeg", "grafik.jpg");
        }

        private string GetXAxisTitle(int xValueId)
        {
            switch (xValueId)
            {
                //case GROUP_NONE:
                //    return "Semua Data";
                case GROUP_CAUSE:
                    return "Sebab Risiko";
                case GROUP_EFFECT:
                    return "Akibat Risiko";
                case GROUP_RISKCLASS:
                    return "Klasifikasi Risiko";
                case GROUP_HQ_BRANCH:
                    return "Pusat/Cabang";
                case GROUP_CLASSBRANCH:
                    return "Kelas Cabang";
                case GROUP_BRANCH_CLASS1:
                    return "Cabang Kelas I";
                case GROUP_BRANCH_CLASS2:
                    return "Cabang Kelas II";
                case GROUP_BRANCH_CLASS3:
                    return "Cabang Kelas III";
                case GROUP_PROBLEVEL:
                    return "Tingkat Probabilitas";
                case GROUP_IMPACTLEVEL:
                    return "Tingkat Dampak";
                case GROUP_RISKLEVEL:
                    return "Tingkat Risiko";
                default:
                    return string.Empty;
            }
        }

        private string GetYAxisTitle(int yValueId)
        {
            switch (yValueId)
            {
                case DATA_COUNT:
                    return "Jumlah Data";
                case DATA_PROBLEVEL:
                    return "Tingkat Probabilitas";
                case DATA_IMPACTLEVEL:
                    return "Tingkat Dampak";
                case DATA_PROBIMPACTLEVEL:
                    return "Tingkat Probabilitas dan Dampak";
                case DATA_RISKLEVEL:
                    return "Tingkat Risiko";
                default:
                    return string.Empty;
            }
        }

        private void UpdateParam(RiskChartViewModel vm)
        {
            Dictionary<int, string> posList = new Dictionary<int, string>();
            posList.Add(1, "Kantor Pusat");
            posList.Add(2, "Cabang");
            vm.PosList = new SelectList(posList, "Key", "Value", vm.PosId);

            Dictionary<int, string> branchList = new Dictionary<int, string>();
            foreach (var branch in db.Branches.OrderBy(m => m.ClassId).ThenBy(m => m.BranchName))
                branchList.Add(branch.BranchId, branch.BranchName + " (Kelas " + branch.BranchClass.ClassName + ")");
            vm.Branches = new SelectList(branchList, "Key", "Value", vm.BranchId);

            Dictionary<int, string> chartTypes = new Dictionary<int, string>();
            chartTypes.Add(1, "Kolom");
            chartTypes.Add(2, "Bar");
            chartTypes.Add(3, "Pie");
            vm.ChartTypes = new SelectList(chartTypes, "Key", "Value", vm.ChartTypeId);

            Dictionary<int, string> xValues = new Dictionary<int, string>();
            xValues.Add(1, "(Tanpa pengelompokan)");
            xValues.Add(2, "Sebab Risiko");
            xValues.Add(3, "Akibat Risiko");
            xValues.Add(4, "Klasifikasi Risiko");
            xValues.Add(5, "Pusat/Cabang");
            xValues.Add(6, "Kelas Cabang");
            xValues.Add(7, "Cabang Kelas I");
            xValues.Add(8, "Cabang Kelas II");
            xValues.Add(9, "Cabang Kelas III");
            xValues.Add(10, "Tingkat Probabilitas");
            xValues.Add(11, "Tingkat Dampak");
            xValues.Add(12, "Tingkat Risiko");
            vm.XValues = new SelectList(xValues, "Key", "Value", vm.XValueId);

            Dictionary<int, string> yValues = new Dictionary<int, string>();
            yValues.Add(1, "Jumlah Data");
            yValues.Add(2, "Tingkat Probabilitas");
            yValues.Add(3, "Tingkat Dampak");
            yValues.Add(4, "Tingkat Probabilitas dan Tingkat Dampak");
            yValues.Add(5, "Tingkat Risiko");
            vm.YValues = new SelectList(yValues, "Key", "Value", vm.YValueId);
        }

        private IEnumerable<Risk> CalcRisks(RiskChartViewModel vm)
        {
            var risks = db.Risks.Where(p => p.ProbLevelId != null && p.ImpactLevelId != null);
            if (vm.PosId == 1)
                risks = risks.Where(p => p.DeptId != null);
            else if (vm.PosId == 2)
            {
                risks = risks.Where(p => p.BranchId != null);
                if (vm.BranchId != null)
                    risks = risks.Where(p => p.BranchId == vm.BranchId);
            }

            if (vm.IsApproved)
            {
                risks = risks.Where(p => p.CloseDate == null || p.CloseDate > vm.ReportDate);
                risks = risks.Where(p => p.ApprovalDate != null && p.ApprovalDate <= vm.ReportDate);
            }
            else
            {
                risks = risks.Where(p => p.ApprovalDate == null && p.RiskDate <= vm.ReportDate);
            }
            return risks;
        }

        private List<ChartData> CalcChartData(IEnumerable<Risk> risks, RiskChartViewModel vm)
        {
            List<ChartData> listData = new List<ChartData>();
            InitializeListData(listData, vm.XValueId);

            switch (vm.XValueId)
            {
                case GROUP_NONE:
                    CalcChartData_NoGroup(listData, risks, vm.YValueId);
                    break;
                case GROUP_CAUSE:
                    CalcChartData_Cause(listData, risks, vm.YValueId);
                    break;
                case GROUP_EFFECT:
                    CalcChartData_Effect(listData, risks, vm.YValueId);
                    break;
                case GROUP_RISKCLASS:
                    CalcChartData_RiskClass(listData, risks, vm.YValueId);
                    break;
                case GROUP_HQ_BRANCH:
                    CalcChartData_HQBranch(listData, risks, vm.YValueId);
                    break;
                case GROUP_CLASSBRANCH:
                    CalcChartData_ClassBranch(listData, risks, vm.YValueId);
                    break;
                case GROUP_BRANCH_CLASS1:
                    CalcChartData_Branch1(listData, risks, vm.YValueId);
                    break;
                case GROUP_BRANCH_CLASS2:
                    CalcChartData_Branch2(listData, risks, vm.YValueId);
                    break;
                case GROUP_BRANCH_CLASS3:
                    CalcChartData_Branch3(listData, risks, vm.YValueId);
                    break;
                case GROUP_PROBLEVEL:
                    CalcChartData_ProbLevel(listData, risks, vm.YValueId);
                    break;
                case GROUP_IMPACTLEVEL:
                    CalcChartData_ImpactLevel(listData, risks, vm.YValueId);
                    break;
                case GROUP_RISKLEVEL:
                    CalcChartData_RiskLevel(listData, risks, vm.YValueId);
                    break;
            }

            foreach (var data in listData)
            {
                if (data.Count > 0)
                {
                    switch (vm.YValueId)
                    {
                        case DATA_COUNT:
                            //data.Value++;
                            break;
                        case DATA_PROBLEVEL:
                            data.Value = data.Value / data.Count;
                            break;
                        case DATA_IMPACTLEVEL:
                            data.Value = data.Value / data.Count;
                            //data.Value += (int)r.ImpactLevelId;
                            break;
                        case DATA_PROBIMPACTLEVEL:
                            data.Value = data.Value / data.Count;
                            data.Value2 = data.Value2 / data.Count;
                            break;
                        case DATA_RISKLEVEL:
                            data.Value = data.Value / data.Count;
                            break;
                    }
                }
            }

            return listData;
        }

        private static void GetChartDataValue(Risk r, ChartData data, int yValueId)
        {
            switch (yValueId)
            {
                case DATA_COUNT:
                    data.Value++;
                    break;
                case DATA_PROBLEVEL:
                    data.Value += (int)r.ProbLevelId;
                    break;
                case DATA_IMPACTLEVEL:
                    data.Value += (int)r.ImpactLevelId;
                    break;
                case DATA_PROBIMPACTLEVEL:
                    data.Value += (int)r.ProbLevelId;
                    data.Value2 += (int)r.ImpactLevelId;
                    break;
                case DATA_RISKLEVEL:
                    data.Value += (int)r.RiskLevel;
                    break;
            }
            data.Count++;
        }

        private void CalcChartData_NoGroup(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                var data = listData[0];
                GetChartDataValue(r, data, yValueId);
            }
        }

        private void CalcChartData_Cause(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                foreach(var data in listData)
                    if (data.Id == r.CauseGroupId)
                    {
                        GetChartDataValue(r, data, yValueId);
                        //switch (yValueId)
                        //{
                        //    case DATA_COUNT:
                        //        listData[0].Value++;
                        //        break;
                        //    case DATA_PROBLEVEL:
                        //        listData[0].Value += r.ProbLevelId;
                        //        break;
                        //    case DATA_IMPACTLEVEL:
                        //        listData[0].Value += r.ImpactLevelId;
                        //        break;
                        //    case DATA_PROBIMPACTLEVEL:
                        //        listData[0].Value += r.ProbLevelId;
                        //        listData[0].Value2 += r.ImpactLevelId;
                        //    case DATA_RISKLEVEL:
                        //        listData[0].Value += r.RiskLevel;
                        //        break;
                        //}
                        //data.Count++;
                        break;
                    }
            }
        }

        private void CalcChartData_Effect(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                foreach (var data in listData)
                    if (data.Id == r.EffectGroupId)
                    {
                        GetChartDataValue(r, data, yValueId);
                        break;
                    }
            }
        }

        private void CalcChartData_RiskClass(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                foreach (var data in listData)
                    if (data.Id == r.RiskCatId)
                    {
                        GetChartDataValue(r, data, yValueId);
                        break;
                    }
            }
        }

        private void CalcChartData_HQBranch(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                ChartData data;
                if (r.DeptId != null)
                    data = listData[0];
                else
                    data = listData[1];
                GetChartDataValue(r, data, yValueId);
            }
        }

        private void CalcChartData_ClassBranch(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                if (r.BranchId != null)
                {
                    foreach (var data in listData)
                        if (data.Id == r.Branch.ClassId)
                        {
                            GetChartDataValue(r, data, yValueId);
                            //data.Count++;
                            break;
                        }
                }
            }
        }

        private void CalcChartData_Branch1(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                if (r.Branch != null && r.Branch.ClassId == Utils.BRANCHCLASS1)
                {
                    foreach (var data in listData)
                        if (data.Id == r.BranchId)
                        {
                            GetChartDataValue(r, data, yValueId);
                            //data.Count++;
                            break;
                        }
                }
            }
        }

        private void CalcChartData_Branch2(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                if (r.Branch != null && r.Branch.ClassId == Utils.BRANCHCLASS2)
                {
                    foreach (var data in listData)
                        if (data.Id == r.BranchId)
                        {
                            GetChartDataValue(r, data, yValueId);
                            //data.Count++;
                            break;
                        }
                }
            }
        }

        private void CalcChartData_Branch3(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                if (r.Branch != null && r.Branch.ClassId == Utils.BRANCHCLASS3)
                {
                    foreach (var data in listData)
                        if (data.Id == r.BranchId)
                        {
                            GetChartDataValue(r, data, yValueId);
                            //data.Count++;
                            break;
                        }
                }
            }
        }

        private void CalcChartData_ProbLevel(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                foreach (var data in listData)
                    if (data.Id == r.ProbLevelId)
                    {
                        GetChartDataValue(r, data, yValueId);
                        //data.Count++;
                        break;
                    }
            }
        }

        private void CalcChartData_ImpactLevel(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                foreach (var data in listData)
                    if (data.Id == r.ImpactLevelId)
                    {
                        GetChartDataValue(r, data, yValueId);
                        //data.Count++;
                        break;
                    }
            }
        }

        private void CalcChartData_RiskLevel(List<ChartData> listData, IEnumerable<Risk> risks, int yValueId)
        {
            foreach (var r in risks)
            {
                foreach (var data in listData)
                    if (data.Id == r.RiskLevel)
                    {
                        GetChartDataValue(r, data, yValueId);
                        //data.Count++;
                        break;
                    }
            }
        }

        private void InitializeListData(List<ChartData> listData, int xValueId)
        {
            listData.Clear();

            switch(xValueId)
            {
                case GROUP_NONE:
                    listData.Add(new ChartData() { Id = 1, Name = "Semua Data", Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_CAUSE:
                    var causeGroups = db.CauseGroups.OrderBy(p => p.CauseGroupId);
                    foreach (var m in causeGroups)
                        listData.Add(new ChartData() { Id = m.CauseGroupId, Name = m.CauseGroupName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_EFFECT:
                    var effectGroups = db.EffectGroups.OrderBy(p => p.EffectGroupId);
                    foreach (var m in effectGroups)
                        listData.Add(new ChartData() { Id = m.EffectGroupId, Name = m.EffectGroupName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_RISKCLASS:
                    var riskCats = db.RiskCats.OrderBy(p => p.RiskCatId);
                    foreach (var m in riskCats)
                        listData.Add(new ChartData() { Id = m.RiskCatId, Name = m.RiskCatName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_HQ_BRANCH:
                    listData.Add(new ChartData() { Id = 1, Name = "Kantor Pusat", Count = 0, Value = 0, Value2 = 0 });
                    listData.Add(new ChartData() { Id = 2, Name = "Kantor Cabang", Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_CLASSBRANCH:
                    var classes = db.BranchClasses.OrderBy(p => p.ClassId);
                    foreach (var m in classes)
                        listData.Add(new ChartData() { Id = m.ClassId, Name = "Kelas " + m.ClassName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_BRANCH_CLASS1:
                    var branches = db.Branches.Where(p => p.ClassId == Utils.BRANCHCLASS1).OrderBy(p => p.BranchId);
                    foreach (var m in branches)
                        listData.Add(new ChartData() { Id = m.BranchId, Name = m.BranchName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_BRANCH_CLASS2:
                    var branches2 = db.Branches.Where(p => p.ClassId == Utils.BRANCHCLASS2).OrderBy(p => p.BranchId);
                    foreach (var m in branches2)
                        listData.Add(new ChartData() { Id = m.BranchId, Name = m.BranchName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_BRANCH_CLASS3:
                    var branches3 = db.Branches.Where(p => p.ClassId == Utils.BRANCHCLASS3).OrderBy(p => p.BranchId);
                    foreach (var m in branches3)
                        listData.Add(new ChartData() { Id = m.BranchId, Name = m.BranchName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_PROBLEVEL:
                    var probs = db.ProbLevels.OrderBy(p => p.ProbLevelId);
                    foreach (var m in probs)
                        listData.Add(new ChartData() { Id = m.ProbLevelId, Name = m.ProbLevelId.ToString() + " - " + m.ProbLevelName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_IMPACTLEVEL:
                    var impacts = db.ImpactLevels.OrderBy(p => p.ImpactLevelId);
                    foreach (var m in impacts)
                        listData.Add(new ChartData() { Id = m.ImpactLevelId, Name = m.ImpactLevelId.ToString() + " - " + m.ImpactLevelName, Count = 0, Value = 0, Value2 = 0 });
                    break;
                case GROUP_RISKLEVEL:
                    List<int> list = new List<int>();
                    for (var i = 1; i <= 5; i++)
                        for (var j = 1; j <= 5; j++)
                            if (!list.Contains(i * j))
                                list.Add(i * j);
                    foreach (var i in list)
                        listData.Add(new ChartData() { Id = i, Name = i.ToString(), Count = 0, Value = 0, Value2 = 0 });
                    break;
            }
        }

        private string GetDataTypeText(int dataTypeId)
        {
            switch (dataTypeId)
            {
                case DATA_COUNT:
                    return "Banyak Peristiwa Risiko";
                case DATA_PROBLEVEL:
                    return "Tingkat Probabilitas";
                case DATA_IMPACTLEVEL:
                    return "Tingkat Dampak";
                case DATA_PROBIMPACTLEVEL:
                    return "Tingkat Probabilitas dan Tingkat Dampak";
                case DATA_RISKLEVEL:
                    return "Tingkat Risiko";
                default:
                    return string.Empty;
            }
        }
    }
}
