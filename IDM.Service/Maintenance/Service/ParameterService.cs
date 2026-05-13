using AutoMapper;
using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using IDM.Service.Maintenance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Service
{
    public class ParameterService : IParameterService
    {
        private readonly IParameterRepository _parameterRepository;
        private readonly IMapper _Mapper;

        public ParameterService(IParameterRepository parameterRepository, IMapper mapper)
        {
            _parameterRepository = parameterRepository;
            _Mapper = mapper;
        }

        public async Task<ParameterDTO> GetByIdAsync(int id)
        {
            var parameter = await _parameterRepository.GetByIdAsync(id);
            return _Mapper.Map<ParameterDTO>(parameter);
        }

        public async Task<int> CreateAsync(ParameterDTO parameterDTO)
        {
            var existingParameter = await GetByParameterName(parameterDTO.Parameter_Name);
            if (existingParameter != null)
            {
                return -1;
            }

            var parameter = _Mapper.Map<Parameter>(parameterDTO);
            return await _parameterRepository.CreateAsync(parameter);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _parameterRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ParameterDTO>> GetAllAsync()
        {
            var parameter = await _parameterRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<ParameterDTO>>(parameter);
        }

        public async Task<bool> UpdateAsync(ParameterDTO parameterDTO)
        {
            var existingParameter = await GetByParameterName(parameterDTO.Parameter_Name);

            if (existingParameter != null && existingParameter.Id != parameterDTO.Id)
            {
                return false;
            }

            var parameter = _Mapper.Map<Parameter>(parameterDTO);
            return await _parameterRepository.UpdateAsync(parameter);
        }

        public async Task<ParameterDTO> GetByParameterName(string name)
        {
            var parameter = await _parameterRepository.GetByParameterName(name);
            return _Mapper.Map<ParameterDTO>(parameter);
        }
    }
}
