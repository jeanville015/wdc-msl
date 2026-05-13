using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WDHelpers.EDCSPCHelper;

namespace IDM.Web.DataAccessModel
{
    public class DataModel
    {
        public DataTable Data { get; set; }
        public DateTime TestDate { get; set; }
        public string OperatorID { get; set; }
        public string WaferID { get; set; }
        public string LotID { get; set; }
        public string ToolEntity { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<Tag> Tags { get; set; }
        public string Product { get; set; }
    }
}