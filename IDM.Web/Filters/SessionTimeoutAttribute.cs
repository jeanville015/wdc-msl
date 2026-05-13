using System;
using System.Web.Mvc;

namespace IDM.Web.Filters
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Skip session check for Login controller to avoid redirect loop
            if (!(filterContext.Controller is Controllers.LoginController))
            {
                // Check if session has expired
                if (filterContext.HttpContext.Session["UserId"] == null)
                {
                    // If user is not authenticated, redirect to login
                    if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new System.Web.Routing.RouteValueDictionary
                            {
                                { "controller", "Login" },
                                { "action", "Login" }
                            });
                        return;
                    }
                    
                    // If user is authenticated but session is lost, redirect to login
                    // This handles session timeout scenario
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            { "controller", "Login" },
                            { "action", "Login" }
                        });
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
