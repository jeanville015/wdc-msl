using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model.Main
{
    public class Data :DocumentAudit
    {
        public int? Id { get; set; }
        public string MaterialNumber { get; set; }
        public string MaterialName { get; set; }
        public string LotNumber { get; set; }
        public string DeliveryDate { get; set; }
        public string ReceivedDate { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string Uom { get; set; }
        public string Area { get; set; }
        public string Supplier { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturingDate { get; set; }
        public string ExpirationDate { get; set; }
        public string InspectionDate { get; set; }
        public string InspectedBy { get; set; }
        public string EncodedBy { get; set; }
        public string ReceivedBy { get; set; }
        public string JobNumber { get; set; }
        public string ToolId { get; set; }
        public string Judgement { get; set; }
        public string InspectionJudgement { get; set; }
        public string PreQualificationTest { get; set; }
        public string VisualAppearanceCheck { get; set; }
        public string PackagingDocumentCheck { get; set; }
        public string Remarks { get; set; }
    }
}
