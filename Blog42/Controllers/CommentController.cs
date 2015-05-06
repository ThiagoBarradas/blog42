using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Blog42.Security;

namespace Blog42.Controllers
{
    public class CommentController : Controller
    {
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult All()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New()
        {
            return View();
        }

        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult Delete(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}
