using IDM.Shared;

namespace IDM.Domain.Entities.Maintenance
{
    public class Area : BaseEntity
    {
        public int AreaId { get; set; }
        public string? AreaName { get; set; }
        public int ActiveFlag { get; set; }

    }
}
