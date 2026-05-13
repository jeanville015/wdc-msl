using IDM.DTO.Main;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace IDM.DTO.Main.View
{
    public class DynamicStagingDetailsView
    {
        // Core properties that exist in all tables
        // The Static Headers (Grouped at the top)
        public string AmethystJob { get; set; }
        public string Analysis { get; set; }
        public string AnalysisTrial { get; set; } 
        public string Area { get; set; }  
        public string SubArea { get; set; }  

        // Dynamic headers for the table
        public List<string> DynamicHeaders { get; set; }

        // Grouped data for the Tabs
        public List<DynamicStagingDTO> Data { get; set; }
    }
}
