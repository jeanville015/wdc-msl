using IDM.DTO.User;
using IDM.Service.User.Interface;
using IDM.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers.User
{
    public class AccountController : BaseController
    {
        public readonly IAccountService _accountService;
        public readonly IRoleService _roleService;

        public AccountController(IAccountService accountService,
            IRoleService roleService)
        {
            _accountService = accountService;
            _roleService = roleService;
        }

        [SessionAuthorize(AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Account");
            return View("~/Views/User/Account/Account.cshtml");
        }

        public async Task<JsonResult> GetAccount()
        {
            var account = await _accountService.GetAllAsync();
            return Json(account, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAccountOnly()
        {
            var account = await _accountService.GetAccountOnly();
            return Json(account, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(AccountDTO account)
        {
            if (ModelState.IsValid)
            {
                account.StoredBy = UserId;
                account.StoreTs = CurrentUtcDateTime;
                var id = await _accountService.CreateAsync(account);
                if (id == -1)
                {
                    string error = "This account is existing please try another account";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(AccountDTO account)
        {
            if (ModelState.IsValid)
            {
                account.UpdatedBy = UserId;
                account.UpdatedTs = CurrentUtcDateTime;
                var success = await _accountService.UpdateAsync(account);
                if (!success)
                {
                    string error = "This account is existing please try another account";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _accountService.DeleteAsync(id);
            return Json(new { success });
        }

        public async Task<JsonResult> GetRole()
        {
            var role = await _roleService.GetAllAsync();
            return Json(role, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetGroup()
        {
            // Your constant list of groups
            var groups = new List<string> { "IT", "SQE", "MSL"}; 

            return Json(groups, JsonRequestBehavior.AllowGet);
        }

    }
}