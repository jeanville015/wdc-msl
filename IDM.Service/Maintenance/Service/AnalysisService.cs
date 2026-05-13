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
    public class AnalysisService : IAnalysisService
    {
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IMapper _Mapper;

        public AnalysisService(IAnalysisRepository analysisRepository, IMapper mapper)
        {
            _analysisRepository = analysisRepository;
            _Mapper = mapper;
        }

        public async Task<AnalysisDTO> GetByIdAsync(int id)
        {
            var analysis = await _analysisRepository.GetByIdAsync(id);
            return _Mapper.Map<AnalysisDTO>(analysis);
        }

        public async Task<AnalysisDTO> GetByToolTypeAndAnalysisAsync(int toolTypeId, string analysisName)
        {
            var analysis = await _analysisRepository.GetByToolTypeAndAnalysisAsync(toolTypeId, analysisName);
            return _Mapper.Map<AnalysisDTO>(analysis);
        }

        public async Task<AnalysisDTO> GetByAnalysisAsync(string analysisName)
        {
            var analysis = await _analysisRepository.GetByAnalysisAsync(analysisName);
            return _Mapper.Map<AnalysisDTO>(analysis);
        }

        public async Task<int> CreateAsync(AnalysisDTO analysisDTO)
        {
            var existingAnalysis = await GetByAnalysisName(analysisDTO.AnalysisName, analysisDTO.ToolTypeId);
            if (existingAnalysis != null)
            {
                return -1;
            }

            var analysis = _Mapper.Map<Analysis>(analysisDTO);
            return await _analysisRepository.CreateAsync(analysis);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _analysisRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AnalysisDTO>> GetAllAsync()
        {
            var analysis = await _analysisRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<AnalysisDTO>>(analysis);
        }

        public async Task<bool> UpdateAsync(AnalysisDTO analysisDTO)
        {
            var existingAnalysis = await GetByAnalysisName(analysisDTO.AnalysisName, analysisDTO.ToolTypeId);

            if (existingAnalysis != null && existingAnalysis.AnalysisId != analysisDTO.AnalysisId)
            {
                return false;
            }

            var analysis = _Mapper.Map<Analysis>(analysisDTO);
            return await _analysisRepository.UpdateAsync(analysis);
        }

        public async Task<AnalysisDTO> GetByAnalysisName(string name, int toolTypeId)
        {
            var analysis = await _analysisRepository.GetByAnalysisName(name, toolTypeId);
            return _Mapper.Map<AnalysisDTO>(analysis);
        }
    }
}
