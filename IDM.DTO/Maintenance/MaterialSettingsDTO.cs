namespace IDM.DTO.Maintenance
{
    public class MaterialSettingsDTO : DocumentAuditDTO
    {
        public int MaterialSettingsId { get; set; }
        public int ToolTypeId { get; set; }
        public string ToolTypeName { get; set; }
        public int MaterialNumberId { get; set; }
        public string MaterialNumber { get; set; }
        public string MaterialName { get; set; }
        public string SettingsName { get; set; }
        public string SettingsValue { get; set; }
        public string ActiveFlag { get; set; }
    }
}
