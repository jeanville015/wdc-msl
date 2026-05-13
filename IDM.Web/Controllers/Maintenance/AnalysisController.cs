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
    public class AnalysisController : BaseController
    {
        public readonly IAnalysisService _analysisService;

        public AnalysisController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Analysis");
            return View("~/Views/Maintenance/Analysis/Analysis.cshtml");
        }

        public async Task<JsonResult> GetAnalysis()
        {
            var analysis = await _analysisService.GetAllAsync();
            return Json(analysis, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetByToolTypeAndAnalysisAsync(int toolTypeId, string analysisName)
        {
            var analysis = await _analysisService.GetByToolTypeAndAnalysisAsync(toolTypeId, analysisName);
            return Json(analysis, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetByAnalysisAsync(string analysisName)
        {
            var analysis = await _analysisService.GetByAnalysisAsync(analysisName);
            return Json(analysis, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(AnalysisDTO analysisDTO)
        {
            if (ModelState.IsValid)
            {
                analysisDTO.StoredBy = UserId;
                analysisDTO.StoreTs = CurrentUtcDateTime;
                var id = await _analysisService.CreateAsync(analysisDTO);
                if (id == -1)
                {
                    string error = "This Analysis is existing please try another Analysis";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(AnalysisDTO analysisDTO)
        {
            if (ModelState.IsValid)
            {
                analysisDTO.UpdatedBy = UserId;
                analysisDTO.UpdatedTs = CurrentUtcDateTime;
                var success = await _analysisService.UpdateAsync(analysisDTO);
                if (!success)
                {
                    string error = "This Analysis is existing please try another Analysis";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _analysisService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}