using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Web.Security;
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

            // Verifica se existe usuário logado, se existir se pode deletar (sendo admin ou autor da postagem) e sinaliza para view
            if (Request.IsAuthenticated && (Roles.GetRolesForUser().Contains("Admin") || post.User.Username == User.Identity.Name))
                ViewBag.canDelete = true;

            // Passa postagem para view
            ViewBag.Post = post;
            return View();
        }

        //
        // GET: /Admin/Post
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult All()
        {
            // Declara listagem de postagens
            List<Post> posts;

            // Inicializa posts. Se for admin, recupera todas as postagens, senão, apenas do próprio usuário
            if (Roles.GetRolesForUser().Contains("Admin"))
                posts = postDAO.SelectAllPosts().ToList<Post>();
            else
                posts = postDAO.SelectPostsByAuthor(User.Identity.Name).ToList<Post>();
            
            // Passa listagem para view
            ViewBag.Posts = posts;            
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
