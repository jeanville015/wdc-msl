using AutoMapper;
using IDM.DTO.Main;
using IDM.Model.Main;
using IDM.Repository.Main.Interface;
using IDM.Repository.Main.Repository;
using IDM.Service.Main.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.Main.Service
{
    public class SmallToolEntryService : ISmallToolEntryService
    {
        private readonly ISmallToolEntryRepository _smallToolEntryRepository;
        private readonly IMapper _Mapper;

        public SmallToolEntryService(ISmallToolEntryRepository smallToolEntryRepository, IMapper mapper)
        {
            _smallToolEntryRepository = smallToolEntryRepository;
            _Mapper = mapper;
        }
        public async Task<int> CreateAsync(SmallToolEntryDTO smallToolEntryDTO)
        {
            var smallToolEntry = _Mapper.Map<SmallToolEntry>(smallToolEntryDTO);
            return await _smallToolEntryRepository.CreateAsync(smallToolEntry);
        }

        public async Task<int> GetAnalysisTrial(SmallToolEntryDTO smallToolEntryDTO)
        {
            var smallToolEntry = _Mapper.Map<SmallToolEntry>(smallToolEntryDTO);
            return await _smallToolEntryRepository.GetAnalysisTrial(smallToolEntry);
        }

        public async Task<int> CreateTrial(SmallToolEntryDTO smallToolEntryDTO)
        {
            var smallToolEntry = _Mapper.Map<SmallToolEntry>(smallToolEntryDTO);

            await _smallToolEntryRepository.RetireTrial(smallToolEntry);
            return await _smallToolEntryRepository.CreateTrial(smallToolEntry);
        }
    }
}
