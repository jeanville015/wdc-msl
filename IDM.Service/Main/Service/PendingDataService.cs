using AutoMapper;
using IDM.DTO.Main;
using IDM.DTO.Maintenance;
using IDM.DTO.User;
using IDM.Model.Maintenance;
using IDM.Repository.Main.Interface;
using IDM.Repository.Main.Repository;
using IDM.Repository.User.Interface;
using IDM.Repository.User.Repository;
using IDM.Service.Main.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Main.Service
{
    public class PendingDataService : IPendingDataService
    {
        private readonly IPendingDataRepository _pendingDataRepository;
        private readonly IMapper _Mapper;

        public PendingDataService(IPendingDataRepository pendingDataRepository, IMapper mapper)
        {
            _pendingDataRepository = pendingDataRepository;
            _Mapper = mapper;
        }
         
        public async Task<PagedResultDTO<PendingDataDTO>> GetAllAsync(int page, int pageSize)
        {

            try
            {
                var result = await _pendingDataRepository.GetAllAsync(page, pageSize);

                var mappedItems = result.Items
                    .Select(entity => _Mapper.Map<PendingDataDTO>(entity))
                    .ToList();

                return new PagedResultDTO<PendingDataDTO>
                {
                    Items = mappedItems,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    PageSize = pageSize,
                    TotalCount = result.TotalCount
                };
            }
            catch (Exception ex)
            {
                throw;
            } 
        }
         
        public async Task<PagedResultDTO<PendingDataDTO>> GetPendingDataDetailsAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId, int page, int pageSize)
        {

            try
            {
                var result = await _pendingDataRepository.GetPendingDataDetailsAsync(deliveryDate, receivedDate, lotNumber, materialNo, jobNumber, toolId, page, pageSize);

                var mappedItems = result.Items
                    .Select(entity => _Mapper.Map<PendingDataDTO>(entity))
                    .ToList();

                return new PagedResultDTO<PendingDataDTO>
                {
                    Items = mappedItems,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize),
                    PageSize = pageSize,
                    TotalCount = result.TotalCount
                };
            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        public async Task<IEnumerable<PendingDataDTO>> GetPendingDataAsync()
        {
            var data = await _pendingDataRepository.GetPendingDataAsync();
            return _Mapper.Map<IEnumerable<PendingDataDTO>>(data);
        }
    }
}
