using System.Collections.Generic;
using System.Data;

namespace IDM.DTO.Maintenance
{
    public class DefectBulkDTO : DocumentAuditDTO
    {
        public List<string> CheckDefectNames { get; set; }
        public List<string> DuplicatedDefectNames { get; set; }
        public List<string> InsertedDefectNames { get; set; } 
        public DataTable DefectDataTable { get; set; }
    }
}
