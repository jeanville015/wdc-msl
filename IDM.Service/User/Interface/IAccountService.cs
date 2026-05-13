using IDM.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Service.User.Interface
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDTO>> GetAllAsync();
        Task<IEnumerable<AccountDTO>> GetAccountOnly();
        Task<AccountDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(AccountDTO accountDTO);
        Task<bool> UpdateAsync(AccountDTO accountDTO);
        Task<bool> UpdateLastLoginAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<AccountDTO> GetByAccountName(string name);
    }
}
