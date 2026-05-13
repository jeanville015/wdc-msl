using IDM.DTO.Main;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace IDM.DTO.Main.View
{
    public class DynamicStagingTabsView
    {
        // Core properties that exist in all tables
        // The Static Headers (Grouped at the top)
        public string AmethystJob { get; set; }
        public string Analysis { get; set; }
        public string AnalysisTrial { get; set; }
        public string LotNumber { get; set; }
        public string DateAnalyzed { get; set; }
        public string AnalyzedBy { get; set; }
        public string DateReviewed { get; set; }
        public string ReviewedBy { get; set; }
        public string Tool { get; set; }

        // Dynamic headers for the table
        public List<string> DynamicHeaders { get; set; }

        // Grouped data for the Tabs
        public Dictionary<string, List<DynamicStagingDTO>> GroupedData { get; set; }
    }
}
