using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;
using IDM.Service.Maintenance.Interface;
using IDM.Service.Maintenance.Service;
using IDM.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Maintenance
{
    public class MaterialSettingsController : BaseController
    {
        public readonly IMaterialSettingsService _materialSettingsService;

        public MaterialSettingsController(IMaterialSettingsService materialSettingsService)
        {
            _materialSettingsService = materialSettingsService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "MSL" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Material Settings");
            return View("~/Views/Maintenance/MaterialSettings/MaterialSettings.cshtml");
        }

        public async Task<JsonResult> GetMaterialSettings()
        {
            var materialSettings = await _materialSettingsService.GetAllAsync();
            return Json(materialSettings, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(MaterialSettingsDTO materialSettingsDTO)
        {
            if (ModelState.IsValid)
            {
                materialSettingsDTO.StoredBy = UserId;
                materialSettingsDTO.StoreTs = CurrentUtcDateTime;
                var id = await _materialSettingsService.CreateAsync(materialSettingsDTO);
                if (id == -1)
                {
                    string error = "This Material Setting is existing please try another Material Setting";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(MaterialSettingsDTO materialSettingsDTO)
        {
            if (ModelState.IsValid)
            {
                materialSettingsDTO.UpdatedBy = UserId;
                materialSettingsDTO.UpdatedTs = CurrentUtcDateTime;
                var success = await _materialSettingsService.UpdateAsync(materialSettingsDTO);
                if (!success)
                {
                    string error = "This Material Setting is existing please try another Material Setting";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _materialSettingsService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}