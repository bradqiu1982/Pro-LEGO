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
    }
}