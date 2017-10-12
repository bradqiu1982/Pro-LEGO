using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProLEGO.Models;

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
        }

        public ActionResult MainPage()
        {
            UserAuth();
            return View();
        }

    }
}