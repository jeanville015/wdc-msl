using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.DTO.Main
{
    public class SmallToolEntryDTO : DocumentAuditDTO
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

        public List<ParameterDetailDTO> Parameters { get; set; }
        public List<TrialDetailDTO> Trial { get; set; }
    }
}
