using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model.Main.Email
{
    public class ApprovalEmailHeaderData
    {
        public string MaterialNumber { get; set; }
        public string MaterialName { get; set; }
        public string LotNumberName { get; set; }
        public string DeliveryDate { get; set; }
        public string ReceivedDate { get; set; }
        public List<ParameterDetails> ParameterDetails { get; set; } = new List<ParameterDetails>();
        public List<ApprovalEmailTabData> ImageTabs { get; set; }
    }

    public class ParameterDetails
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string UomName { get; set; }
        public string UpperSpecsLimit { get; set; }
        public string UpperControlLimit { get; set; }
        public string SiteName { get; set; }
        public string Judgement { get; set; }
        public string ControlJudgement { get; set; }
    }

    public class ApprovalEmailTabData
    {
        public string SampleSequence { get; set; }
        public string Product { get; set; }
        public List<ApprovalEmailDefectData> Defects { get; set; } = new List<ApprovalEmailDefectData>();
    }

    public class ApprovalEmailDefectData
    {
        public string DefectGroup { get; set; }
        public string Defect { get; set; }
        public string BDPSemImagePath { get; set; }
    }
}
