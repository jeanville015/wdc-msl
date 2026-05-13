using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Main
{
    public class PendingDataDTO : DocumentAuditDTO
    {
        public int? Id { get; set; }
        public string Material_No { get; set; }
        public string Material_Name { get; set; }
        public string LotNumber { get; set; }
        public string Area { get; set; }
        public string Supplier_Name { get; set; }
        public string Manufacturer_Name { get; set; }
        public string Delivery_Date { get; set; }
        public string Received_Date { get; set; }
        public string Manufacturing_Date { get; set; }
        public string Expiration_Date { get; set; }
        public string Inspection_Date { get; set; }
        public string InspectedBy { get; set; }
        public string EncodedBy { get; set; }
        public string Job_Number { get; set; }
        public string ToolId { get; set; }
        public string Pre_Qualification_Test { get; set; }
        public string Visual_Appearance_Check { get; set; }
        public string Packaging_Document_Check { get; set; }
        public string Parameter_Name { get; set; }
        public string Uom_Name { get; set; }
        public string Site_Name { get; set; }
        public string Parameter_Value { get; set; }
        public string Lower_Specs_Limit { get; set; }
        public string Upper_Specs_Limit { get; set; }
        public string Judgement { get; set; }
        public string InspectionValue { get; set; }
        public string LowerControlLimit { get; set; }
        public string UpperControlLimit { get; set; }
        public string Control_Judgement { get; set; }
        public string Remarks { get; set; }
        public string ReceivedBy { get; set; }
        //public int? Id { get; set; }
        //public string MaterialNumber { get; set; }
        //public string MaterialName { get; set; }
        //public string LotNumber { get; set; }
        //public string Area { get; set; }
        //public string Supplier { get; set; }
        //public string Manufacturer { get; set; }
        //public string DeliveryDate { get; set; }
        //public string ReceivedDate { get; set; }
        //public string ManufacturingDate { get; set; }
        //public string ExpirationDate { get; set; }
        //public string InspectionDate { get; set; }
        //public string InspectedBy { get; set; }
        //public string EncodedBy { get; set; }
        //public string JobNumber { get; set; }
        //public string ToolId { get; set; }
        //public string PreQualificationTest { get; set; }
        //public string VisualAppearanceCheck { get; set; }
        //public string PackagingDocumentCheck { get; set; }
        //public string ParameterName { get; set; }
        //public string Uom { get; set; }
        //public string SiteName { get; set; }
        //public string ParameterValue { get; set; }
        //public string LowerSpecsLimit { get; set; }
        //public string UpperSpecsLimit { get; set; }
        //public string Judgement { get; set; }
        //public string InspectionValue { get; set; }
        //public string LowerControlLimit { get; set; }
        //public string UpperControlLimit { get; set; }
        //public string ControlJudgement { get; set; }
        //public string Remarks { get; set; }
        //public string ReceivedBy { get; set; }
    }
}