using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Main
{
    public class StagingDTO : DocumentAuditDTO
    {
        public string AmethystJob { get; set; }
        public string Analysis { get; set; }
        public string MaterialNumber { get; set; }
        public string LotNumber { get; set; }
        public string DateAnalyzed { get; set; }
        public string AnalyzedBy { get; set; }
        public string DateReviewed { get; set; }
        public string ReviewedBy { get; set; }
        public string Tool { get; set; }
        public string ParmName { get; set; }
        public string Trial1 { get; set; }
        public string Trial2 { get; set; }
        public string Trial3 { get; set; }
        public string Trial4 { get; set; }
        public string Trial5 { get; set; }
        public string Trial6 { get; set; }
        public string Trial7 { get; set; }
        public string Trial8 { get; set; }
        public string Trial9 { get; set; }
        public string Trial10 { get; set; }
        public string Average { get; set; }
        public string UpperLimit { get; set; }
        public string LowerLimit { get; set; }
        public string Judgement { get; set; }
    }
}
