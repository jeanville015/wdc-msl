using IDM.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Repository.User.Interface
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<IEnumerable<int>> GetAnalysisListByUserIdAsync( int userId);
        Task<Account> GetByIdAsync(int id);
        Task<int> CreateAsync(Account account);
        Task<bool> UpdateAsync(Account account);
        Task<bool> UpdateLastLoginAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<Account> GetByAccountName(string name);
    }
}
