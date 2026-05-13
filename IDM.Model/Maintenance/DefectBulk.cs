using System.Collections.Generic;
using System.Data;

namespace IDM.Model.Maintenance
{
    public class DefectBulk : DocumentAudit
    {
        public List<string> CheckDefectNames { get; set; }
        public List<string> DuplicatedDefectNames { get; set; }
        public List<string> InsertedDefectNames { get; set; } 
        public DataTable DefectDataTable { get; set; }
    }
}
