using IDM.Service.Common.Interface;
using System;
using System.DirectoryServices;
using System.Threading;
using System.Threading.Tasks;

namespace IDM.Service.Common.Service
{
    public class UserService : IUserService
    {
        private static DateTime _lastFailureTime = DateTime.MinValue;
        private static readonly TimeSpan _circuitBreakTimeout = TimeSpan.FromMinutes(5);

        public async Task<string> GetUserEmailAsync(string adNumber)
        {
            return await GetUserInfoByAdNumber(adNumber,0);
        }

        public async Task<string> GetUserNameAsync(string adNumber)
        {
            return await GetUserInfoByAdNumber(adNumber,1);
        }

        private async Task<string> GetUserInfoByAdNumber(string adNumber, int x)
        {
            try
            {
                using (DirectoryEntry adsEntry = new DirectoryEntry("LDAP://DC=ad,DC=shared"))
                {
                    using (DirectorySearcher adsSearcher = new DirectorySearcher(adsEntry))
                    {
                        adsSearcher.Filter = "(sAMAccountName=" + adNumber + ")";
                        adsSearcher.PropertiesToLoad.Add("mail");
                        adsSearcher.PropertiesToLoad.Add("displayName");
                        adsSearcher.ClientTimeout = TimeSpan.FromSeconds(5);
                        adsSearcher.ServerTimeLimit = TimeSpan.FromSeconds(5);

                        SearchResult result = adsSearcher.FindOne();
                        if (result != null)
                        {
                            string email = result.Properties["mail"].Count > 0 ? 
                                result.Properties["mail"][0].ToString() : string.Empty;
                            string name = result.Properties["displayName"].Count > 0 ? 
                                result.Properties["displayName"][0].ToString() : string.Empty;
                            if (x == 1) return name; else return email;
                          
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AD lookup failed for {adNumber}: {ex.Message}");
            }
            return null;
        }
    }
}
