using IDM.Domain.Entities.Maintenance;

namespace IDM.Infrastructure.Repositories.Interfaces
{
    public interface IAreaRepository
    {
        Task<IEnumerable<Area>> GetAllAsync();
        Task<Area> GetByIdAsync(int id);
        Task<int> CreateAsync(Area area);
        Task<bool> UpdateAsync(Area area);
        Task<bool> DeleteAsync(int id);
        Task<Area> GetByAreaName(string name);
    }
}
