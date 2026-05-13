using System;
using System.Web.Mvc;
using System.Linq;

namespace IDM.Web.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public string[] AllowedGroups { get; set; }
        public string[] AllowedRoles { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check if user is authenticated
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Check if session has user data
                if (filterContext.HttpContext.Session["UserId"] == null)
                {
                    // Redirect to login page
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            { "controller", "Login" },
                            { "action", "Login" }
                        });
                    return;
                }
            }

            // ── 2. Group authorization check ─────────────────────────────
            if (AllowedGroups != null && AllowedGroups.Length > 0)
            {
                var userGroup = filterContext.HttpContext.Session["UserGroup"]?.ToString() ?? "";

                if (!AllowedGroups.Contains(userGroup))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            { "controller", "Home" },
                            //{ "action", "AccessDenied" }
                            { "action", "Index" }
                        });
                    return;
                }
            }

            // ── 3. Role authorization check ──────────────────────────────
            if (AllowedRoles != null && AllowedRoles.Length > 0)
            {
                var userRole = filterContext.HttpContext.Session["UserRole"]?.ToString() ?? "";

                if (!AllowedRoles.Contains(userRole))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            { "controller", "Home" },
                            //{ "action", "AccessDenied" }
                            { "action", "Index" }
                        });
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
