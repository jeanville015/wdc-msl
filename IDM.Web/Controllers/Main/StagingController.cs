using IDM.DTO.Main.View;
using IDM.Model.Common;
using IDM.Model.Maintenance;
using IDM.Service.Common.Interface;
using IDM.Service.Main.Interface;
using IDM.Service.Main.Service;
using IDM.Service.Maintenance.Interface;
using IDM.Service.Maintenance.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing; 
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace IDM.Web.Controllers.Main
{
    // Class to match the table data structure from JavaScript
    public class TableData
    {
        public List<string> Columns { get; set; }
        public List<List<string>> Data { get; set; }
    }

    [Authorize]
    public class StagingController : BaseController
    {
        public readonly IDynamicStagingService _dynamicStagingService;
        public readonly IAnalysisService _analysisService;

        public StagingController(IDynamicStagingService dynamicStagingService, IAnalysisService analysisService)
        {
            _dynamicStagingService = dynamicStagingService;
            _analysisService = analysisService;
        }

        public ActionResult Index()
        {
            SetPageHeader("Staging");
            return View("~/Views/Main/Staging/Staging.cshtml");
        }
        public ActionResult WImage()
        {
            SetPageHeader("Staging");
            return View("~/Views/Main/Staging/Staging_WImage.cshtml");
        }

        public async Task<JsonResult> GetByAnalysisAsync(string analysisName)
        {
            var analysis = await _analysisService.GetByAnalysisAsync(analysisName);
            return Json(analysis, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetByAnalysis2KXAsync(string analysisName)
        {
            var analysis = await _analysisService.GetByAnalysisAsync(analysisName);
            return View(analysis);
        }

        public async Task<JsonResult> GetByJobAndAnalysisAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            var status = await _dynamicStagingService.GetStatusAsync(amethystJob, analysis, analysisTrial);
            if (status != "PENDING")
            {
                return Json(new { 
                    redirect = true, 
                    redirectUrl = Url.Action("Index", "ToolEntryItems"),
                    status = status,
                    message = $"Analysis already {status.ToLower()}, redirecting to Tool Entry Items..."
                }, JsonRequestBehavior.AllowGet);
            }
            
            var data = await _dynamicStagingService.GetByJobAndAnalysisAsync(table, amethystJob, analysis, analysisTrial);
            return Json(new { 
                redirect = false, 
                data = data,
                status = status
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetByJobAndAnalysisPartialAsync(string table, string amethystJob, string analysis, int analysisTrial)
        {
            var status = await _dynamicStagingService.GetStatusAsync(amethystJob, analysis, analysisTrial);
            if (status != "PENDING")
            {
                return Json(new
                {
                    redirect = true,
                    redirectUrl = Url.Action("Index", "ToolEntryItems"),
                    status = status,
                    message = $"Analysis already {status.ToLower()}, redirecting to Tool Entry Items..."
                }, JsonRequestBehavior.AllowGet);
            } 

            var data = await _dynamicStagingService.GetByJobAndAnalysisAsync(table, amethystJob, analysis, analysisTrial);
            var first = data.First();
            var staticFields = new[] { "AmethystJob", "Analysis", "AnalysisTrial", "LotNumber", "DateAnalyzed", "AnalyzedBy", "DateReviewed", "ReviewedBy", "Tool", "SampleSequence" };

            // Define the specific dynamic columns you want to show
            var allowedDynamicColumns = new[] { "Area", "SubArea", "RawFileName", "Image", "Annotation" };

            var grouped = data.GroupBy(x => x.AdditionalProperties.ContainsKey("SampleSequence")
                    ? x.AdditionalProperties["SampleSequence"]?.ToString()
                    : "Unknown")
                  .ToDictionary(g => g.Key, g => g.ToList());

            // Build display labels separately — safe characters only in the key
            var tabLabels = grouped.ToDictionary(
                g => g.Key,
                g => {
                    var firstRow = g.Value.First();
                    var seq = g.Key;
                    var product = firstRow.AdditionalProperties.ContainsKey("Product") ? firstRow.AdditionalProperties["Product"]?.ToString() : "";
                    var sliderSN = firstRow.AdditionalProperties.ContainsKey("SliderSN") ? firstRow.AdditionalProperties["SliderSN"]?.ToString() : "";
                    return $"S{seq}_{product}_{sliderSN}";
                }
            );

            var dynamicStagingTabsView = new DynamicStagingTabsView
            {   
                AmethystJob = first.AmethystJob,
                Analysis = first.Analysis,
                // Extracting others from AdditionalProperties if they aren't core properties
                AnalysisTrial = first.GetProperty("AnalysisTrial")?.ToString(),
                LotNumber = first.GetProperty("LotNumber")?.ToString(),
                DateAnalyzed = first.GetProperty("DateAnalyzed")?.ToString(),
                AnalyzedBy = await _userService.GetUserNameAsync(first.GetProperty("AnalyzedBy")?.ToString()),
                DateReviewed = first.GetProperty("DateReviewed")?.ToString(),
                ReviewedBy = await _userService.GetUserNameAsync(first.GetProperty("ReviewedBy")?.ToString()),
                Tool = first.GetProperty("Tool")?.ToString(),
                DynamicHeaders = first.AdditionalProperties.Keys.Where(k => allowedDynamicColumns.Contains(k)).ToList(),
                GroupedData = grouped,
                TabLabels = tabLabels 
            };

            return PartialView("~/Views/Main/Staging/2KX/_list.cshtml", dynamicStagingTabsView); 
        }

        public async Task<ActionResult> GetDataStagingDefectAsync(string table, string amethystJob, string analysis, int analysisTrial, string area, string subArea)
        {
           var data = await _dynamicStagingService.GetDataStagingDefectAsync(table, amethystJob, analysis, analysisTrial, area, subArea);
           var first = data.First();
           var staticFields = new[] { "AmethystJob", "Analysis", "AnalysisTrial", "Area", "SubArea"};

            // Define the specific dynamic columns you want to show
            var allowedDynamicColumns = new[] { "DefectGroup", "Defect", "SemImage", "EdxImage" };

            var dynamicStagingDetailsView = new DynamicStagingDetailsView
            {
                AmethystJob = first.AmethystJob,
                Analysis = first.Analysis,
                // Extracting others from AdditionalProperties if they aren't core properties
                AnalysisTrial = first.GetProperty("AnalysisTrial")?.ToString(),
                Area = first.GetProperty("Area")?.ToString(),
                SubArea = first.GetProperty("SubArea")?.ToString(),

                DynamicHeaders = first.AdditionalProperties.Keys
                                      .Where(k => allowedDynamicColumns.Contains(k)).ToList(),

                Data = data.OrderBy(x => x.AdditionalProperties.ContainsKey("SampleSequence")
                                     ? x.AdditionalProperties["SampleSequence"]?.ToString()
                                     : "Unknown")
                        .ToList()
            };

            return PartialView("~/Views/Main/Staging/2KX/Details/_list.cshtml", dynamicStagingDetailsView);
        }

        /// <summary>
        ///  sample data
        /// </summary>
        /// <param name="sourceTable">DATA_STAGING_2KX</param>
        /// <param name="table">DESTINATION_2KX</param>
        /// <param name="amethystJob">123456711</param>
        /// <param name="analysis">2KX</param>
        /// <param name="analysisTrial">1</param>
        /// <param name="analyzedBy">7327671</param>
        /// <param name="status">PASSED</param>
        /// <param name="tableContent"></param>
        /// <returns></returns>
        public async Task<JsonResult> SetApproval(string sourceTable, string table, string amethystJob, string analysis, int analysisTrial, string analyzedBy, string status, string tableContent)
        {

            var ExecuteBDPRequirementsProcessResult = new OperationResult();

            try
            {
                var config = GetConfiguration();

                // Deserialize table content from JSON string
                var tableData = JsonConvert.DeserializeObject<TableData>(tableContent);

                ////var tableData = await _dynamicStagingService.GetByJobAndAnalysisDataTableAsync(sourceTable, amethystJob, analysis, analysisTrial);

                var result = await _dynamicStagingService.SetApprovalAsync(table, amethystJob, analysis, analysisTrial, status, analyzedBy, tableData, UserId);

                if (status == "REJECTED")
                {
                    RejectedMail(analyzedBy, amethystJob, analysis, analysisTrial, status, UserId);
                }
                else
                {
                    var uploaded = await _dynamicStagingService.MQUploadPreparationParameter(tableData, config, table, UserId);
                    if (!uploaded)
                    {
                        return Json(new { success = false, message = "Error uploading on MQ", data = uploaded }, JsonRequestBehavior.AllowGet);
                    }

                    var customer = await _dynamicStagingService.GetCustomerAsync(amethystJob, analysis, analysisTrial);
                    ApproveMail(analyzedBy, amethystJob, analysis, analysisTrial, status, UserId, customer, null);
                }

                return Json(new { success = true, message = "Approval processed successfully", data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error processing approval: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> SetApproval_StagingWImage(string sourceTable, string table, string amethystJob, string analysis, int analysisTrial, string analyzedBy, string status, string tableContent, string returnUrl)
        {
            // PROCESS NOT READY
            //return Json(new { success = false, message = "This Feature/Process is under development!"}, JsonRequestBehavior.AllowGet); 

            var result = new OperationResult() { OperationStatus = true, OperationStatusMessage = "Set Approval Successful!" };
            if (status == "REJECTED")
            {
                var resultEmailSendRejectionEmail = _emailService.SendRejectionEmailAsync(analyzedBy, amethystJob, analysis, analysisTrial, status, UserId).Result;
            }
            else //status == "APPROVED"
            {

                OperationResult ORsetApprovalStagingWImage = await _dynamicStagingService.SetApprovalStagingWImage(sourceTable, table, amethystJob, analysis, analysisTrial, analyzedBy, status, tableContent, UserId);
                if (ORsetApprovalStagingWImage.OperationStatus == false)
                {
                    return Json(new { success = false, message = "Error processing approval: " + ORsetApprovalStagingWImage.OperationStatusMessage }, JsonRequestBehavior.AllowGet);
                }

                var customer = await _dynamicStagingService.GetCustomerAsync(amethystJob, analysis, analysisTrial);
                var resultEmailSendApprovalEmail = _emailService.SendApprovalEmailAsync(analyzedBy, amethystJob, analysis, analysisTrial, status, UserId, customer, returnUrl).Result;
            }
            return Json(new { success = true, message = "Approval processed successfully", data = true }, JsonRequestBehavior.AllowGet); 
        }

        //[HttpGet]
        [Route("Image/Preview/{*fullPath}")]
        public ActionResult Preview(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath) || !System.IO.File.Exists(fullPath))
            {
                return ReturnPlaceholderImage();
            }

            string extension = Path.GetExtension(fullPath).ToLower();

            // If it's a TIFF, convert it to PNG for the browser
            if (extension == ".tif" || extension == ".tiff")
            {
                using (var image = System.Drawing.Image.FromFile(fullPath))
                {
                    var ms = new MemoryStream();
                    // TIFFs can be multipage; this default grabs the first page
                    image.Save(ms, ImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }

            // Standard behavior for browser-supported formats (jpg, png, gif)
            string contentType = MimeMapping.GetMimeMapping(fullPath);
            return File(fullPath, contentType);
        }

        private ActionResult ReturnPlaceholderImage()
        {
            int width = 160, height = 120;

            using (var bmp = new System.Drawing.Bitmap(width, height))
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Background
                g.Clear(System.Drawing.Color.FromArgb(240, 240, 240));

                // Border
                using (var borderPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(200, 200, 200), 2))
                    g.DrawRectangle(borderPen, 1, 1, width - 2, height - 2);

                // Camera body (rounded rect)
                var cameraColor = System.Drawing.Color.FromArgb(180, 180, 180);
                using (var brush = new System.Drawing.SolidBrush(cameraColor))
                {
                    g.FillRectangle(brush, 55, 38, 50, 36);   // body
                    g.FillEllipse(brush, 48, 33, 14, 10);      // left bump
                    g.FillRectangle(brush, 98, 43, 8, 6);      // viewfinder
                }

                // Lens (circle)
                using (var lensBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(210, 210, 210)))
                    g.FillEllipse(lensBrush, 68, 43, 24, 24);

                using (var lensBorderPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(160, 160, 160), 2))
                    g.DrawEllipse(lensBorderPen, 68, 43, 24, 24);

                // "No Image" text
                using (var font = new System.Drawing.Font("Arial", 9, System.Drawing.FontStyle.Regular))
                using (var textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(150, 150, 150)))
                {
                    var textFormat = new System.Drawing.StringFormat { Alignment = System.Drawing.StringAlignment.Center };
                    g.DrawString("No Image", font, textBrush, new System.Drawing.RectangleF(0, 85, width, 20), textFormat);
                }

                var ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return File(ms.ToArray(), "image/png");
            }
        }

    }
}