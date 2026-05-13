using AutoMapper;
using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using IDM.Repository.Maintenance.Repository;
using IDM.Service.Maintenance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Service
{
    public class MaterialSettingsService : IMaterialSettingsService
    {
        private readonly IMaterialSettingsRepository _materialSettingsRepository;
        private readonly IMapper _Mapper;

        public MaterialSettingsService(IMaterialSettingsRepository materialSettingsRepository, IMapper mapper)
        {
            _materialSettingsRepository = materialSettingsRepository;
            _Mapper = mapper;
        }

        public async Task<MaterialSettingsDTO> GetByIdAsync(int id)
        {
            var materialSettings = await _materialSettingsRepository.GetByIdAsync(id);
            return _Mapper.Map<MaterialSettingsDTO>(materialSettings);
        }

        public async Task<int> CreateAsync(MaterialSettingsDTO materialSettingsDTO)
        {
            var existingMaterialSettings = await GetBySettingsName(materialSettingsDTO.SettingsName, materialSettingsDTO.ToolTypeId, materialSettingsDTO.MaterialNumberId);
            if (existingMaterialSettings != null)
            {
                return -1;
            }

            var materialSettings = _Mapper.Map<MaterialSettings>(materialSettingsDTO);
            return await _materialSettingsRepository.CreateAsync(materialSettings);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _materialSettingsRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MaterialSettingsDTO>> GetAllAsync()
        {
            var materialSettings = await _materialSettingsRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<MaterialSettingsDTO>>(materialSettings);
        }

        public async Task<bool> UpdateAsync(MaterialSettingsDTO materialSettingsDTO)
        {
            var existingMaterialSettings = await GetBySettingsName(materialSettingsDTO.SettingsName, materialSettingsDTO.ToolTypeId, materialSettingsDTO.MaterialNumberId);

            if (existingMaterialSettings != null && existingMaterialSettings.MaterialSettingsId != materialSettingsDTO.MaterialSettingsId)
            {
                return false;
            }

            var materialSettings = _Mapper.Map<MaterialSettings>(materialSettingsDTO);
            return await _materialSettingsRepository.UpdateAsync(materialSettings);
        }

        public async Task<MaterialSettingsDTO> GetBySettingsName(string name, int toolTypeId, int materialId)
        {
            var materialSettings = await _materialSettingsRepository.GetBySettingsName(name, toolTypeId, materialId);
            return _Mapper.Map<MaterialSettingsDTO>(materialSettings);
        }
    }
}
