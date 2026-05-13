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
    public class ManufacturerController : BaseController
    {
        public readonly IManufacturerService _manufacturerService;

        public ManufacturerController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Manufacturer");
            return View("~/Views/Maintenance/Manufacturer/Manufacturer.cshtml");
        }

        public async Task<JsonResult> GetManufacturer()
        {
            var manufacturer = await _manufacturerService.GetAllAsync();
            return Json(manufacturer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(ManufacturerDTO manufacturer)
        {
            if (ModelState.IsValid)
            {
                manufacturer.StoredBy = UserId;
                manufacturer.StoreTs = CurrentUtcDateTime;
                var id = await _manufacturerService.CreateAsync(manufacturer);
                if (id == -1)
                {
                    string error = "This Manufacturer is existing please try another Manufacturer";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(ManufacturerDTO manufacturer)
        {
            if (ModelState.IsValid)
            {
                manufacturer.UpdatedBy = UserId;
                manufacturer.UpdatedTs = CurrentUtcDateTime;
                var success = await _manufacturerService.UpdateAsync(manufacturer);
                if (!success)
                {
                    string error = "This Manufacturer is existing please try another Manufacturer";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _manufacturerService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}