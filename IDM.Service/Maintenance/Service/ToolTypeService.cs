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
    public class ToolTypeService : IToolTypeService
    {
        private readonly IToolTypeRepository _toolTypeRepository;
        private readonly IMapper _Mapper;

        public ToolTypeService(IToolTypeRepository toolTypeRepository, IMapper mapper)
        {
            _toolTypeRepository = toolTypeRepository;
            _Mapper = mapper;
        }

        public async Task<ToolTypeDTO> GetByIdAsync(int id)
        {
            var tool = await _toolTypeRepository.GetByIdAsync(id);
            return _Mapper.Map<ToolTypeDTO>(tool);
        }

        public async Task<int> CreateAsync(ToolTypeDTO toolTypeDTO)
        {
            var existingToolType = await GetByToolTypeName(toolTypeDTO.ToolTypeName);
            if (existingToolType != null)
            {
                return -1;
            }

            var toolType = _Mapper.Map<ToolType>(toolTypeDTO);
            return await _toolTypeRepository.CreateAsync(toolType);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _toolTypeRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ToolTypeDTO>> GetAllAsync()
        {
            var toolType = await _toolTypeRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<ToolTypeDTO>>(toolType);
        }

        public async Task<bool> UpdateAsync(ToolTypeDTO toolTypeDTO)
        {
            var existingToolType = await GetByToolTypeName(toolTypeDTO.ToolTypeName);

            if (existingToolType != null && existingToolType.ToolTypeId != toolTypeDTO.ToolTypeId)
            {
                return false;
            }

            var toolType = _Mapper.Map<ToolType>(toolTypeDTO);
            return await _toolTypeRepository.UpdateAsync(toolType);
        }

        public async Task<ToolTypeDTO> GetByToolTypeName(string name)
        {
            var toolType = await _toolTypeRepository.GetByToolTypeName(name);
            return _Mapper.Map<ToolTypeDTO>(toolType);
        }
    }
}
