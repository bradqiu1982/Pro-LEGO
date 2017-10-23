using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProLEGO.Models
{
    public class ProjectLog
    {
        public ProjectLog()
        {
            Machine = "";
            Project = "";
            Event = "";
            PJColumn = "";
            CreateTime = DateTime.Parse("1982-05-06 10:00:00"); ;
        }
        public ProjectLog(string machine, string pro,string col, string evt)
        {
            Machine = machine;
            Project = pro;
            PJColumn = col;
            Event = evt;
            CreateTime = DateTime.Now;
            AddLog();
        }

        public string Machine { set; get; }
        public string Project { set; get; }
        public string PJColumn { set; get; }
        public string Event { set; get; }
        public DateTime CreateTime { set; get; }

        private void AddLog()
        {
            var sql = "insert into ProjectLog(Machine,Project,PJColumn,Event,CreateTime) values(N'<Machine>',N'<Project>',N'<PJColumn>',N'<Event>',N'<CreateTime>')";
            sql = sql.Replace("<Machine>",Machine).Replace("<Project>",Project).Replace("<Event>",Event).Replace("<PJColumn>", PJColumn)
                .Replace("<CreateTime>",CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
            DBUtility.ExeLocalSqlNoRes(sql);

        }

        public static List<ProjectLog> RetrieveAllProject(string pjname)
        {
            var ret = new List<ProjectLog>();

            var sql = "select Machine,Project,PJColumn,Event,CreateTime from ProjectLog where Project = '<Project>' order by CreateTime DESC";
            sql = sql.Replace("<Project>", pjname);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                try
                {
                    var tempvm = new ProjectLog();
                    tempvm.Machine = Convert.ToString(line[0]);
                    tempvm.Project = Convert.ToString(line[1]);
                    tempvm.PJColumn = Convert.ToString(line[2]);
                    tempvm.Event = Convert.ToString(line[3]);
                    tempvm.CreateTime = Convert.ToDateTime(line[4]);
                    ret.Add(tempvm);
                }
                catch (Exception ex) { }
            }

            return ret;
        }


    }
}