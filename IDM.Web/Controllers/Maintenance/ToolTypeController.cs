using IDM.DTO.Maintenance;
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
    public class ToolTypeController : BaseController
    {
        public readonly IToolTypeService _toolTypeService;

        public ToolTypeController(IToolTypeService toolTypeService)
        {
            _toolTypeService = toolTypeService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "MSL" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Tool Type");
            return View("~/Views/Maintenance/ToolType/ToolType.cshtml");
        }

        public async Task<JsonResult> GetToolType()
        {
            var tool = await _toolTypeService.GetAllAsync();
            return Json(tool, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetByIdAsync(int id)
        {
            var tool = await _toolTypeService.GetByIdAsync(id);
            return Json(tool, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(ToolTypeDTO toolTypeDTO)
        {
            if (ModelState.IsValid)
            {
                toolTypeDTO.StoredBy = UserId;
                toolTypeDTO.StoreTs = CurrentUtcDateTime;
                var id = await _toolTypeService.CreateAsync(toolTypeDTO);
                if (id == -1)
                {
                    string error = "This Tool Type is existing please try another Tool Type";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(ToolTypeDTO toolTypeDTO)
        {
            if (ModelState.IsValid)
            {
                toolTypeDTO.UpdatedBy = UserId;
                toolTypeDTO.UpdatedTs = CurrentUtcDateTime;
                var success = await _toolTypeService.UpdateAsync(toolTypeDTO);
                if (!success)
                {
                    string error = "This Tool Type is existing please try another Tool Type";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _toolTypeService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}