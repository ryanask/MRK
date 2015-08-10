using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using AskrindoMVC.Helpers;
using System.IO;
using System.Web.UI;

namespace AskrindoMVC.Controllers
{
    public class TestController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        public ActionResult Index()
        {
            return View();
        }

        public void GetExcel()
        {
            var usr = from p in db.UserInfos
                      select new { p.FullName, p.JobTitle, p.Dept.DeptName, p.Division.DivisionName, p.Branch.BranchName };
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = usr;
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=test.xls");
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            grid.RenderControl(hw);
            Response.Write(sw.ToString());
            Response.End();
        }

        public void GetExcelManual()
        {
            string[] titles = { "FullName", "JobTitle", "Department" };

            var usr = db.UserInfos.OrderBy(p => p.FullName);
            StringWriter sw = new StringWriter();
            sw.WriteLine("<table rules='all' border='1' style='border-collapse:collapse;'>");
            sw.WriteLine("<tr");
            foreach(var s in titles)
                sw.WriteLine("<th style='background-color: #eee'>{0}</th>", s);
            //sw.WriteLine("<th style='background-color: #eee'>Nama Lengkap</th>");
            //sw.WriteLine("<th style='background-color: #eee'>Jabatan</th>");
            //sw.WriteLine("<th style='background-color: #eee'>Departemen</th>");
            sw.WriteLine("</tr>");
            foreach (var m in usr)
            {
                sw.WriteLine("<tr>");
                sw.WriteLine(string.Format("<td>{0}</td>", m.FullName));
                sw.WriteLine(string.Format("<td>{0}</td>", m.JobTitle));
                sw.WriteLine(string.Format("<td>{0}</td>", m.Dept.DeptName));
                sw.WriteLine("</tr>");
            }
            sw.WriteLine("</table>");

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=test.xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Write(sw.ToString());
            Response.End();
        }
    }
}
