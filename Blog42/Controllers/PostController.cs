using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Blog42.Security;
using Blog42.DataAccess;
using Blog42.Models;

namespace Blog42.Controllers
{
    public class PostController : Controller
    {
        // Cria e inicializa objeto de acesso aos dados das postagens
        private PostDAO postDAO = new PostDAO();

        //
        // GET: /Post/{id}
        public ActionResult Show(int id)
        {
            // Recebe dados da postagem
            Post post = postDAO.GetPost(id);

            // Se não recebeu post, redireciona para erro
            if (post == null)
                return RedirectToAction("Index", "Error");

            // Passa postagem para view
            ViewBag.Post = post;
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
