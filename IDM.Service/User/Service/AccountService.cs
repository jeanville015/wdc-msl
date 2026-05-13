using AutoMapper;
using IDM.DTO.User;
using IDM.Model.User;
using IDM.Repository.User.Interface;
using IDM.Service.User.Interface;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IDM.Service.User.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _Mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _Mapper = mapper;
        }

        public async Task<AccountDTO> GetByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            return _Mapper.Map<AccountDTO>(account);
        }

        public async Task<int> CreateAsync(AccountDTO accountDTO)
        {
            var existingAccount = await GetByAccountName(accountDTO.User_Id);
            if (existingAccount != null)
            {
                return -1;
            }

            var account = _Mapper.Map<Account>(accountDTO);
            return await _accountRepository.CreateAsync(account);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _accountRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AccountDTO>> GetAllAsync()
        {
            var account = await _accountRepository.GetAllAsync();
            var accountDTOs = _Mapper.Map<List<AccountDTO>>(account);
            foreach (var dto in accountDTOs)
            {
                if (!string.IsNullOrWhiteSpace(dto.User_Id))
                {
                    dto.AdName = await GetFullNameByAD(dto.User_Id);
                }

                //List of analysis per User
                dto.User_Analysis = (await _accountRepository.GetAnalysisListByUserIdAsync(dto.Id)).ToList();
            }
            return accountDTOs;
            //return _Mapper.Map<IEnumerable<AccountDTO>>(account);
        }

        public async Task<IEnumerable<int>> GetAnalysisListByUserIdAsync(int userId)
        {
            var analysisListByUserId = await _accountRepository.GetAnalysisListByUserIdAsync(userId);
            return analysisListByUserId; 
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountOnly()
        {
            var account = await _accountRepository.GetAllAsync();
            var accountDTOs = _Mapper.Map<List<AccountDTO>>(account);
            return accountDTOs;
            //return _Mapper.Map<IEnumerable<AccountDTO>>(account);
        }

        public async Task<bool> UpdateAsync(AccountDTO accountDTO)
        {
            var existingAccount = await GetByAccountName(accountDTO.User_Id);

            if (existingAccount != null && existingAccount.Id != accountDTO.Id)
            {
                return false;
            }

            var account = _Mapper.Map<Account>(accountDTO);
            return await _accountRepository.UpdateAsync(account);
        }

        public async Task<bool> UpdateLastLoginAsync(int id)
        {
            return await _accountRepository.UpdateLastLoginAsync(id);
        }

        public async Task<AccountDTO> GetByAccountName(string name)
        {
            var account = await _accountRepository.GetByAccountName(name);
            return _Mapper.Map<AccountDTO>(account);
        }

        public async Task<string> GetFullNameByAD(string adNumber)
        {
            return await Task.Run(() =>
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    var user = UserPrincipal.FindByIdentity(context, adNumber);
                    return user?.DisplayName ?? string.Empty;
                }
            });
        }
    }
}
