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
    }
}