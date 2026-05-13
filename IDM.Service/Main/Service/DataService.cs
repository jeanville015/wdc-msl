using AutoMapper;
using IDM.DTO.Main;
using IDM.DTO.User;
using IDM.Repository.Main.Interface;
using IDM.Repository.User.Interface;
using IDM.Repository.User.Repository;
using IDM.Service.Main.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Main.Service
{
    public class DataService : IDataService
    {
        private readonly IDataRepository _dataRepository;
        private readonly IMapper _Mapper;

        public DataService(IDataRepository dataRepository, IMapper mapper)
        {
            _dataRepository = dataRepository;
            _Mapper = mapper;
        }

        public async Task<IEnumerable<DataDTO>> GetAllAsync()
        {
            var data = await _dataRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<DataDTO>>(data);
        }

        public async Task<IEnumerable<DataDTO>> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId)
        {
            var data = await _dataRepository.GetDataAsync(deliveryDate, receivedDate, materialNo, lotNumber, jobNumber, toolId);
            return _Mapper.Map<IEnumerable<DataDTO>>(data);
        }
    }
}
