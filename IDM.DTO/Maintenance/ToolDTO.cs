namespace IDM.DTO.Maintenance
{
    public class ToolDTO : DocumentAuditDTO
    {
        public int ToolId { get; set; }
        public string ToolName { get; set; }
        public int ToolTypeId { get; set; }
        public string ToolTypeName { get; set; }
        public string ActiveFlag { get; set; }
    }
}
