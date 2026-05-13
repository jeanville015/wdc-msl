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
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly IMapper _Mapper;

        public MaterialService(IMaterialRepository materialRepository, IMapper mapper)
        {
            _materialRepository = materialRepository;
            _Mapper = mapper;
        }

        public async Task<MaterialDTO> GetByIdAsync(int id)
        {
            var material = await _materialRepository.GetByIdAsync(id);
            return _Mapper.Map<MaterialDTO>(material);
        }

        public async Task<int> CreateAsync(MaterialDTO materialDTO)
        {
            var existingMaterial = await GetByMaterialName(materialDTO.Material_No);
            if (existingMaterial != null)
            {
                return -1;
            }

            var material = _Mapper.Map<Material>(materialDTO);
            return await _materialRepository.CreateAsync(material);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _materialRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MaterialDTO>> GetAllAsync()
        {
            var material = await _materialRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<MaterialDTO>>(material);
        }

        public async Task<bool> UpdateAsync(MaterialDTO materialDTO)
        {
            var existingMaterial = await GetByMaterialName(materialDTO.Material_No);

            if (existingMaterial != null && existingMaterial.Id != materialDTO.Id)
            {
                return false;
            }

            var material = _Mapper.Map<Material>(materialDTO);
            return await _materialRepository.UpdateAsync(material);
        }

        public async Task<MaterialDTO> GetByMaterialName(string name)
        {
            var material = await _materialRepository.GetByMaterialName(name);
            return _Mapper.Map<MaterialDTO>(material);
        }
    }
}
