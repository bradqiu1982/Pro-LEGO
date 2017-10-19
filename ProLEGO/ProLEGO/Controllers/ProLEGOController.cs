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

        public ActionResult AllProjects(int currentpage = 0,string searchkey = null)
        {
            UserAuth();

            ViewBag.NavLeft = false;
            ViewBag.NavRight = false;
            var pagesize = 4;

            if (!string.IsNullOrEmpty(searchkey))
            {
                searchkey = searchkey.Replace("'", "");
                ViewBag.searchkey = searchkey;
            }

            IEnumerable<ProjectVM> allpro = ProjectVM.RetrieveAllProjectData(searchkey, ViewBag.compName);
            IEnumerable<ProjectVM> showvm = null;

            if (currentpage < 0) currentpage = 0;

            if (allpro.Count() > 0)
            {
                if (allpro.Count() > currentpage * pagesize)
                {
                    showvm = allpro.Skip(currentpage * pagesize).Take((allpro.Count() >= ((currentpage + 1) * pagesize)) ? pagesize : (allpro.Count() - currentpage * pagesize));
                }
                else
                {
                    currentpage = 0;
                    showvm = allpro.Skip(currentpage * pagesize).Take((allpro.Count() >= ((currentpage + 1) * pagesize)) ? pagesize : (allpro.Count() - currentpage * pagesize));
                }
            }
            else
            {
                currentpage = 0;
            }

            if (currentpage > 0)
            {
                ViewBag.NavLeft = true;
                ViewBag.LeftPage = currentpage - 1;
            }
            if (allpro.Count() > (currentpage + 1) * pagesize)
            {
                ViewBag.NavRight = true;
                ViewBag.RightPage = currentpage + 1;
            }

            return View(showvm);
        }

        public ActionResult ProjectDetail(string ProjectName)
        {
            UserAuth();
            var pvm = ProjectVM.RetriveProjectData(ProjectName, null, ViewBag.compName);
            return View(pvm);
        }

        public JsonResult SaveProjectData()
        {
            UserAuth();
            var projectkey = Request.Form["project_key"];
            var data = (List<List<string>>)Newtonsoft.Json.JsonConvert.DeserializeObject(Request.Form["data"],(new List<List<string>>()).GetType());
            foreach (var line in data)
            {
                if (string.Compare(line[1], "0") == 0)
                {
                    var fbc = new MachineColumn();
                    fbc.ColumnName = line[2];
                    fbc.MachineName = ViewBag.compName;
                    fbc.AddFobiddenColumn();
                }
                else
                {
                    MachineColumn.RemoveFobiddenColumn(ViewBag.compName, line[2]);
                }

                ProjectVM.UpdateProjectColumnValue(ViewBag.compName, projectkey, line[2], line[3]);
            }

            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }

        public ActionResult AddProColumn()
        {
            UserAuth();
            var pjcollist = ProjectColumn.RetrieveAllPJColumn();
            return View(pjcollist);
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

            new System.Threading.ManualResetEvent(false).WaitOne(1500);
            ProjectVM.CreateProject(ViewBag.compName, "SFP+");
            new System.Threading.ManualResetEvent(false).WaitOne(1500);
            ProjectVM.CreateProject(ViewBag.compName, "Coherent");
            new System.Threading.ManualResetEvent(false).WaitOne(1500);
            ProjectVM.CreateProject(ViewBag.compName, "QSFP PSM4");

            //ProjectVM.UpdateProjectColumnValue(ViewBag.compName, "CFP4 SR4", "PM", "Meg.Wang");
            //ProjectVM.UpdateProjectColumnValue(ViewBag.compName, "QSFP 28G SR4", "PM", "Steven.Qiu");
            //ProjectVM.UpdateProjectColumnValue(ViewBag.compName, "QSFP 28G SR4", "Start Date", "2017-10-16 10:00:00");

            return View("MainPage");
        }

    }
}