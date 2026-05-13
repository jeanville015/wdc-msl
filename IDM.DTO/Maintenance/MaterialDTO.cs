using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Maintenance
{
    public class MaterialDTO : DocumentAuditDTO
    {
        public int Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_No { get; set; }
        public int AreaId { get; set; }
        public string Area_Name { get; set; }
        public int ManufacturerId { get; set; }
        public string Manufacturer_Name { get; set; }
        public int SupplierId { get; set; }
        public string Supplier_Name { get; set; }
        public string ActiveFlag { get; set; }
    }
}
