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
            ProjectKey = "";
            ProjectName = "";
            PJColList = new List<ProjectColumn>();
            ColNameValue = new Dictionary<string, string>();
        }

        public string ProjectKey { set; get; }
        public string ProjectName { set; get; }
        public List<ProjectColumn> PJColList { set; get; }
        public Dictionary<string, string> ColNameValue { set; get; }

        public static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

    }
}