using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;
using IDM.Service.Maintenance.Interface;
using IDM.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Maintenance
{
    public class MaterialController : BaseController
    {
        public readonly IMaterialService _materialService;
        public readonly IMaterialParameterService _materialParameterService;

        public MaterialController(IMaterialService materialService, IMaterialParameterService materialParameterService)
        {
            _materialService = materialService;
            _materialParameterService = materialParameterService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Material");
            return View("~/Views/Maintenance/Material/Material.cshtml");
        }

        public async Task<JsonResult> GetMaterial()
        {
            var material = await _materialService.GetAllAsync();
            return Json(material, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(MaterialDTO material)
        {
            if (ModelState.IsValid)
            {
                material.StoredBy = UserId;
                material.StoreTs = CurrentUtcDateTime;
                var id = await _materialService.CreateAsync(material);
                if (id == -1)
                {
                    string error = "This Material Number is existing please try another Material";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(MaterialDTO material)
        {
            if (ModelState.IsValid)
            {
                material.UpdatedBy = UserId;
                material.UpdatedTs = CurrentUtcDateTime;
                var success = await _materialService.UpdateAsync(material);
                if (!success)
                {
                    string error = "This Material Number is existing please try another Material";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _materialService.DeleteAsync(id);
            return Json(new { success });
        }

        public async Task<JsonResult> GetMaterialParameterById(string id)
        {
            var material = await _materialParameterService.GetByMaterialNoAsync(id);
            return Json(material, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CreateMaterialParameter(MaterialParameterDTO materialParameter)
        {
            if (ModelState.IsValid)
            {
                materialParameter.StoredBy = UserId;
                materialParameter.StoreTs = CurrentUtcDateTime;
                var id = await _materialParameterService.CreateAsync(materialParameter);
                if (id == -1)
                {
                    string error = "This Material Parameter is existing please try another Material Parameter";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteMaterialParameter(int id)
        {
            var success = await _materialParameterService.DeleteAsync(id);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<JsonResult> EditMaterialParameter(MaterialParameterDTO materialParameter)
        {
            if (ModelState.IsValid)
            {
                materialParameter.UpdatedBy = UserId;
                materialParameter.UpdatedTs = CurrentUtcDateTime;
                var success = await _materialParameterService.UpdateAsync(materialParameter);
                if (!success)
                {
                    string error = "This Material Parameter is existing please try another Material Parameter";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }
    }
}