using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IDM.Web.Controllers
{
    public class UnderConstructionController : BaseController
    {
        public ActionResult Index(string subMenu)
        {
            ViewBag.ActiveSubMenu = subMenu;
            return View();
        }
    }
}