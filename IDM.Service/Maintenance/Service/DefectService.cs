using AutoMapper;
using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using IDM.Service.Maintenance.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Service
{
    public class DefectService : IDefectService
    {
        private readonly IDefectRepository _defectRepository;
        private readonly IMapper _Mapper;

        public DefectService(IDefectRepository defectRepository, IMapper mapper)
        {
            _defectRepository = defectRepository;
            _Mapper = mapper;
        }

        public async Task<DefectDTO> GetByIdAsync(int id)
        {
            var defect = await _defectRepository.GetByIdAsync(id);
            return _Mapper.Map<DefectDTO>(defect);
        }

        public async Task<int> CreateAsync(DefectDTO defectDTO)
        {
            var existingDefect = await GetByDefectName(defectDTO.DefectName);
            if (existingDefect != null)
            {
                return -1;
            }

            var uom = _Mapper.Map<Defect>(defectDTO);
            return await _defectRepository.CreateAsync(uom);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _defectRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DefectDTO>> GetAllAsync()
        {
            var uom = await _defectRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<DefectDTO>>(uom);
        }

        public async Task<bool> UpdateAsync(DefectDTO defectDTO)
        {
            var existingDefect = await GetByDefectName(defectDTO.DefectName);

            if (existingDefect != null && existingDefect.DefectId != defectDTO.DefectId)
            {
                return false;
            }

            var defect = _Mapper.Map<Defect>(defectDTO);
            return await _defectRepository.UpdateAsync(defect);
        }

        public async Task<DefectDTO> GetByDefectName(string name)
        {
            var defect = await _defectRepository.GetByDefectName(name);
            return _Mapper.Map<DefectDTO>(defect);
        }

        public async Task<DefectBulkDTO> GetByDefectBulkNames(DataTable csvData)
        {
            var defectBulk = await _defectRepository.GetByDefectBulkNames(csvData);
            return _Mapper.Map<DefectBulkDTO>(defectBulk);
        }

        public async Task<bool> UploadFile(DefectBulkDTO defectBulkDTO)
        {
            var existingDefect = await GetByDefectBulkNames(defectBulkDTO.DefectDataTable);
            if (existingDefect.DuplicatedDefectNames.Count > 0)
            {
                return false;
            }

            var uom = _Mapper.Map<DefectBulk>(defectBulkDTO);
            return await _defectRepository.CreateBulkAsync(uom);
        }
    }
}
