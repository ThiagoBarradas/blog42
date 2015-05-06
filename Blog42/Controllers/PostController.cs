using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Blog42.Security;

namespace Blog42.Controllers
{
    public class PostController : Controller
    {
        public ActionResult Show()
        {
            return View();
        }

        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult All()
        {
            return View();
        }

        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult New()
        {
            return View();
        }

        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
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
