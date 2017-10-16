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
        }

        public string ColumnID { set; get; }
        public string ColumnName { set; get; }
        public string ColumnType { set; get; }
        public string ColumnDefaultVal { set; get; }
        public DateTime ColumnCreateDate { set; get; }

        public List<string> ColumnDefaultValList {
            get {
                var ret = new List<string>();
                if(string.IsNullOrEmpty(ColumnDefaultVal))
                {
                    var splitstrs = ColumnDefaultVal.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in splitstrs)
                    {
                        ret.Add(item.Trim());
                    }
                }
                return ret;
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
            var sql = "insert into ProjectColumn(ColumnID,ColumnName,ColumnType,ColumnDefaultVal,ColumnCreateDate) values('<ColumnID>','<ColumnName>','<ColumnType>','<ColumnDefaultVal>','<ColumnCreateDate>')";
            sql = sql.Replace("<ColumnID>", newkey).Replace("<ColumnCreateDate>", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"))
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
            var sql = "select ColumnID,ColumnName,ColumnType,ColumnDefaultVal from ProjectColumn order by ColumnCreateDate ASC";
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
            var sql = "select ColumnID,ColumnName,ColumnType,ColumnDefaultVal from ProjectColumn where ColumnType = '<ColumnType>' order by ColumnCreateDate ASC";
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

    }
}