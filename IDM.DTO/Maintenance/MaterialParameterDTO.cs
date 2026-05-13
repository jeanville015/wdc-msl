using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Maintenance
{
    public class MaterialParameterDTO : DocumentAuditDTO
    {
        public int Id { get; set; }
        public string Material_No { get; set; }
        public int ParameterId { get; set; }
        public string Parameter_Name { get; set; }
        public int UomId { get; set; }
        public string Uom_Name { get; set; }
        public int SiteId { get; set; }
        public string Site_Name { get; set; }
        public string EdcSpcFlag { get; set; }
        public string LowerSpecsLimit { get; set; }
        public string UpperSpecsLimit { get; set; }
        public string LowerControlLimit { get; set; }
        public string UpperControlLimit { get; set; }
        public string ActiveFlag { get; set; }

    }
}
