using IDM.DTO.Maintenance;
using IDM.Model.Maintenance;
using IDM.Service.Maintenance.Interface;
using IDM.Web.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Maintenance
{
    public class DefectController : BaseController
    {
        public readonly IDefectService _defectService;

        public DefectController(IDefectService defectService)
        {
            _defectService = defectService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "MSL" }, AllowedRoles = new[] { "Admin" })]
        public ActionResult Index()
        {
            SetPageHeader("Defect");
            return View("~/Views/Maintenance/Defect/Defect.cshtml");
        }

        public async Task<JsonResult> GetDefect()
        {
            var defect = await _defectService.GetAllAsync();
            return Json(defect, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Create(DefectDTO defect)
        {
            if (ModelState.IsValid)
            {
                defect.StoredBy = UserId;
                defect.StoreTs = CurrentUtcDateTime;
                var id = await _defectService.CreateAsync(defect);
                if (id == -1)
                {
                    string error = "This Defect is existing please try another Defect";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(DefectDTO defect)
        {
            if (ModelState.IsValid)
            {
                defect.UpdatedBy = UserId;
                defect.UpdatedTs = CurrentUtcDateTime;
                var success = await _defectService.UpdateAsync(defect);
                if (!success)
                {
                    string error = "This Defect is existing please try another Defect";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var success = await _defectService.DeleteAsync(id);
            return Json(new { success });
        }
         
        [HttpPost] 
        public async Task<JsonResult> UploadDefect(HttpPostedFileBase file)
        {
            DataTable dt = new DataTable();
            DefectBulkDTO defectBulk = new DefectBulkDTO();
            if ((ModelState.IsValid) && (file != null || file.ContentLength > 0))
            {
                using (var reader = new StreamReader(file.InputStream))
                {
                    // 1. Read the first line for headers
                    string[] headers = reader.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header.Trim());
                    }
                    dt.Columns.Add("STOREDBY", typeof(string)); 
                    dt.Columns.Add("ActiveFlag", typeof(char)); 
                    dt.Columns.Add("STORETS", typeof(DateTime)); 

                    // 2. Read the remaining rows
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] rows = line.Split(',');
                            // Create a new row based on the table's schema
                            DataRow newRow = dt.NewRow();

                            // Fill columns from CSV (6 columns)
                            for (int i = 0; i < rows.Length; i++)
                            {
                                newRow[i] = rows[i].Trim();
                            }

                            // fill required columns not in csv
                            newRow["STOREDBY"] = UserId;
                            newRow["ActiveFlag"] = 'Y';
                            newRow["STORETS"] = CurrentUtcDateTime; 
                            dt.Rows.Add(newRow);
                        }
                    }
                }
                defectBulk.DefectDataTable = dt;
                var success = await _defectService.UploadFile(defectBulk);
                if (!success)
                {
                    string error = "Some of the defect on the CSV file is existing please check the data.";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success });
            }
            return Json(new { success = false });
        }
    }
}