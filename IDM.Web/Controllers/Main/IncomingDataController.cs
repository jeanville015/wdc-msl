using IDM.DTO;
using IDM.DTO.Main;
using IDM.Model.Main;
using IDM.Service.Main.Interface;
using IDM.Web.Filters;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Main
{
    public class IncomingDataController : BaseController
    {
        public readonly IIncomingDataService _incomingDataService;
        public IncomingDataController(IIncomingDataService incomingDataService)
        {
            _incomingDataService = incomingDataService;
        }

        [SessionAuthorize(AllowedGroups = new[] { "IT", "SQE" })]
        public ActionResult Index()
        {
            SetPageHeader("Incoming Data");
            return View("~/Views/Main/IncomingData/IncomingData.cshtml");
        }

        [HttpPost]
        public async Task<JsonResult> Create(IncomingDataDTO incomingDataDTO)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid model state" });

            // Set audit fields
            incomingDataDTO.StoredBy = UserId;
            incomingDataDTO.StoreTs = CurrentUtcDateTime;

            // Create the incoming data
            var id = await _incomingDataService.CreateAsync(incomingDataDTO);
            if (id == -1)
                return Json(new { success = false, message = "This Incoming Data already exists. Please try another Area." });

            // Get MQ configuration
            var config = GetConfiguration();
            //PDBAXLib.PdbClass PDB = new PDBAXLib.PdbClass();
            // Upload parameters to MQ
            var parameterResult = await _incomingDataService.MQUploadPreparationParameter(incomingDataDTO, config);
            if (parameterResult == -1)
                return Json(new { success = false, message = "MQ upload failed for Parameter table." });

            // Upload trials to MQ
            var trialResult = await _incomingDataService.MQUploadPreparationTrial(incomingDataDTO, config);
            if (trialResult == -1)
                return Json(new { success = false, message = "MQ upload failed for Trial table." });

            // Send email if any parameter failed or is out of control
            if (incomingDataDTO.Parameters.Any(p => 
                p.Specs_Judgement == "FAILED" || 
                p.Control_Judgement == "FAILED" || 
                p.Control_Judgement == "OOC"))
            {
                SQEMail(incomingDataDTO);
            }

            return Json(new { success = true, id });
        }

        
        [HttpPost]
        public async Task<JsonResult> CreateTrial(IncomingDataDTO incomingDataDTO)
        {
            if (ModelState.IsValid)
            {
                incomingDataDTO.StoredBy = UserId;
                incomingDataDTO.StoreTs = CurrentUtcDateTime;
                var id = await _incomingDataService.CreateTrial(incomingDataDTO);
                if (id == -1)
                {
                    string error = "This Incoming Data is existing please try another Area";
                    return Json(new { success = false, message = error });
                }
                return Json(new { success = true, id });
            }
            return Json(new { success = false });
        }

    }
}