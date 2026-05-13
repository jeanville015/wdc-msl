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
    public class DataController : BaseController
    {
        public readonly IDataService _dataService;
        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" })]
        public ActionResult Index()
        {
            SetPageHeader("Data");
            return View("~/Views/Main/Data/Data.cshtml");
        }

        public async Task<JsonResult> GetData()
        {
            var data = await _dataService.GetAllAsync();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetDataAsync(string deliveryDate, string receivedDate, string materialNo, string lotNumber, string jobNumber, string toolId)
        {
            var data = await _dataService.GetDataAsync(deliveryDate, receivedDate, materialNo, lotNumber, jobNumber, toolId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}