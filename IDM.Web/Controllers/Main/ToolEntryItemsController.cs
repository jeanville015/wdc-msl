using IDM.DTO.Main;
using IDM.Service.Main.Interface;
using IDM.Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Main
{
    public class ToolEntryItemsController : BaseController
    {
        public readonly IToolEntryItemsService _toolEntryItemsService;
        public readonly IStagingService _stagingService;
        public readonly IDynamicStagingService _dynamicStagingService;
        public ToolEntryItemsController(IToolEntryItemsService toolEntryItemsService, IStagingService stagingService, IDynamicStagingService dynamicStagingService)
        {
            _toolEntryItemsService = toolEntryItemsService;
            _stagingService = stagingService;
            _dynamicStagingService = dynamicStagingService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "MSL" })]
        public ActionResult Index()
        {
            SetPageHeader("Tool Entry Items");
            return View("~/Views/Main/ToolEntryItems/ToolEntryItems.cshtml");
        }

        public async Task<JsonResult> GetData()
        {
            var data = await _toolEntryItemsService.GetAllAsync();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAllAsyncForApprove()
        {
            var data = await _toolEntryItemsService.GetAllAsyncForApprove();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAllAsyncByAmethystJob(ToolEntryItemsDTO toolEntryItemsDTO)
        {
            var data = await _toolEntryItemsService.GetAllAsyncByAmethystJob(toolEntryItemsDTO.AmethystJob);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string analyzedBy)
        {
            var data = await _toolEntryItemsService.GetDataAsync(deliveryDate, receivedDate, materialNo, lotNumber, jobNumber, analyzedBy);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> GetByJobAndAnalysisAsync(string table, string amethystJob, string analysis)
        //{
        //    var data = await _stagingService.GetByJobAndAnalysisAsync(table, amethystJob, analysis);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public async Task<JsonResult> GetByJobAndAnalysisAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            var data = await _dynamicStagingService.GetByJobAndAnalysisAsync(table, amethystJob, analysis, analysisTrial);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}