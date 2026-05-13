using IDM.Shared;

namespace IDM.Application.DTOs.Maintenance
{
    public class AreaDTO : BaseEntity
    {
        public int AreaId { get; set; }
        public string? AreaName { get; set; }
        public int ActiveFlag { get; set; }
    }
}
