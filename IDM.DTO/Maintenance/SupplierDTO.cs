using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Maintenance
{
    public class SupplierDTO : DocumentAuditDTO
    {
        public int Id { get; set; }
        public string Supplier_Name { get; set; }
        public string ActiveFlag { get; set; }

    }
}
