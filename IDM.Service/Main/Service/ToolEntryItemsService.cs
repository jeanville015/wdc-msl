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
    public class ToolEntryItemsService : IToolEntryItemsService
    {
        private readonly IToolEntryItemsRepository _toolEntryItemsRepository;
        private readonly IMapper _Mapper;

        public ToolEntryItemsService(IToolEntryItemsRepository toolEntryItemsRepository, IMapper mapper)
        {
            _toolEntryItemsRepository = toolEntryItemsRepository;
            _Mapper = mapper;
        }

        public async Task<IEnumerable<ToolEntryItemsDTO>> GetAllAsync()
        {
            var data = await _toolEntryItemsRepository.GetAllAsync();
            return _Mapper.Map<IEnumerable<ToolEntryItemsDTO>>(data);
        }

        public async Task<IEnumerable<ToolEntryItemsDTO>> GetAllAsyncForApprove()
        {
            var data = await _toolEntryItemsRepository.GetAllAsyncForApprove();
            return _Mapper.Map<IEnumerable<ToolEntryItemsDTO>>(data);
        }

        public async Task<IEnumerable<ToolEntryItemsDTO>> GetAllAsyncByAmethystJob(string amethystJob)
        {
            var data = await _toolEntryItemsRepository.GetAllAsyncByAmethystJob(amethystJob);
            return _Mapper.Map<IEnumerable<ToolEntryItemsDTO>>(data);
        }

        public async Task<IEnumerable<ToolEntryItemsDTO>> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId)
        {
            var data = await _toolEntryItemsRepository.GetDataAsync(deliveryDate, receivedDate, materialNo, lotNumber, jobNumber, toolId);
            return _Mapper.Map<IEnumerable<ToolEntryItemsDTO>>(data);
        }
    }
}
