namespace IDM.Model.Maintenance
{
    public class ToolType : DocumentAudit
    {
        public int ToolTypeId { get; set; }
        public string ToolTypeName { get; set; }
        public string RequireApproval { get; set; }
        public string ActiveFlag { get; set; }
    }
}
