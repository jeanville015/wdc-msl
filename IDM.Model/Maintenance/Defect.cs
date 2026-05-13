namespace IDM.Model.Maintenance
{
    public class Defect : DocumentAudit
    {
        public int DefectId { get; set; }
        public string AnalysisApplicable { get; set; }
        public string DefectType { get; set; }
        public string DefectName { get; set; }
        public string _2KxSem { get; set; }
        public string Talc { get; set; }
        public string OverAllPhysicalDefect { get; set; }
        public string ActiveFlag { get; set; }
    }
}
