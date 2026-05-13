using System.Web;
using System.Web.Mvc;

namespace IDM.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // SessionTimeoutAttribute removed temporarily to fix compilation
            // filters.Add(new SessionTimeoutAttribute());
        }
    }
}
