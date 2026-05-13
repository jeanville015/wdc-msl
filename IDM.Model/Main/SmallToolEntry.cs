using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Model.Main
{
    public class SmallToolEntry : DocumentAudit
    {
        public int EntryItemsId { get; set; }
        public string Material_No { get; set; }
        public string LotNumber { get; set; }
        public string AmethystJob { get; set; }
        public string ToolName { get; set; }
        public string Analysis { get; set; }
        public int AnalysisTrial { get; set; }
        public string DateAnalyzed { get; set; }
        public string AnalyzedBy { get; set; }
        public string DateReviewed { get; set; }
        public string ReviewedBy { get; set; }
        public string Customer { get; set; }

        public List<ParameterDetail> Parameters { get; set; }
        public List<TrialDetail> Trial { get; set; }
    }
}
