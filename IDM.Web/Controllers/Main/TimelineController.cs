using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers.Main
{
    public class TimelineController : BaseController
    {
        // GET: Timeline
        public ActionResult Index()
        {
            SetPageHeader("Timeline");
            return View("~/Views/Main/Timeline/Timeline.cshtml");
        }
    }
}