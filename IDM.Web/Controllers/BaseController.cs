using IDM.Web.Helper;
using IDM.DTO;
using IDM.Model.Main;
using IDM.Web.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Configuration;
using IDM.DTO.Main;
using IDM.Service.Common.Interface;

namespace IDM.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IEmailService _emailService;
        protected readonly IUserService _userService;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Skip session check for Login controller to avoid redirect loop
            // Skip session check for Staging controller to allow public access
            var controllerName = filterContext.RouteData.Values["controller"]?.ToString();
            if (controllerName != "Login" && controllerName != "Staging")
            {
                // Check if user is authenticated OR session exists
                // Use OR instead of AND to be more flexible
                if (!User.Identity.IsAuthenticated && Session["UserId"] == null)
                {
                    // Debug information
                    System.Diagnostics.Debug.WriteLine($"Authentication check failed:");
                    System.Diagnostics.Debug.WriteLine($"User.Identity.IsAuthenticated: {User.Identity.IsAuthenticated}");
                    System.Diagnostics.Debug.WriteLine($"Session['UserId']: {Session["UserId"]}");
                    System.Diagnostics.Debug.WriteLine($"Request URL: {filterContext.HttpContext.Request.Url}");
                    
                    // Redirect to login page
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            { "controller", "Login" },
                            { "action", "Login" }
                        });
                    return;
                }

                // Additional check: If user is authenticated but session is lost (session timeout)
                if (User.Identity.IsAuthenticated && Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Session timeout detected - user authenticated but session data missing");
                    
                    // Redirect to login page to force re-login
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

        protected string Username
        {
            //get => User.Identity?.Name;
            get => "";
        }
        protected string UserId
        {
            get
            {
                // Try to get from authentication cookie first
                if (User?.Identity?.IsAuthenticated == true)
                {
                    return User.Identity.Name;
                }
                
                // Fallback to session if available
                if (Session["UserId"] != null)
                {
                    return Session["UserId"].ToString();
                }
                
                // Fallback to hardcoded for development
                return "7327671";
            }
        }
        protected DateTime CurrentUtcDateTime
        {
            get
            {
                return DateTime.Now;
            }
        }


        protected override JsonResult Json(object data, string contentType,
                Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new CamelCaseJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected void SetPageHeader(string pageHeader)
        {
            var controller = ControllerContext.RouteData.Values["Controller"]?.ToString();
            var action = ControllerContext.RouteData.Values["Action"]?.ToString();

            var breadcrumbs = new List<(string Title, string Url, string Class)>
            {
                ("Home", "/", ""),
            };

            if (controller == "Home")
            {
                breadcrumbs.Add(("Home", null, ""));
                SetActiveMenu("Home", null);
            }

            #region Maintenance
            if (controller == "Uom")
            {
                breadcrumbs.Add(("Uom", "/Uom", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Uom");
            }
            if (controller == "TestingSite")
            {
                breadcrumbs.Add(("Testing Site", "/TestingSite", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "TestingSite");
            }
            if (controller == "Parameter")
            {
                breadcrumbs.Add(("Parameter", "/Parameter", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Parameter");
            }
            if (controller == "Area")
            {
                breadcrumbs.Add(("Area", "/Area", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Area");
            }
            if (controller == "Manufacturer")
            {
                breadcrumbs.Add(("Manufacturer", "/Manufacturer", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Manufacturer");
            }
            if (controller == "Supplier")
            {
                breadcrumbs.Add(("Supplier", "/Supplier", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Supplier");
            }
            if (controller == "Material")
            {
                breadcrumbs.Add(("Material", "/Material", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Material");
            }
            if (controller == "Tool")
            {
                breadcrumbs.Add(("Tool", "/Tool", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Tool");
            }
            if (controller == "ToolType")
            {
                breadcrumbs.Add(("Tool Type", "/ToolType", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "ToolType");
            }
            if (controller == "Analysis")
            {
                breadcrumbs.Add(("Analysis", "/Analysis", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Analysis");
            }
            if (controller == "MaterialSettings")
            {
                breadcrumbs.Add(("Material Settings", "/MaterialSettings", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "MaterialSettings");
            }
            if (controller == "Defect")
            {
                breadcrumbs.Add(("Defect", "/Defect", action == "Index" ? "active" : ""));
                SetActiveMenu("Maintenance", "Defect");
            }
            #endregion
            #region User
            if (controller == "Role")
            {
                breadcrumbs.Add(("Role", "/Role", action == "Index" ? "active" : ""));
                SetActiveMenu("User", "Role");
            }
            if (controller == "Account")
            {
                breadcrumbs.Add(("Account", "/Account", action == "Index" ? "active" : ""));
                SetActiveMenu("User", "Account");
            }
            #endregion
            #region Main
            if (controller == "IncomingData")
            {
                breadcrumbs.Add(("Incoming Data", "/IncomingData", action == "Index" ? "active" : ""));
                SetActiveMenu("IncomingData", "");
            }
            if (controller == "Timeline")
            {
                breadcrumbs.Add(("Timeline", "/Timeline", action == "Index" ? "active" : ""));
                SetActiveMenu("Timeline", "");
            }
            if (controller == "Data")
            {
                breadcrumbs.Add(("Data", "/Data", action == "Index" ? "active" : ""));
                SetActiveMenu("Data", "");
            }
            if (controller == "ToolEntryItems")
            {
                breadcrumbs.Add(("Tool Entry Items", "/ToolEntryItems", action == "Index" ? "active" : ""));
                SetActiveMenu("ToolEntryItems", "");
            }
            if (controller == "SmallToolEntry")
            {
                breadcrumbs.Add(("Small Tool Entry", "/SmallToolEntry", action == "Index" ? "active" : ""));
                SetActiveMenu("SmallToolEntry", "");
            }
            #endregion

            ViewData["PageHeader"] = pageHeader;
            ViewData["Breadcrumbs"] = breadcrumbs;
        }

        protected void SetActiveMenu(string activeMenu, string activeSubMenu = null)
        {
            ViewData["ActiveMenu"] = activeMenu;
            ViewData["ActiveSubMenu"] = activeSubMenu;
        }

        protected ConfigDTO GetConfiguration()
        {
            return new ConfigDTO
            {
                MQConnectionFile = GetAppSetting("MQConnectionFile"),
                MQTransaction = GetAppSetting("MQTransaction"),
                MQTransactionTrial = GetAppSetting("MQTransactionTrial"),
                MQVersion = GetAppSetting("MQVersion"),
                MQExcludeColumn = GetAppSetting("MQExcludeColumn"),
                MQAdjustColumn = GetAppSetting("MQAdjustColumn"),

                SMTPHost = GetAppSetting("SMTPHost"),
                SMTPPort = GetAppSetting("SMTPPort"),
                EmailSender = GetAppSetting("EmailSender"),
                DefaultEmailRecipients = GetAppSetting("DefaultEmailRecipients"),
                Website = GetAppSetting("Website")
            };
        }

        protected string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key]?.ToString() ?? string.Empty;
        }

        
        
        protected bool SQEMail(IncomingDataDTO incomingData)
        {
            // Wrapper method for backward compatibility - delegates to EmailService
            var result = _emailService.SendFailedDataEmailAsync(incomingData).Result;
            return result;
        }

        protected bool RejectedMail(string analyzedBy, string job, string analysis, int analysisTrial, string status, string approver)
        {
            // Wrapper method for backward compatibility - delegates to EmailService
            var result = _emailService.SendRejectionEmailAsync(analyzedBy, job, analysis, analysisTrial, status, approver).Result;
            return result;
        }

        protected bool ApproveMail(string analyzedBy, string job, string analysis, int analysisTrial, string status, string approver, string customer, string returnUrl)
        {
            // Wrapper method for backward compatibility - delegates to EmailService
            var result = _emailService.SendApprovalEmailAsync(analyzedBy, job, analysis, analysisTrial, status, approver, customer, returnUrl).Result;
            return result;
        }

        public BaseController(IEmailService emailService, IUserService userService)
        {
            _emailService = emailService;
            _userService = userService;
        }

        // Parameterless constructor for backward compatibility during migration
        public BaseController()
        {
            // Temporary bridge - manually instantiate services until all controllers use proper DI
            var config = GetConfiguration();
            _userService = new IDM.Service.Common.Service.UserService();
            _emailService = new IDM.Service.Common.Service.EmailService(_userService, config, new IDM.Web.Utility.MailSender());
        }
    }
}