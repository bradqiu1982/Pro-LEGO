using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProLEGO.Models
{
    public class ProjectVM
    {
        public ProjectVM()
        {
            ProjectName = "";
            PJColList = new List<ProjectColumn>();
            ColNameValue = new List<KeyValuePair<string, string>>();
            UpdateTime = DateTime.Parse("1982-05-06 10:00:00");
        }

        public string ProjectName { set; get; }
        public List<ProjectColumn> PJColList { set; get; }
        public List<KeyValuePair<string, string>> ColNameValue { set; get; }
        public DateTime UpdateTime { set; get; }

        public List<KeyValuePair<string, string>> PJMemList
        {
            get {
                var ret = new List<KeyValuePair<string, string>>();
                if (PJColList.Count > 0 && ColNameValue.Count > 0)
                {
                    var coltypedict = new Dictionary<string, string>();
                    foreach (var col in PJColList)
                    {
                        coltypedict.Add(col.ColumnName, col.ColumnType);
                    }

                    foreach (var kv in ColNameValue)
                    {
                        if (coltypedict.ContainsKey(kv.Key))
                        {
                            if (string.Compare(coltypedict[kv.Key], PROJECTCOLUMNTYPE.ROLE, true) == 0)
                            {
                                ret.Add(new KeyValuePair<string, string>(kv.Key,kv.Value));
                            }
                        }
                    }//end foreach
                }//end if
                return ret;
            }
        }

        public static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static bool CreateProject(string machine,string ProjectName)
        {
            if (ProjectExist(ProjectName))
                return false;
            var allcolumn = ProjectColumn.RetrieveAllPJColumn();
            if (allcolumn.Count == 0)
                return false;

            foreach (var item in allcolumn)
            {
                var sql = "insert into ProjectVM(ProjectName,ColumnName,ColumnID,ColumnValue,UpdateTime) values('<ProjectName>','<ColumnName>','<ColumnID>','<ColumnValue>','<UpdateTime>')";
                sql = sql.Replace("<ProjectName>", ProjectName).Replace("<ColumnName>", item.ColumnName).Replace("<ColumnID>", item.ColumnID)
                .Replace("<ColumnValue>","").Replace("<UpdateTime>", "1982-05-06 10:00:00");

                DBUtility.ExeLocalSqlNoRes(sql);
            }

            var csql = "update ProjectVM set UpdateTime = '<UpdateTime>' where ProjectName = '<ProjectName>'";
            csql = csql.Replace("<UpdateTime>", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")).Replace("<ProjectName>", ProjectName);
            DBUtility.ExeLocalSqlNoRes(csql);

            new ProjectLog(machine, ProjectName, "ALLCOL", "Add project: " + ProjectName);

            return true;
        }

        public static List<KeyValuePair<string,string>> RetrieveAllProjectName()
        {
            var ret = new List<KeyValuePair<string, string>>();

            var pjlist = new List<string>();
            var sql = "select distinct ProjectName from ProjectVM order by ProjectName";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                pjlist.Add(Convert.ToString(line[0]));
            }

            foreach (var pj in pjlist)
            {
                var csql = "select top 1 UpdateTime from ProjectVM where ProjectName = '<ProjectName>'";
                csql = csql.Replace("<ProjectName>", pj);
                var cdbret = DBUtility.ExeLocalSqlWithRes(csql);
                var temppair = new KeyValuePair<string, string>(pj,Convert.ToDateTime(cdbret[0][0]).ToString("yyyy-MM-dd hh:mm:ss"));
                ret.Add(temppair);
            }

            ret.Sort(delegate (KeyValuePair<string, string> pair1, KeyValuePair<string, string> pair2)
            {
                var time1 = DateTime.Parse(pair1.Value);
                var time2 = DateTime.Parse(pair2.Value);
                return time2.CompareTo(time1);
            });

            return ret;
        }

        public static void AddNewColumn(string ColumnName, string ColumnID)
        {
            var allpj = RetrieveAllProjectName();
            foreach (var pj in allpj)
            {
                var sql = "insert into ProjectVM(ProjectName,ColumnName,ColumnID,ColumnValue,UpdateTime) values('<ProjectName>','<ColumnName>','<ColumnID>','<ColumnValue>','<UpdateTime>')";
                sql = sql.Replace("<ProjectName>", pj.Key).Replace("<ColumnName>", ColumnName).Replace("<ColumnID>", ColumnID)
                    .Replace("<ColumnValue>", "").Replace("<UpdateTime>", pj.Value);
                DBUtility.ExeLocalSqlNoRes(sql);
            }
        }

        public static bool ProjectExist(string ProjectName)
        {
            var sql = "select ProjectName from ProjectVM where ProjectName = '<ProjectName>'";
            sql = sql.Replace("<ProjectName>", ProjectName);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static void UpdateProjectColumnValue(string machine,string ProjectName, string ColumnName, string Value)
        {
            var coldict = ProjectColumn.RetrieveAllPJColumnDict();
            if (!coldict.ContainsKey(ColumnName))
                return;

            var sql = "update ProjectVM set ColumnValue = '<ColumnValue>' where  ProjectName = '<ProjectName>' and ColumnName='<ColumnName>'";
            sql = sql.Replace("<ProjectName>", ProjectName).Replace("<ColumnName>", ColumnName).Replace("<ColumnValue>", Value);
            DBUtility.ExeLocalSqlNoRes(sql);
            
            var csql = "update ProjectVM set UpdateTime = '<UpdateTime>' where ProjectName = '<ProjectName>'";
            csql = csql.Replace("<UpdateTime>", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")).Replace("<ProjectName>", ProjectName);
            DBUtility.ExeLocalSqlNoRes(csql);

            new ProjectLog(machine, ProjectName, ColumnName, "Add value: " + Value);
        }


        public static ProjectVM RetriveProjectData(string pjname, List<ProjectColumn> defcols)
        {
            var ret = new ProjectVM();

            var pjcollist = defcols;
            if (pjcollist == null)
            {
                pjcollist = ProjectColumn.RetrieveAllPJColumn();
            }

            var colvaluedict = new Dictionary<string, string>();
            var sql = "select ProjectName,ColumnName,ColumnValue,UpdateTime from ProjectVM where ProjectName = '<ProjectName>'";
            sql = sql.Replace("<ProjectName>", pjname);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.ProjectName = Convert.ToString(line[0]);
                var tempcolname = Convert.ToString(line[1]);
                var colvalue = Convert.ToString(line[2]);
                colvaluedict.Add(tempcolname, colvalue);
                ret.UpdateTime = Convert.ToDateTime(line[3]);
            }

            ret.ColNameValue.Clear();
            foreach (var col in pjcollist)
            {
                if (colvaluedict.ContainsKey(col.ColumnName))
                {
                    ret.ColNameValue.Add(new KeyValuePair<string, string>(col.ColumnName,colvaluedict[col.ColumnName]));
                }
            }

            ret.PJColList = pjcollist;

            return ret;
        }

        public static List<ProjectVM> RetrieveAllProjectData()
        {
            var allpjcol = ProjectColumn.RetrieveAllPJColumn();
            var allpjname = RetrieveAllProjectName();
            var ret = new List<ProjectVM>();
            foreach (var pjname in allpjname)
            {
                var pjdata = RetriveProjectData(pjname.Key,allpjcol);
                if (pjdata.ColNameValue.Count > 0)
                {
                    ret.Add(pjdata);
                }
            }
            return ret;
        }

    }
}