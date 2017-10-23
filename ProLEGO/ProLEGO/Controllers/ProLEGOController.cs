using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProLEGO.Models;
using System.IO;

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
			
			ViewBag.isie8 = false;
            ViewBag.isie9 = false;
            ViewBag.showie8modal = false;

            var browse = Request.Browser;
            if (string.Compare(browse.Browser, "IE", true) == 0
                && (string.Compare(browse.Version, "7.0", true) == 0
                || string.Compare(browse.Version, "8.0", true) == 0))
            {
                ViewBag.isie8 = true;
            }
            if (string.Compare(browse.Browser, "IE", true) == 0
                && (string.Compare(browse.Version, "7.0", true) == 0
                || string.Compare(browse.Version, "8.0", true) == 0
                || string.Compare(browse.Version, "9.0", true) == 0))
            {
                ViewBag.isie9 = true;
            }
        }

        public ActionResult MainPage()
        {
            UserAuth();
            if (ViewBag.Admin)
            {
                var sysdict = CfgUtility.GetSysConfig(this);
                var pjlist = new List<string>();
                foreach (var kv in sysdict)
                {
                    if (kv.Key.ToUpper().Contains("PJNAME"))
                    {
                        pjlist.Add(kv.Value);
                    }
                }
                ViewBag.PJList = Newtonsoft.Json.JsonConvert.SerializeObject(pjlist.ToArray());
            }
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

        private List<string> PrepeareAllPJData()
        {
            UserAuth();

            List<ProjectVM> allpro = ProjectVM.RetrieveAllProjectData(null, ViewBag.compName);
            if (allpro.Count > 0)
            {
                var ret = new List<string>();
                var line = "\"" + "Project Name";
                foreach (var col in allpro[0].PJColList)
                {
                    line = line + "\"," + "\"" + col.ColumnName.Replace("\"", "");
                }
                line = line + "\"";
                ret.Add(line);

                foreach (var pj in allpro)
                {
                    line = "\"" + pj.ProjectName.Replace("\"", "");

                    foreach (var kv in pj.ColNameValue)
                    {
                        line = line + "\"," + "\"" + kv.colvalue.Replace("\"", "");
                    }
                    line = line + "\"";
                    ret.Add(line);
                }

                return ret;
            }

            return new List<string>();
        }

        public ActionResult DownloadAllProject()
        {
            UserAuth();

            string datestring = DateTime.Now.ToString("yyyyMMdd");
            string imgdir = Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
            if (!Directory.Exists(imgdir))
            {
                Directory.CreateDirectory(imgdir);
            }

            var fn = "ALL_Project_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var filename = imgdir + fn;

            var lines = PrepeareAllPJData();

            var wholefile = "";
            foreach (var l in lines)
            {
                wholefile = wholefile + l + "\r\n";
            }
            System.IO.File.WriteAllText(filename, wholefile);

            return File(filename, "application/vnd.ms-excel", fn);
        }

        public JsonResult CreateProject()
        {
            UserAuth();
            var ret = new JsonResult();

            if (ViewBag.Admin)
            {
                var ProjectName = Request.Form["pjname"].Replace("'", "").Trim();
                var cret = ProjectVM.CreateProject(ViewBag.compName, ProjectName);
                if (cret)
                {
                    ret.Data = new { success = true, projectkey = ProjectName };
                    new ProjectLog(ViewBag.compName, ProjectName, "ALL", "Create this project");
                }
                else
                {
                    ret.Data = new { success = false, msg = "no project architect exist,please contact administrator to add project architect" };
                }
            }
            else
            {
                ret.Data = new { success = false, msg = "you do not have such authorization,please contact administrator to add authorization" };
            }
            
            return ret;
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

        public ActionResult ManageProColumn()
        {
            UserAuth();
            var pjcollist = ProjectColumn.RetrieveAllPJColumn();
            return View(pjcollist);
        }

        public JsonResult AddProColumn()
        {
            UserAuth();
            var colname = Request.Form["col_name"].Trim();
            var coltype = Request.Form["col_type"].Trim();
            var coldefval = Request.Form["col_value"].Trim();

            var pjcol = new ProjectColumn();
            pjcol.ColumnName = colname;
            pjcol.ColumnType = coltype;
            pjcol.ColumnDefaultVal = coldefval;
            pjcol.AddPJColumn(ViewBag.compName);

            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }

        public JsonResult AddDefColumnValue()
        {
            UserAuth();
            var colname = Request.Form["col_name"].Trim();
            var coldefval = Request.Form["col_val"].Trim();

            var procol = ProjectColumn.RetrievePJColumnByName(colname);
            if (procol.Count > 0)
            {
                ProjectColumn.UpdatePJColDefaultVal(colname, procol[0].ColumnDefaultVal + ";" + coldefval);
            }

            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }

        public JsonResult RemoveDefColumnValue()
        {
            UserAuth();
            var colname = Request.Form["col_name"].Trim();
            var coldefval = Request.Form["col_val"].Trim();

            var procol = ProjectColumn.RetrievePJColumnByName(colname);
            if (procol.Count > 0)
            {
                ProjectColumn.UpdatePJColDefaultVal(colname, procol[0].ColumnDefaultVal.Replace((";" + coldefval),"").Replace(coldefval, ""));
            }

            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }

        public JsonResult UpdateDateDefColumnValue()
        {
            UserAuth();
            var colname = Request.Form["col_name"].Trim();
            var coldefval = Request.Form["col_value"].Trim();

            var procol = ProjectColumn.RetrievePJColumnByName(colname);
            if (procol.Count > 0)
            {
                ProjectColumn.UpdatePJColDefaultVal(colname,coldefval);
            }

            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }

        public JsonResult DisplayPlatformCol()
        {
            UserAuth();
            var colname = Request.Form["column_name"].Trim();
            var display = Request.Form["dis_play"].Trim();

            var procol = ProjectColumn.RetrievePJColumnByName(colname);
            if (procol.Count > 0)
            {
                if (string.Compare(display, "0") == 0)
                {
                    ProjectColumn.UpdatePJColRemoved(colname,true);
                    new ProjectLog(ViewBag.compName, "ALLPJ", "ALLCOL", "Remove Column: " + colname + " at whole platform");
                }
                else
                {
                    ProjectColumn.UpdatePJColRemoved(colname,false);
                    new ProjectLog(ViewBag.compName, "ALLPJ", "ALLCOL", "Show Column: " + colname + " at whole platform");
                }
            }

            var ret = new JsonResult();
            ret.Data = new { success = true };
            return ret;
        }


        //public FileResult DownLoadAllProject()
        //{

        //}

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