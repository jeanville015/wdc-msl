namespace IDM.DTO.Main
{
    public class ToolEntryItemsDTO : DocumentAuditDTO
    {
        public int EntryItemsId { get; set; }
        public string AmethystJob { get; set; }
        public string Analysis { get; set; }
        public string AnalysisTrial { get; set; }
        public string ToolName { get; set; }
        public string DateAnalyzed { get; set; }
        public string AnalyzedBy { get; set; }
        public string DateReviewed { get; set; }
        public string ReviewedBy { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
    }
}
