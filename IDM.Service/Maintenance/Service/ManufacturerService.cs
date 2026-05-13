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
    public class ManufacturerService : IManufacturerService
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _Mapper;

        public ManufacturerService(IManufacturerRepository manufacturerRepository, IMapper mapper)
        {
            _manufacturerRepository = manufacturerRepository;
            _Mapper = mapper;
        }

        public async Task<ManufacturerDTO> GetByIdAsync(int id)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
            return _Mapper.Map<ManufacturerDTO>(manufacturer);
        }

        public async Task<int> CreateAsync(ManufacturerDTO manufacturerDTO)
        {
            var existingManufacturer = await GetByManufacturerName(manufacturerDTO.Manufacturer_Name);
            if (existingManufacturer != null)
            {
                return -1;
            }

            var manufacturer = _Mapper.Map<Manufacturer>(manufacturerDTO);
            return await _manufacturerRepository.CreateAsync(manufacturer);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _manufacturerRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ManufacturerDTO>> GetAllAsync()
        {
            var manufacturer = await _manufacturerRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<ManufacturerDTO>>(manufacturer);
        }

        public async Task<bool> UpdateAsync(ManufacturerDTO manufacturerDTO)
        {
            var existingManufacturer = await GetByManufacturerName(manufacturerDTO.Manufacturer_Name);

            if (existingManufacturer != null && existingManufacturer.Id != manufacturerDTO.Id)
            {
                return false;
            }

            var manufacturer = _Mapper.Map<Manufacturer>(manufacturerDTO);
            return await _manufacturerRepository.UpdateAsync(manufacturer);
        }

        public async Task<ManufacturerDTO> GetByManufacturerName(string name)
        {
            var manufacturer = await _manufacturerRepository.GetByManufacturerName(name);
            return _Mapper.Map<ManufacturerDTO>(manufacturer);
        }
    }
}
