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
    public class MaterialParameterService : IMaterialParameterService
    {
        private readonly IMaterialParameterRepository _materialParameterRepository;
        private readonly IMapper _Mapper;

        public MaterialParameterService(IMaterialParameterRepository materialParameterRepository, IMapper mapper)
        {
            _materialParameterRepository = materialParameterRepository;
            _Mapper = mapper;
        }

        public async Task<IEnumerable<MaterialParameterDTO>> GetByIdAsync(int id)
        {
            var materialParameter = await _materialParameterRepository.GetByIdAsync(id);
            return _Mapper.Map<IEnumerable<MaterialParameterDTO>>(materialParameter);
        }

        public async Task<IEnumerable<MaterialParameterDTO>> GetByMaterialNoAsync(string id)
        {
            var materialParameter = await _materialParameterRepository.GetByMaterialNoAsync(id);
            return _Mapper.Map<IEnumerable<MaterialParameterDTO>>(materialParameter);
        }

        public async Task<int> CreateAsync(MaterialParameterDTO materialParameterDTO)
        {
            var existingMaterialParameter = await GetByParameterAndSite(materialParameterDTO);
            if (existingMaterialParameter != null)
            {
                return -1;
            }

            var materialParameter = _Mapper.Map<MaterialParameter>(materialParameterDTO);
            return await _materialParameterRepository.CreateAsync(materialParameter);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _materialParameterRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MaterialParameterDTO>> GetAllAsync()
        {
            var materialParameter = await _materialParameterRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<MaterialParameterDTO>>(materialParameter);
        }

        public async Task<bool> UpdateAsync(MaterialParameterDTO materialParameterDTO)
        {
            var existingMaterialParameter = await GetByParameterAndSite(materialParameterDTO);

            if (existingMaterialParameter != null && existingMaterialParameter.Id != materialParameterDTO.Id)
            {
                return false;
            }

            var materialParameter = _Mapper.Map<MaterialParameter>(materialParameterDTO);
            return await _materialParameterRepository.UpdateAsync(materialParameter);
        }

        public async Task<MaterialParameterDTO> GetByParameterAndSite(MaterialParameterDTO materialParameterDTO)
        {
            var materialParameter = _Mapper.Map<MaterialParameter>(materialParameterDTO);
            materialParameter = await _materialParameterRepository.GetByParameterAndSite(materialParameter);
            return _Mapper.Map<MaterialParameterDTO>(materialParameter);
        }
    }
}
