using IDM.DTO.Main.View;
using IDM.Service.Main.Interface;
using IDM.Web.DataAccess;
using IDM.Web.Filters;
using IDM.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Main
{
    public class PendingDataController : BaseController
    {
        private readonly EDCSPC _edcspc = new EDCSPC();
        public readonly IPendingDataService _pendingDataService;
        public PendingDataController(IPendingDataService pendingDataService)
        {
            _pendingDataService = pendingDataService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" })]
        public ActionResult Index()
        {
            SetPageHeader("PendingData");
            return View("~/Views/Main/PendingData/PendingData.cshtml");
        }

        public async Task<ActionResult> GetAllPendingDataAsync(int page=1, int pageSize=10)
        {
            //var data = await _pendingDataService.GetAllAsync(page, pageSize);
            //return PartialView("~/Views/Main/PendingData/_list.cshtml", data);

            try
            {
                var data = await _pendingDataService.GetAllAsync(page, pageSize);
                return PartialView("~/Views/Main/PendingData/_list.cshtml", data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPendingDataDetailsAsync(string deliveryDate, string receivedDate, string lotNumber, string materialNo, string jobNumber, string toolId, int page = 1, int pageSize = 10)
        {
            try
            {
                var data = await _pendingDataService.GetPendingDataDetailsAsync(deliveryDate, receivedDate, lotNumber, materialNo, jobNumber, toolId, page, pageSize);

                return PartialView("~/Views/Main/PendingData/_list_details.cshtml", data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ActionResult> GetPendingDataAsync()
        {
            var data = await _pendingDataService.GetPendingDataAsync();
            return PartialView("~/Views/Main/PendingData/PendingData.cshtml", data);
        }
    }
}