using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model.Maintenance
{
    public class Manufacturer : DocumentAudit
    {
        public int Id { get; set; }
        public string Manufacturer_Name { get; set; }
        public string ActiveFlag { get; set; }
    }
}
