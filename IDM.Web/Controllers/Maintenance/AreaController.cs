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
    public class AreaController : BaseController
    {
        public readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Area");
            return View("~/Views/Maintenance/Area/Area.cshtml");
        }

        public async Task<JsonResult> GetArea()
        {
            var area = await _areaService.GetAllAsync();
            return Json(area, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(AreaDTO areaDto)
        {
            if (ModelState.IsValid)
            {
                areaDto.StoredBy = UserId;
                areaDto.StoreTs = CurrentUtcDateTime;
                //areaDto.STOREDBY = UserId;
                //areaDto.STORETS = CurrentUtcDateTime;
                var id = await _areaService.CreateAsync(areaDto);
                if (id == -1)
                {
                    string error = "This Area is existing please try another Area";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(AreaDTO areaDto)
        {
            if (ModelState.IsValid)
            {
                //areaDto.UPDATEDBY = UserId;
                //areaDto.UPDATEDTS = CurrentUtcDateTime;
                areaDto.UpdatedBy = UserId;
                areaDto.UpdatedTs = CurrentUtcDateTime;
                var success = await _areaService.UpdateAsync(areaDto);
                if (!success)
                {
                    string error = "This Area is existing please try another Area";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _areaService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}