using IDM.Model.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.Maintenance.Interface
{
    public interface IParameterRepository
    {
        Task<IEnumerable<Parameter>> GetAllAsync();
        Task<Parameter> GetByIdAsync(int id);
        Task<int> CreateAsync(Parameter parameter);
        Task<bool> UpdateAsync(Parameter parameter);
        Task<bool> DeleteAsync(int id);
        Task<Parameter> GetByParameterName(string name);
    }
}
