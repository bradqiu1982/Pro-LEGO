using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProLEGO.Models
{
    public class PROJECTCOLUMNTYPE
    {
        public static string ROLE = "ROLE";
        public static string DATE = "DATE";
        public static string BOOL = "BOOL";
        public static string INFORMATION = "INFORMATION";
    }

    public class ProjectColumn
    {
        public ProjectColumn()
        {
            ColumnID = "";
            ColumnName = "";
            ColumnType = "";
            ColumnDefaultVal = "";
            ColumnCreateDate = DateTime.Parse("1982-05-06 10:00:00");
            Removed = "";
            AdditionDefault = new List<string>();
        }

        public string ColumnID { set; get; }
        public string ColumnName { set; get; }
        public string ColumnType { set; get; }
        public string ColumnDefaultVal { set; get; }
        public DateTime ColumnCreateDate { set; get; }
        public string Removed { set; get; }
        public List<string> AdditionDefault { set; get; }

        public string ColumnDefaultValList {
            get {
                if (!string.IsNullOrEmpty(ColumnDefaultVal))
                {
                    var splitstrs = ColumnDefaultVal.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (AdditionDefault.Count > 0)
                    {
                        var combinelist = new List<string>();
                        combinelist.AddRange(splitstrs);
                        combinelist.AddRange(AdditionDefault);
                        return JsonConvert.SerializeObject(combinelist.ToArray());
                    }

                    return JsonConvert.SerializeObject(splitstrs);
                }
                else
                {
                    if (AdditionDefault.Count > 0)
                    {
                        return JsonConvert.SerializeObject(AdditionDefault.ToArray());
                    }
                    return JsonConvert.SerializeObject(new string[] { });
                }

                //var ret = new List<string>();
                //if(!string.IsNullOrEmpty(ColumnDefaultVal))
                //{
                //    var splitstrs = ColumnDefaultVal.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                //    foreach (var item in splitstrs)
                //    {
                //        ret.Add(item.Trim());
                //    }
                //}
                //return ret;
            }
        }

        public static Dictionary<string, bool> ExistPJColumn()
        {
            var ret = new Dictionary<string, bool>();
            var pjcolist = RetrieveAllPJColumn();
            foreach (var pjc in pjcolist)
            {
                ret.Add(pjc.ColumnName, true);
            }
            return ret;
        }

        public bool AddPJColumn(string machine)
        {
            var existcols = ExistPJColumn();
            if (existcols.ContainsKey(ColumnName))
                return false;

            var newkey = ProjectVM.GetUniqKey();
            var sql = "insert into ProjectColumn(ColumnID,ColumnName,ColumnType,ColumnDefaultVal,ColumnCreateDate) values(N'<ColumnID>',N'<ColumnName>',N'<ColumnType>',N'<ColumnDefaultVal>',N'<ColumnCreateDate>')";
            sql = sql.Replace("<ColumnID>", newkey).Replace("<ColumnCreateDate>", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                .Replace("<ColumnName>", ColumnName).Replace("<ColumnType>", ColumnType).Replace("<ColumnDefaultVal>", ColumnDefaultVal);
            var ret = DBUtility.ExeLocalSqlNoRes(sql);
            if (ret)
            {
                ProjectVM.AddNewColumn(ColumnName, newkey);
            }

            new ProjectLog(machine, "ALLPJ", "ALLCOL", "Add Column: " + ColumnName);

            return ret;
        }

        public static List<ProjectColumn> RetrieveAllPJColumn()
        {
            var ret = new List<ProjectColumn>();
            var sql = "select ColumnID,ColumnName,ColumnType,ColumnDefaultVal,Removed from ProjectColumn order by ColumnCreateDate ASC";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempvm = new ProjectColumn();
                tempvm.ColumnID = Convert.ToString(line[0]);
                tempvm.ColumnName = Convert.ToString(line[1]);
                tempvm.ColumnType = Convert.ToString(line[2]);
                tempvm.ColumnDefaultVal = Convert.ToString(line[3]);
                tempvm.Removed = Convert.ToString(line[4]);
                if (string.Compare(tempvm.ColumnType, PROJECTCOLUMNTYPE.ROLE, true) == 0)
                {
                    tempvm.AdditionDefault.Clear();
                    tempvm.AdditionDefault.AddRange(RetrieveRoleAdditionalDefault());
                }
                ret.Add(tempvm);
            }
            return ret;
        }

        public static List<ProjectColumn> RetrieveAllPJColumnWithoutRemoved()
        {
            var ret = new List<ProjectColumn>();
            var sql = "select ColumnID,ColumnName,ColumnType,ColumnDefaultVal,Removed from ProjectColumn order by ColumnCreateDate ASC";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempvm = new ProjectColumn();
                tempvm.ColumnID = Convert.ToString(line[0]);
                tempvm.ColumnName = Convert.ToString(line[1]);
                tempvm.ColumnType = Convert.ToString(line[2]);
                tempvm.ColumnDefaultVal = Convert.ToString(line[3]);
                tempvm.Removed = Convert.ToString(line[4]);
                if (string.Compare(tempvm.Removed, "true", true) == 0)
                {
                    continue;
                }

                if (string.Compare(tempvm.ColumnType, PROJECTCOLUMNTYPE.ROLE, true) == 0)
                {
                    tempvm.AdditionDefault.Clear();
                    tempvm.AdditionDefault.AddRange(RetrieveRoleAdditionalDefault());
                }
                ret.Add(tempvm);
            }
            return ret;
        }

        public static List<ProjectColumn> RetrievePJColumnByName(string ColumnName)
        {
            var ret = new List<ProjectColumn>();
            var sql = "select ColumnID,ColumnName,ColumnType,ColumnDefaultVal from ProjectColumn where ColumnName = N'<ColumnName>'";
            sql = sql.Replace("<ColumnName>", ColumnName);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempvm = new ProjectColumn();
                tempvm.ColumnID = Convert.ToString(line[0]);
                tempvm.ColumnName = Convert.ToString(line[1]);
                tempvm.ColumnType = Convert.ToString(line[2]);
                tempvm.ColumnDefaultVal = Convert.ToString(line[3]);
                ret.Add(tempvm);
            }
            return ret;
        }

        public static void UpdatePJColDefaultVal(string ColumnName, string ColumnDefaultVal)
        {
            var sql = "update ProjectColumn set ColumnDefaultVal = N'<ColumnDefaultVal>' where ColumnName = N'<ColumnName>'";
            sql = sql.Replace("<ColumnName>", ColumnName).Replace("<ColumnDefaultVal>", ColumnDefaultVal);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        //TRUE or FALSE
        public static void UpdatePJColRemoved(string ColumnName, bool Removed)
        {
            var sql = "update ProjectColumn set Removed = N'<Removed>' where ColumnName = N'<ColumnName>'";
            sql = sql.Replace("<ColumnName>", ColumnName).Replace("<Removed>", Removed?"TRUE":"FALSE");
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static Dictionary<string, bool> RetrieveAllPJColumnDict()
        {
            var dict = new Dictionary<string, bool>();
            var allcol = RetrieveAllPJColumn();
            foreach (var col in allcol)
            {
                dict.Add(col.ColumnName, true);
            }
            return dict;
        }

        public static List<ProjectColumn> RetriveRolePJColumn()
        {
            var ret = new List<ProjectColumn>();
            var sql = "select ColumnID,ColumnName,ColumnType,ColumnDefaultVal from ProjectColumn where ColumnType = N'<ColumnType>' order by ColumnCreateDate ASC";
            sql = sql.Replace("<ColumnType>", PROJECTCOLUMNTYPE.ROLE);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempvm = new ProjectColumn();
                tempvm.ColumnID = Convert.ToString(line[0]);
                tempvm.ColumnName = Convert.ToString(line[1]);
                tempvm.ColumnType = Convert.ToString(line[2]);
                tempvm.ColumnDefaultVal = Convert.ToString(line[3]);
                ret.Add(tempvm);
            }
            return ret;
        }

        public static List<string> RetrieveRoleAdditionalDefault()
        {
            var sql = "select UserName from UserTable";
            var dbret = DBUtility.ExeTraceSqlWithRes(sql);
            var ret = new List<string>();

            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]).Replace("@FINISAR.COM",""));
            }
            ret.Sort();
            return ret;
        }

    }
}