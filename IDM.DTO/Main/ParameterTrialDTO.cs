using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Main
{
    public class ParameterTrialDTO : DocumentAuditDTO
    {
        public string Material_No { get; set; }
        public string DeliveryDate { get; set; }
        public string ReceivedDate { get; set; }
        public string LotNumber { get; set; }
        public string Job_Number { get; set; }
        public string ToolId { get; set; }
        public string Trial_Counter { get; set; }
        public string Trial_Value { get; set; }
        public string Parameter_Name { get; set; }
        public string Site_Name { get; set; }
        public string ActiveFlag { get; set; }
    }
}
