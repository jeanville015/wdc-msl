namespace IDM.Model.Maintenance
{
    public class Analysis : DocumentAudit
    {
        public int AnalysisId { get; set; }
        public int ToolTypeId { get; set; }
        public string ToolTypeName { get; set; }
        public string AnalysisName { get; set; }
        public string SourceTable { get; set; }
        public string DestinationTable { get; set; }
        public string ActiveFlag { get; set; }
    }
}
