using IDM.DTO.Main;
using IDM.Service.Main.Interface;
using IDM.Service.Main.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Main
{
    public class SmallToolEntryController : BaseController
    {
        public readonly ISmallToolEntryService _smallToolEntryService;

        public SmallToolEntryController(ISmallToolEntryService smallToolEntryService)
        {
            _smallToolEntryService = smallToolEntryService;
        }

        public ActionResult Index()
        {
            SetPageHeader("Small Tool Entry");
            return View("~/Views/Main/SmallToolEntry/SmallToolEntry.cshtml");
        }


        [HttpPost]
        public async Task<JsonResult> CreateToolEntry(SmallToolEntryDTO smallToolEntryDTO)
        {
            if (ModelState.IsValid)
            {
                smallToolEntryDTO.StoredBy = UserId;
                smallToolEntryDTO.StoreTs = CurrentUtcDateTime;
                var id = await _smallToolEntryService.CreateAsync(smallToolEntryDTO);
                if (id == -1)
                {
                    string error = "This Small Tool Entry have an error while inserting!";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> CreateSmallToolStaging(string tableContent)
        {
            var tableData = JsonConvert.DeserializeObject<TableData>(tableContent);

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> CreateTrial(SmallToolEntryDTO smallToolEntryDTO)
        {
            if (ModelState.IsValid)
            {
                smallToolEntryDTO.StoredBy = UserId;
                smallToolEntryDTO.StoreTs = CurrentUtcDateTime;
                var id = await _smallToolEntryService.CreateTrial(smallToolEntryDTO);
                if (id == -1)
                {
                    string error = "This Small tool data entry have an error while inserting the trial values";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }
    }
}