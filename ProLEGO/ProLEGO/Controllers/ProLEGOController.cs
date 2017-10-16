using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProLEGO.Models;

namespace ProLEGO.Controllers
{
    public class ProLEGOController : Controller
    {
        public static string DetermineCompName(string IP)
        {
            try
            {
                IPAddress myIP = IPAddress.Parse(IP);
                IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                return compName.First();
            }
            catch (Exception ex)
            { return string.Empty; }
        }

        private void UserAuth()
        {
            string IP = Request.UserHostName;
            var compName = DetermineCompName(IP);
            ViewBag.compName = compName;

            var adminmachine = CfgUtility.GetSysConfig(this)["ADMINMACHINE"];
            if (!string.IsNullOrEmpty(compName) && adminmachine.ToUpper().Contains(compName.ToUpper()))
            {
                ViewBag.Admin = true;
            }
            else
            {
                ViewBag.Admin = false;
            }
        }

        public ActionResult MainPage()
        {
            UserAuth();
            return View();
        }

        public ActionResult HeartBeat()
        {
            UserAuth();
            var pjcol = new ProjectColumn();
            pjcol.ColumnName = "Project Code";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.ColumnDefaultVal = "S109;S107;S108";
            pjcol.AddPJColumn(ViewBag.compName);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "NPI Status";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.ColumnDefaultVal = "PIP1;EVT;DVT;MVT;MP";
            pjcol.AddPJColumn(ViewBag.compName);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "Start Date";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.DATE;
            pjcol.AddPJColumn(ViewBag.compName);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "PM";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
            pjcol.AddPJColumn(ViewBag.compName);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "PQE";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
            pjcol.AddPJColumn(ViewBag.compName);

            ProjectVM.CreateProject(ViewBag.compName, "QSFP 28G SR4");

            ProjectVM.CreateProject(ViewBag.compName, "CFP4 SR4");

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "CQE";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
            pjcol.AddPJColumn(ViewBag.compName);

            ProjectVM.UpdateProjectColumnValue(ViewBag.compName, "CFP4 SR4", "PM", "Meg.Wang");
            ProjectVM.UpdateProjectColumnValue(ViewBag.compName, "QSFP 28G SR4", "PM", "Steven.Qiu");
            ProjectVM.UpdateProjectColumnValue(ViewBag.compName, "QSFP 28G SR4", "Start Date", "2017-10-16 10:00:00");

            return View("MainPage");
        }

    }
}