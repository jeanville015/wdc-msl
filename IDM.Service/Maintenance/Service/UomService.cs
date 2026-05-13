
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
    public class UomService : IUomService
    {
        private readonly IUomRepository _uomRepository;
        private readonly IMapper _Mapper;

        public UomService(IUomRepository uomRepository, IMapper mapper)
        {
            _uomRepository = uomRepository;
            _Mapper = mapper;
        }

        public async Task<UomDTO> GetByIdAsync(int id)
        {
            var uom = await _uomRepository.GetByIdAsync(id);
            return _Mapper.Map<UomDTO>(uom);
        }

        public async Task<int> CreateAsync(UomDTO uomDTO)
        {
            var existingUom = await GetByUomName(uomDTO.Uom_Name);
            if (existingUom != null)
            {
                return -1;
            }

            var uom = _Mapper.Map<Uom>(uomDTO);
            return await _uomRepository.CreateAsync(uom);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _uomRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<UomDTO>> GetAllAsync()
        {
            var uom = await _uomRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<UomDTO>>(uom);
        }

        public async Task<bool> UpdateAsync(UomDTO uomDTO)
        {
            var existingUom = await GetByUomName(uomDTO.Uom_Name);

            if (existingUom != null && existingUom.Id != uomDTO.Id)
            {
                return false;
            }

            var uom = _Mapper.Map<Uom>(uomDTO);
            return await _uomRepository.UpdateAsync(uom);
        }

        public async Task<UomDTO> GetByUomName(string name)
        {
            var uom = await _uomRepository.GetByUomName(name);
            return _Mapper.Map<UomDTO>(uom);
        }
    }
}
