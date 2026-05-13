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
    public class TestingSiteService : ITestingSiteService
    {
        private readonly ITestingSiteRepository _testingSiteRepository;
        private readonly IMapper _Mapper;

        public TestingSiteService(ITestingSiteRepository testingSiteRepository, IMapper mapper)
        {
            _testingSiteRepository = testingSiteRepository;
            _Mapper = mapper;
        }

        public async Task<TestingSiteDTO> GetByIdAsync(int id)
        {
            var testingSite = await _testingSiteRepository.GetByIdAsync(id);
            return _Mapper.Map<TestingSiteDTO>(testingSite);
        }

        public async Task<int> CreateAsync(TestingSiteDTO testingSiteDTO)
        {
            var existingTestingSite = await GetBySiteName(testingSiteDTO.Site_Name);
            if (existingTestingSite != null)
            {
                return -1;
            }

            var testingSite = _Mapper.Map<TestingSite>(testingSiteDTO);
            return await _testingSiteRepository.CreateAsync(testingSite);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _testingSiteRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TestingSiteDTO>> GetAllAsync()
        {
            var testingSite = await _testingSiteRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<TestingSiteDTO>>(testingSite);
        }

        public async Task<bool> UpdateAsync(TestingSiteDTO testingSiteDTO)
        {
            var existingTestingSite = await GetBySiteName(testingSiteDTO.Site_Name);

            if (existingTestingSite != null && existingTestingSite.Id != testingSiteDTO.Id)
            {
                return false;
            }

            var testingSite = _Mapper.Map<TestingSite>(testingSiteDTO);
            return await _testingSiteRepository.UpdateAsync(testingSite);
        }

        public async Task<TestingSiteDTO> GetBySiteName(string name)
        {
            var testingSite = await _testingSiteRepository.GetBySiteName(name);
            return _Mapper.Map<TestingSiteDTO>(testingSite);
        }
    }
}
