using AutoMapper;
using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;
using IDM.Repository.Maintenance.Interface;
using IDM.Service.Maintenance.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDM.Service.Maintenance.Service
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _areaRepository;
        private readonly IMapper _Mapper;

        public AreaService(IAreaRepository areaRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _Mapper = mapper;
        }

        public async Task<AreaDTO> GetByIdAsync(int id)
        {
            var area = await _areaRepository.GetByIdAsync(id);
            return _Mapper.Map<AreaDTO>(area);
        }

        public async Task<int> CreateAsync(AreaDTO areaDTO)
        {
            var existingArea = await GetByAreaName(areaDTO.Area_Name);
            if (existingArea != null)
            {
                return -1;
            }

            var area = _Mapper.Map<Area>(areaDTO);
            return await _areaRepository.CreateAsync(area);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _areaRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AreaDTO>> GetAllAsync()
        {
            var area = await _areaRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<AreaDTO>>(area);
        }

        public async Task<bool> UpdateAsync(AreaDTO areaDTO)
        {
            var existingArea = await GetByAreaName(areaDTO.Area_Name);

            if (existingArea != null && existingArea.Id != areaDTO.Id)
            {
                return false;
            }

            var area = _Mapper.Map<Area>(areaDTO);
            return await _areaRepository.UpdateAsync(area);
        }

        public async Task<AreaDTO> GetByAreaName(string name)
        {
            var area = await _areaRepository.GetByAreaName(name);
            return _Mapper.Map<AreaDTO>(area);
        }
    }
}
