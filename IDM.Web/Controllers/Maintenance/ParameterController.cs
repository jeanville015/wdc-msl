using IDM.DTO.Maintenance;
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
    public class ParameterController : BaseController
    {
        public readonly IParameterService _parameterService;

        public ParameterController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Parameter");
            return View("~/Views/Maintenance/Parameter/Parameter.cshtml");
        }

        public async Task<JsonResult> GetParameter()
        {
            var Parameter = await _parameterService.GetAllAsync();
            return Json(Parameter, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(ParameterDTO parameterDTO)
        {
            if (ModelState.IsValid)
            {
                parameterDTO.StoredBy = UserId;
                parameterDTO.StoreTs = CurrentUtcDateTime;
                var id = await _parameterService.CreateAsync(parameterDTO);
                if (id == -1)
                {
                    string error = "This Parameter is existing please try another Parameter";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(ParameterDTO parameterDTO)
        {
            if (ModelState.IsValid)
            {
                parameterDTO.UpdatedBy = UserId;
                parameterDTO.UpdatedTs = CurrentUtcDateTime;
                var success = await _parameterService.UpdateAsync(parameterDTO);
                if (!success)
                {
                    string error = "This Parameter is existing please try another Parameter";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _parameterService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}