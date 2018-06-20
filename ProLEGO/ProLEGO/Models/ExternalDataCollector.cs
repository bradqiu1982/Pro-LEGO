using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace ProLEGO.Models
{
    public class NativeMethods : IDisposable
    {

        // obtains user token  

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]

        static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,

            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);



        // closes open handes returned by LogonUser  

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        extern static bool CloseHandle(IntPtr handle);
        [DllImport("Advapi32.DLL")]
        static extern bool ImpersonateLoggedOnUser(IntPtr hToken);
        [DllImport("Advapi32.DLL")]
        static extern bool RevertToSelf();
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_LOGON_NEWCREDENTIALS = 2;

        private bool disposed;

        public NativeMethods(string sUsername, string sDomain, string sPassword)
        {

            // initialize tokens  

            IntPtr pExistingTokenHandle = new IntPtr(0);
            IntPtr pDuplicateTokenHandle = new IntPtr(0);
            try
            {
                // get handle to token  
                bool bImpersonated = LogonUser(sUsername, sDomain, sPassword,

                    LOGON32_LOGON_NEWCREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref pExistingTokenHandle);
                if (true == bImpersonated)
                {

                    if (!ImpersonateLoggedOnUser(pExistingTokenHandle))
                    {
                        int nErrorCode = Marshal.GetLastWin32Error();
                        throw new Exception("ImpersonateLoggedOnUser error;Code=" + nErrorCode);
                    }
                }
                else
                {
                    int nErrorCode = Marshal.GetLastWin32Error();
                    throw new Exception("LogonUser error;Code=" + nErrorCode);
                }

            }

            finally
            {
                // close handle(s)  
                if (pExistingTokenHandle != IntPtr.Zero)
                    CloseHandle(pExistingTokenHandle);
                if (pDuplicateTokenHandle != IntPtr.Zero)
                    CloseHandle(pDuplicateTokenHandle);
            }

        }

        protected virtual void Dispose(bool disposing)
        {

            if (!disposed)
            {
                RevertToSelf();
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

    public class ExternalDataCollector
    {
        private static bool FileExist(Controller ctrl, string filename)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    return File.Exists(filename);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private static void FileCopy(Controller ctrl, string src, string des, bool overwrite, bool checklocal = false)
        {
            try
            {


                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    if (checklocal)
                    {
                        if (File.Exists(des))
                        {
                            return;
                        }
                    }

                    File.Copy(src, des, overwrite);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private static bool DirectoryExists(Controller ctrl, string dirname)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    return Directory.Exists(dirname);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        private static List<string> DirectoryEnumerateFiles(Controller ctrl, string dirname)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    var ret = new List<string>();
                    ret.AddRange(Directory.EnumerateFiles(dirname));
                    return ret;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<List<string>> RetrieveDataFromExcel(Controller ctrl, string filename, string sheetname)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);
                var folderuser = syscfgdict["SHAREFOLDERUSER"];
                var folderdomin = syscfgdict["SHAREFOLDERDOMIN"];
                var folderpwd = syscfgdict["SHAREFOLDERPWD"];

                using (NativeMethods cv = new NativeMethods(folderuser, folderdomin, folderpwd))
                {
                    return ExcelReader.RetrieveDataFromExcel(filename, sheetname);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static string ConvertStr(object obj)
        {
            try
            {
                if (obj == null)
                { return string.Empty; }
                else
                { return Convert.ToString(obj); }
            }
            catch (Exception ex) { }
            return string.Empty;
        }

        public static void LoadAllProjects(Controller ctrl)
        {
            //var pjcollist = new List<string>();
            //pjcollist.Add("Project Code");
            //pjcollist.Add("PM");
            //pjcollist.Add("Planner");
            //pjcollist.Add("Project Phase");
            //pjcollist.Add("BU");
            //pjcollist.Add("COST CENTER");
            //pjcollist.Add("Planner Code");
            //pjcollist.Add("Location");
            //pjcollist.Add("R&D Type");
            //pjcollist.Add("Project Group");

            var pjcol = new ProjectColumn();
            pjcol.ColumnName = "Project Code";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "PM";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "Planner";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "Project Phase";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "BU";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "COST CENTER";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "Planner Code";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "Location";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "R&D Type";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            pjcol = new ProjectColumn();
            pjcol.ColumnName = "Project Group";
            pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
            pjcol.AddPJColumn(ctrl.ViewBag.compName);
            new System.Threading.ManualResetEvent(false).WaitOne(1000);

            var sql = @"select (Project+'/'+Series) as ProjectName,Project_Code,Project_Manager,Planner,Phase_in_current_week
                         ,BU,Cost_Center,Location,RD_Type,(Project_Group+'/'+Group1+'/'+Group2) as PJGroup,Planner_Code  FROM [OA].[dbo].[View_OA_Planning_Project_List]";
            var dbret = DBUtility.ExeOASqlWithRes(sql);
            foreach (var line in dbret)
            {
                var pjname = ConvertStr(line[0]);
                if (string.IsNullOrEmpty(pjname)) { continue; }

                ProjectVM.CreateProject(ctrl.ViewBag.compName, pjname);

                var pjcode = ConvertStr(line[1]);
                var pm = ConvertStr(line[2]);
                var planner = ConvertStr(line[3]);

                var phase = ConvertStr(line[4]);
                var bu = ConvertStr(line[5]);
                var costcenter = ConvertStr(line[6]);

                var location = ConvertStr(line[7]);
                var rdtype = ConvertStr(line[8]);
                var pjgroup = ConvertStr(line[9]);
                var plannercode = ConvertStr(line[10]);

                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Project Code", pjcode);
                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "PM", pm);
                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Planner", planner);


                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Project Phase", phase);
                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "BU", bu);
                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "COST CENTER", costcenter);

                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Planner Code", plannercode);

                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Location", location);
                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "R&D Type", rdtype);
                ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Project Group", pjgroup);
            }

        }


        //public static void LoadAllProjects(Controller ctrl)
        //{

        //    var pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Project Code";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "PM";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Owner";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Planner";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Type";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Series";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "NPI Status";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.ColumnDefaultVal = "PIP1;EVT;DVT;MVT;MP";
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "RD Site";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Period";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Buget";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Planner Code";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);


        //    var syscfgdict = CfgUtility.GetSysConfig(ctrl);
        //    var pjlistfolder = syscfgdict["PJLISTFILEFD"];
        //    var pjsrcfss = DirectoryEnumerateFiles(ctrl, pjlistfolder);
        //    foreach (var fl in pjsrcfss)
        //    {
        //        if (fl.ToUpper().Contains("PROJECT LEGO DATA"))
        //        {
        //            try
        //            {
        //                var fn = Path.GetFileName(fl);
        //                fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
        //                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");
        //                string datestring = DateTime.Now.ToString("yyyyMMdd");
        //                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
        //                if (!DirectoryExists(ctrl, imgdir))
        //                    Directory.CreateDirectory(imgdir);
        //                var desfl = imgdir + fn;
        //                FileCopy(ctrl, fl, desfl, true);
        //                if (FileExist(ctrl, desfl))
        //                {
        //                    var alldata = RetrieveDataFromExcel(ctrl, desfl, "Project(NPI&RD)");
        //                    foreach (var line in alldata)
        //                    {
        //                        var pjcode = line[1];
        //                        if (pjcode.ToUpper().Contains("PROJECT CODE") 
        //                            || pjcode.ToUpper().Contains("NA"))
        //                        {
        //                            continue;
        //                        }

        //                        var pjname = line[7];
        //                        var pm = line[16];

        //                        if (!string.IsNullOrEmpty(pjcode)
        //                            && !string.IsNullOrEmpty(pjname)
        //                            && !string.IsNullOrEmpty(pm))
        //                        {
        //                            var owner = line[15];
        //                            var type = line[17] + "/" + line[18] + "/" + line[20];
        //                            var rdsite = line[22];

        //                            var phase = line[27];
        //                            var period = line[28];
        //                            var buget = line[32];

        //                            var plannercode = line[62];
        //                            var series = line[59];
        //                            var planner = line[68];

        //                            ProjectVM.CreateProject(ctrl.ViewBag.compName, pjname);

        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Project Code", pjcode);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "PM", pm);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Owner", owner);

        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Planner", planner);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Type", type);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Series", series);

        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "NPI Status", phase);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "RD Site", rdsite);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Period", period);

        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Buget", buget);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pjname, "Planner Code", plannercode);
        //                        }//check PM
        //                    }//end foreach
        //                }//copied file exist
        //            }
        //            catch (Exception ex) { }
        //        }// src file exist
        //    }// scan file from source fold
        //}

        //public static void LoadAllProjects(Controller ctrl)
        //{
        //    var pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "PM";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.ROLE;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "Project Code";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);

        //    pjcol = new ProjectColumn();
        //    pjcol.ColumnName = "NPI Status";
        //    pjcol.ColumnType = PROJECTCOLUMNTYPE.INFORMATION;
        //    pjcol.ColumnDefaultVal = "PIP1;EVT;DVT;MVT;MP";
        //    pjcol.AddPJColumn(ctrl.ViewBag.compName);


        //    var syscfgdict = CfgUtility.GetSysConfig(ctrl);
        //    var pmdict = CfgUtility.GetEmployDict(ctrl);
        //    var pjlistfolder = syscfgdict["PJLISTFILEFD"];
        //    var pjsrcfss = DirectoryEnumerateFiles(ctrl, pjlistfolder);
        //    foreach (var fl in pjsrcfss)
        //    {
        //        if (fl.ToUpper().Contains("PROJECT LIST"))
        //        {
        //            try
        //            {
        //                var fn = Path.GetFileName(fl);
        //                fn = fn.Replace(" ", "_").Replace("#", "").Replace("'", "")
        //                        .Replace("&", "").Replace("?", "").Replace("%", "").Replace("+", "");
        //                string datestring = DateTime.Now.ToString("yyyyMMdd");
        //                string imgdir = ctrl.Server.MapPath("~/userfiles") + "\\docs\\" + datestring + "\\";
        //                if (!DirectoryExists(ctrl, imgdir))
        //                    Directory.CreateDirectory(imgdir);
        //                var desfl = imgdir + fn;
        //                FileCopy(ctrl, fl, desfl, true);
        //                if (FileExist(ctrl, desfl))
        //                {
        //                    var alldata = RetrieveDataFromExcel(ctrl,desfl, null);
        //                    foreach (var line in alldata)
        //                    {
        //                        if (!string.IsNullOrEmpty(line[15]) 
        //                            && pmdict.ContainsKey(line[15].Replace(" ",".").ToUpper()))
        //                        {
        //                            var pj = line[2].Replace("/", "-").Trim();
        //                            var pjcode = line[4];

        //                            var pjprogress = "MP";
        //                            if (!string.IsNullOrEmpty(line[7]))
        //                            {
        //                                pjprogress = "PIP1";
        //                            }
        //                            if (!string.IsNullOrEmpty(line[8]))
        //                            {
        //                                pjprogress = "EVT";
        //                            }
        //                            if (!string.IsNullOrEmpty(line[9]))
        //                            {
        //                                pjprogress = "DVT";
        //                            }
        //                            if (!string.IsNullOrEmpty(line[10]))
        //                            {
        //                                pjprogress = "MVT";
        //                            }
        //                            if (!string.IsNullOrEmpty(line[11]))
        //                            {
        //                                pjprogress = "MP";
        //                            }
        //                            var PM = line[15].Replace(" ", ".").ToUpper();

        //                            ProjectVM.CreateProject(ctrl.ViewBag.compName, pj);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pj, "PM", PM);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pj, "Project Code", pjcode);
        //                            ProjectVM.UpdateProjectColumnValue(ctrl.ViewBag.compName, pj, "NPI Status", pjprogress);
        //                        }//check PM
        //                    }//end foreach
        //                }//copied file exist
        //            }
        //            catch (Exception ex) { }
        //        }// src file exist
        //    }// scan file from source fold
        //}



    }
}