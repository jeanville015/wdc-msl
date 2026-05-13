using System.Threading.Tasks;

namespace IDM.Service.Common.Interface
{
    public interface IUserService
    {
        Task<string> GetUserEmailAsync(string adNumber);
        Task<string> GetUserNameAsync(string adNumber);
    }
}
