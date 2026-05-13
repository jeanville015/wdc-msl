using IDM.DTO.Maintenance;
using IDM.Service.Maintenance.Interface;
using IDM.Web.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Maintenance
{
    public class UomController : BaseController
    {
        public readonly IUomService _uomService;

        public UomController(IUomService uomService)
        {
            _uomService = uomService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Uom");
            return View("~/Views/Maintenance/Uom/Uom.cshtml");
        }

        public async Task<JsonResult> GetUom()
        {
            var uom = await _uomService.GetAllAsync();
            return Json(uom, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(UomDTO uom)
        {
            if (ModelState.IsValid)
            {
                uom.StoredBy = UserId;
                uom.StoreTs = CurrentUtcDateTime;
                var id = await _uomService.CreateAsync(uom);
                if (id == -1)
                {
                    string error = "This UOM is existing please try another UOM";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(UomDTO uom)
        {
            if (ModelState.IsValid)
            {
                uom.UpdatedBy = UserId;
                uom.UpdatedTs = CurrentUtcDateTime;
                var success = await _uomService.UpdateAsync(uom);
                if (!success)
                {
                    string error = "This UOM is existing please try another UOM";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _uomService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}