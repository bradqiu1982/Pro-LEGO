using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProLEGO.Models
{
    public class MachineColumn
    {
        public MachineColumn()
        {
            MachineName = "";
            ColumnID = "";
            ColumnName = "";
        }

        public string MachineName { set; get; }
        public string ColumnID { set; get; }
        public string ColumnName { set; get; }

        public void AddFobiddenColumn()
        {
            var namedict = RetrieveFobiddenColumnName(MachineName);
            if (namedict.ContainsKey(ColumnName)) return;

            var sql = "insert into MachineColumn(MachineName,ColumnID,ColumnName) values('<MachineName>','<ColumnID>','<ColumnName>')";
            sql = sql.Replace("<ColumnID>", ColumnID).Replace("<ColumnName>", ColumnName)
                .Replace("<MachineName>", MachineName);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void RemoveFobiddenColumn(string MachineName, string ColumnName)
        {
            var sql = "delete from MachineColumn where MachineName = '<MachineName>' and ColumnName = '<ColumnName>'";
            sql = sql.Replace("<ColumnName>", ColumnName).Replace("<MachineName>", MachineName);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static Dictionary<string,bool> RetrieveFobiddenColumnName(string MachineName)
        {
            var ret = new Dictionary<string, bool>();
            var sql = "select ColumnName,ColumnID from MachineColumn where  MachineName = '<MachineName>'";
            sql = sql.Replace("<MachineName>", MachineName);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                if (!ret.ContainsKey(Convert.ToString(line[0])))
                {
                    ret.Add(Convert.ToString(line[0]), true);
                }
            }
            return ret;
        }

        //public static Dictionary<string, bool> RetrieveFobiddenColumnID(string MachineName)
        //{
        //    var ret = new Dictionary<string, bool>();
        //    var sql = "select ColumnName,ColumnID from MachineColumn where  MachineName = '<MachineName>'";
        //    sql = sql.Replace("<MachineName>", MachineName);
        //    var dbret = DBUtility.ExeLocalSqlWithRes(sql);
        //    foreach (var line in dbret)
        //    {
        //        if (!ret.ContainsKey(Convert.ToString(line[1])))
        //        {
        //            ret.Add(Convert.ToString(line[1]), true);
        //        }
        //    }
        //    return ret;
        //}


    }
}