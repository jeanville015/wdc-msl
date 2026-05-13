using AutoMapper;
using IDM.DTO;
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
    public class StagingService : IStagingService
    {
        private readonly IStagingRepository _stagingRepository;
        private readonly IMapper _Mapper;

        public StagingService(IStagingRepository stagingRepository, IMapper mapper)
        {
            _stagingRepository = stagingRepository;
            _Mapper = mapper;
        }

        public async Task<IEnumerable<StagingDTO>> GetByJobAndAnalysisAsync(string table, string amethysJob, string analysis, int analysisTrial)
        {
            var data = await _stagingRepository.GetByJobAndAnalysisAsync(table, amethysJob, analysis, analysisTrial);
            return _Mapper.Map<IEnumerable<StagingDTO>>(data);
        }


    }
}
