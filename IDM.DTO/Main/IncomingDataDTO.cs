using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Main
{
    public class IncomingDataDTO : DocumentAuditDTO
    {
        public int? Id { get; set; }
        public string Material_No { get; set; }
        public string Material_Name { get; set; }
        public string LotNumber { get; set; }
        public string Job_Number { get; set; }
        public string ToolId { get; set; }
        public string Area_Name { get; set; }
        public string Supplier_Name { get; set; }
        public string Manufacturer_Name { get; set; }
        public string Delivery_Date { get; set; }
        public string Received_Date { get; set; }
        public string Manufacturing_Date { get; set; }
        public string Expiration_Date { get; set; }
        public string Inspection_Date { get; set; }
        public string InspectedBy { get; set; }
        public string EncodedBy { get; set; }
        public string ReceivedBy { get; set; }
        public string Pre_Qualification_Test { get; set; }
        public string View_Appearance_Check { get; set; }
        public string Packaging_Document_Check { get; set; }
        public string Remarks { get; set; }
        public string ActiveFlag { get; set; }

        public List<ParameterDetailDTO> Parameters { get; set; }
        public List<TrialDetailDTO> Trial { get; set; }
    }

    public class ParameterDetailDTO
    {
        public string Parameter_Name { get; set; }
        public string Parameter_Value { get; set; }
        public string Uom_Name { get; set; }
        public string Site_Name { get; set; }
        public string Lower_Specs_Limit { get; set; }
        public string Upper_Specs_Limit { get; set; }
        public string Specs_Judgement { get; set; }
        public string Lower_Control_Limit { get; set; }
        public string Upper_Control_Limit { get; set; }
        public string Control_Judgement { get; set; }
        public string EdcSpcFlag { get; set; }
    }

    public class TrialDetailDTO
    {
        public string Trial_Counter { get; set; }
        public string Trial_Value { get; set; }
        public string Parameter_Name { get; set; }
        public string Site_Name { get; set; }
    }
}
