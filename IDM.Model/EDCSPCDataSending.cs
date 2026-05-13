using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model
{
    public class EDCSPCDataSending
    {
        public EDCSPCDataSending()
        {
            ChartName = new List<string>();
            SourceEntity = new List<string>();
        }
        public List<string> SourceEntity { get; set; }
        public string WaferLot { get; set; }
        public string Product { get; set; }
        public string Operation { get; set; }
        public List<string> ChartName { get; set; }
        public string Operator { get; set; }
        public DateTime TimePrepared { get; set; }
    }
}
