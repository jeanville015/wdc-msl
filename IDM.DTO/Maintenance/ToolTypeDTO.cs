namespace IDM.DTO.Maintenance
{
    public class ToolTypeDTO: DocumentAuditDTO
    {
        public int ToolTypeId { get; set; }
        public string ToolTypeName { get; set; }
        public string RequireApproval { get; set; }
        public string ActiveFlag { get; set; }
    }
}
