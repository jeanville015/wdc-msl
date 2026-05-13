using IDM.DTO.Maintenance;
using IDM.Service.Maintenance.Interface;
using IDM.Web.Filters;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Maintenance
{
    public class TestingSiteController : BaseController
    {
        public readonly ITestingSiteService _testingSiteService;

        public TestingSiteController(ITestingSiteService testingSiteService)
        {
            _testingSiteService = testingSiteService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Testing Site");
            return View("~/Views/Maintenance/TestingSite/TestingSite.cshtml");
        }

        public async Task<JsonResult> GetSite()
        {
            var testingSite = await _testingSiteService.GetAllAsync();
            return Json(testingSite, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(TestingSiteDTO testingSiteDTO)
        {
            if (ModelState.IsValid)
            {
                testingSiteDTO.StoredBy = UserId;
                testingSiteDTO.StoreTs = CurrentUtcDateTime;
                var id = await _testingSiteService.CreateAsync(testingSiteDTO);
                if (id == -1)
                {
                    string error = "This Testing Site is existing please try another Testing Site";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(TestingSiteDTO testingSiteDTO)
        {
            if (ModelState.IsValid)
            {
                testingSiteDTO.UpdatedBy = UserId;
                testingSiteDTO.UpdatedTs = CurrentUtcDateTime;
                var success = await _testingSiteService.UpdateAsync(testingSiteDTO);
                if (!success)
                {
                    string error = "This Testing Site is existing please try another Testing Site";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _testingSiteService.DeleteAsync(id);
            return Json(new { success });
        }
    }
}