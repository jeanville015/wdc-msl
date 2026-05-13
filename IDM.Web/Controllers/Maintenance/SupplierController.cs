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
    public class SupplierController : BaseController
    {
        public readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Supplier");
            return View("~/Views/Maintenance/Supplier/Supplier.cshtml");
        }

        public async Task<JsonResult> GetSupplier()
        {
            var supplier = await _supplierService.GetAllAsync();
            return Json(supplier, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(SupplierDTO supplier)
        {
            if (ModelState.IsValid)
            {
                supplier.StoredBy = UserId;
                supplier.StoreTs = CurrentUtcDateTime;
                var id = await _supplierService.CreateAsync(supplier);
                if (id == -1)
                {
                    string error = "This Supplier is existing please try another Supplier";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(SupplierDTO supplier)
        {
            if (ModelState.IsValid)
            {
                supplier.UpdatedBy = UserId;
                supplier.UpdatedTs = CurrentUtcDateTime;
                var success = await _supplierService.UpdateAsync(supplier);
                if (!success)
                {
                    string error = "This Supplier is existing please try another Supplier";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _supplierService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}