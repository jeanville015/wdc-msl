using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Maintenance
{
    public class UomDTO : DocumentAuditDTO
    {
        public int Id { get; set; }
        public string Uom_Name { get; set; }
        public string ActiveFlag { get; set; }

    }
}
