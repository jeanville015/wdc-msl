using IDM.DTO.Maintenance;
using IDM.DTO.User;
using IDM.Service.User.Interface;
using IDM.Web.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IDM.Web.Controllers.User
{
    public class RoleController : BaseController
    {
        public readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [SessionAuthorize(AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Role");
            return View("~/Views/User/Role/Role.cshtml");
        }

        public async Task<JsonResult> GetRole()
        {
            var role = await _roleService.GetAllAsync();
            return Json(role, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(RoleDTO role)
        {
            if (ModelState.IsValid)
            {
                role.StoredBy = UserId;
                role.StoreTs = CurrentUtcDateTime;
                var id = await _roleService.CreateAsync(role);
                if (id == -1)
                {
                    string error = "This Role is existing please try another Role";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(RoleDTO role)
        {
            if (ModelState.IsValid)
            {
                role.UpdatedBy = UserId;
                role.UpdatedTs = CurrentUtcDateTime;
                var success = await _roleService.UpdateAsync(role);
                if (!success)
                {
                    string error = "This Role is existing please try another Role";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _roleService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}