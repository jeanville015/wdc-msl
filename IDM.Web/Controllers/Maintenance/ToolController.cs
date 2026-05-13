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
    public class ToolController : BaseController
    {
        public readonly IToolService _toolService;

        public ToolController(IToolService toolService)
        {
            _toolService = toolService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "MSL" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Tool");
            return View("~/Views/Maintenance/Tool/Tool.cshtml");
        }

        public async Task<JsonResult> GetTool()
        {
            var tool = await _toolService.GetAllAsync();
            return Json(tool, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetByToolNameAsync(string toolName)
        {
            var tool = await _toolService.GetByToolNameAsync(toolName);
            return Json(tool, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(ToolDTO toolDTO)
        {
            if (ModelState.IsValid)
            {
                toolDTO.StoredBy = UserId;
                toolDTO.StoreTs = CurrentUtcDateTime;
                var id = await _toolService.CreateAsync(toolDTO);
                if (id == -1)
                {
                    string error = "This Tool is existing please try another Tool";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(ToolDTO toolDTO)
        {
            if (ModelState.IsValid)
            {
                toolDTO.UpdatedBy = UserId;
                toolDTO.UpdatedTs = CurrentUtcDateTime;
                var success = await _toolService.UpdateAsync(toolDTO);
                if (!success)
                {
                    string error = "This Tool is existing please try another Tool";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _toolService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}