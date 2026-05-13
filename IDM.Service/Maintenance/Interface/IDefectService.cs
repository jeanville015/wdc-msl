using IDM.DTO.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace IDM.Service.Maintenance.Interface
{
    public interface IDefectService
    {
        Task<IEnumerable<DefectDTO>> GetAllAsync();
        Task<DefectDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(DefectDTO defectDTO);
        Task<bool> UpdateAsync(DefectDTO defectDTO);
        Task<bool> DeleteAsync(int id);
        Task<DefectDTO> GetByDefectName(string name);
        Task<bool> UploadFile(DefectBulkDTO defectBulkDTO);

    }
}
