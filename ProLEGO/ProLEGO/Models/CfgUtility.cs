using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProLEGO.Models
{
    public class CfgUtility
    {
        public static Dictionary<string, string> GetSysConfig(Controller ctrl)
        {
            var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/ProLEGOCfg.txt"));
            var ret = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.Contains("##"))
                {
                    continue;
                }

                if (line.Contains(":::"))
                {
                    var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                    if (!ret.ContainsKey(kvpair[0].Trim()))
                    {
                        ret.Add(kvpair[0].Trim(), kvpair[1].Trim());
                    }
                }//end if
            }//end foreach
            return ret;
        }

        public static Dictionary<string, string> GetEmployDict(Controller ctrl)
        {
            var lines = System.IO.File.ReadAllLines(ctrl.Server.MapPath("~/Scripts/ProLEGOCfg.txt"));
            var ret = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (line.Contains("##"))
                {
                    continue;
                }

                if (line.Contains(":::"))
                {
                    var kvpair = line.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);
                    if (!ret.ContainsKey(kvpair[1].Trim().ToUpper()))
                    {
                        ret.Add(kvpair[1].Trim().ToUpper(), kvpair[0].Trim());
                    }
                }//end if
            }//end foreach
            return ret;
        }
    }
}