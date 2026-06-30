using IDM.DTO.Main;
using IDM.DTO.Main.View;
using IDM.Service.Main.Interface;
using IDM.Service.Main.Service;
using IDM.Web.DataAccess;
using IDM.Web.Filters;
using IDM.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IDM.Model;
using IDM.Model.Common;
using System.Data;

namespace IDM.Web.Controllers.Main
{
    public class PendingDataController : BaseController
    {
        public readonly IPendingDataService _pendingDataService;
        public PendingDataController(IPendingDataService pendingDataService)
        {
            _pendingDataService = pendingDataService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" })]
        public ActionResult Index()
        {
            SetPageHeader("Pending Data");
            return View("~/Views/Main/PendingData/PendingData.cshtml");
        }

        public async Task<ActionResult> GetAllPendingDataAsync(int page=1, int pageSize=10)
        { 
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
                var data = await _pendingDataService.GetPendingDataDetailsPaginatedAsync(deliveryDate, receivedDate, lotNumber, materialNo, jobNumber, toolId, page, pageSize);

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

        [HttpPost]
        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" }, AllowedRoles = new[] { "Admin" })]
        public async Task<JsonResult> SetStatusDecision(string status, string lotNumber, string deliveryDate, string receivedDate, string materialNo, string jobNumber, string toolId)
        {
            try
            {
                status = status?.Trim().ToUpperInvariant();

                if (status != "PASSED" && status != "REJECTED")
                    return Json(new { success = false, message = "Invalid decision status." });

                // MQ upload; EDCSPC upload
                if (status == "PASSED")
                {
                    var pendingData = await _pendingDataService.GetPendingDataDetailsAsync(deliveryDate, receivedDate, lotNumber, materialNo, jobNumber, toolId);
                    // Get MQ configuration 
                    var config = GetConfiguration();

                    // Upload parameters to MQ------------------------------------------------------------------------//
                    var parameterResult = await _pendingDataService.MQUploadPreparationParameter(config, pendingData);
                    if (parameterResult == -1)
                        return Json(new { success = false, message = "MQ upload failed for Parameter table." });
                    //------------------------------------------------------------------------------------------------//

                    // Upload trials to MQ--------------------------------------------------------------------//
                    var trialResult = await _pendingDataService.MQUploadPreparationTrial(config, pendingData);
                    if (trialResult == -1)
                        return Json(new { success = false, message = "MQ upload failed for Trial table." });
                    //---------------------------------------------------------------------------------------//

                    // EDCSPC -------------------------------------------------------------------------------------------//
                    var edcspcResult = _pendingDataService.SendEDCSPC(pendingData, Convert.ToString(Session["Username"]));
                    if (!edcspcResult.Success)
                        return Json(new { success = false, message = "EDCSPC upload failed: " + edcspcResult.Error });
                    // -------------------------------------------------------------------------------------------------//
                }

                // Only update status after successful MQ / EDCSPC processing-----------------------------------------------------------------------------------------//
                var rowsAffected = await _pendingDataService.UpdateDataParameterDetails(status, lotNumber, deliveryDate, receivedDate, materialNo, jobNumber, toolId);
                if (rowsAffected <= 0)
                    return Json(new
                    {
                        success = false,
                        message = "No matching pending records were found."
                    });
                //----------------------------------------------------------------------------------------------------------------------------------------------------//

                return Json(new
                {
                    success = true,
                    message = $"Status changed to {status}.",
                    rowsAffected
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the status."
                });
            }
        }


    }
}