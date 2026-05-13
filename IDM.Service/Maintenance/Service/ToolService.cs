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
    public class ToolService : IToolService
    {
        private readonly IToolRepository _toolRepository;
        private readonly IMapper _Mapper;

        public ToolService(IToolRepository toolRepository, IMapper mapper)
        {
            _toolRepository = toolRepository;
            _Mapper = mapper;
        }

        public async Task<ToolDTO> GetByIdAsync(int id)
        {
            var tool = await _toolRepository.GetByIdAsync(id);
            return _Mapper.Map<ToolDTO>(tool);
        }

        public async Task<ToolDTO> GetByToolNameAsync(string toolName)
        {
            var tool = await _toolRepository.GetByToolNameAsync(toolName);
            return _Mapper.Map<ToolDTO>(tool);
        }

        public async Task<int> CreateAsync(ToolDTO toolDTO)
        {
            var existingTool = await GetByToolName(toolDTO.ToolName, toolDTO.ToolTypeId);
            if (existingTool != null)
            {
                return -1;
            }

            var tool = _Mapper.Map<Tool>(toolDTO);
            return await _toolRepository.CreateAsync(tool);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _toolRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ToolDTO>> GetAllAsync()
        {
            var tool = await _toolRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<ToolDTO>>(tool);
        }

        public async Task<bool> UpdateAsync(ToolDTO toolDTO)
        {
            var existingTool = await GetByToolName(toolDTO.ToolName, toolDTO.ToolTypeId);

            if (existingTool != null && existingTool.ToolId != toolDTO.ToolId)
            {
                return false;
            }

            var tool = _Mapper.Map<Tool>(toolDTO);
            return await _toolRepository.UpdateAsync(tool);
        }

        public async Task<ToolDTO> GetByToolName(string name, int toolTypeId)
        {
            var tool = await _toolRepository.GetByToolName(name, toolTypeId);
            return _Mapper.Map<ToolDTO>(tool);
        }
    }
}
