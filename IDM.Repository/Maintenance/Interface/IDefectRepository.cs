using IDM.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IDefectRepository
    {
        Task<IEnumerable<Defect>> GetAllAsync();
        Task<Defect> GetByIdAsync(int id);
        Task<int> CreateAsync(Defect defect);
        Task<bool> UpdateAsync(Defect defect);
        Task<bool> DeleteAsync(int id);
        Task<Defect> GetByDefectName(string name);
        Task<DefectBulk> GetByDefectBulkNames(DataTable csvData); 
        Task<bool> CreateBulkAsync(DefectBulk defectBulk);
    }
}
