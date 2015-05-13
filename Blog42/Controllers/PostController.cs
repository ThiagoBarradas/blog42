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

        //
        // GET: /Admin/New
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult New()
        {
            return View();
        }

        //
        // POST: /Admin/New
        [HttpPost]
        [PermissionFilter(Roles = "Author, Admin")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken()]
        public ActionResult New(PostNew postNew)
        {
            if (ModelState.IsValid) // Se não existir Username cadastrado e dados do formulário forem válidos
            {
                // Recupera usuário atual
                UserDAO userDAO = new UserDAO();
                User user = userDAO.GetUser(User.Identity.Name);

                // Se não conseguiu recuperar usuário, página de erro
                if (user == null)
                    return RedirectToAction("Index", "Error");

                // Cria usuário e atribui valores a novo usuário
                Post post = new Post();
                post.Title = postNew.Title;
                post.Content = postNew.Content;
                post.CreatedBy = user.Id;

                // Tenta persistir novo usuário, se conseguir sinaliza sucesso para a view, senão, adiciona mensagem de erro
                if (postDAO.CreatePost(post))
                    ViewBag.Success = true;
                else
                    ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");
            }
            return View(postNew);
        }

        //
        //GET: /Admin/Post/Edit/{id}
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        //
        // GET: /Admin/Post/Delete/{id}
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult Delete(int id)
        {
            // Recupera informações do post a ser deletado
            Post post = postDAO.GetPost(id);

            // Se post não encontrado, ou usuário não tem permissão para deletar redireciona para página de erro 
            if (post == null || !(Roles.GetRolesForUser().Contains("Admin") || post.User.Username == User.Identity.Name))
                return RedirectToAction("Index", "Error");

            // Copia as informações recebidas para modelo da página de deleção
            PostDelete postDelete = new PostDelete();
            postDelete.PostId = post.Id;
            postDelete.Title = post.Title;
            postDelete.CreatedBy = post.User.Name;
            postDelete.CreatedAt = post.CreatedAt;

            return View(postDelete);
        }

        //
        // POST: /Admin/Post/Delete/{id}
        [HttpPost]
        [PermissionFilter(Roles = "Author, Admin")]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(PostDelete postDelete)
        {
            // Recupera informações do post a ser deletado
            Post post = postDAO.GetPost(postDelete.PostId);

            // Se post não encontrado, ou usuário não tem permissão para deletar redireciona para página de erro 
            if (post == null || !(Roles.GetRolesForUser().Contains("Admin") || post.User.Username == User.Identity.Name))
                return RedirectToAction("Index", "Error");

            // Tenta deletar, se conseguir, sinaliza sucesso, senão, adiciona erro
            if (postDAO.DeletePost(postDelete.PostId))
                ViewBag.Success = true;
            else
                ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");

            return View(postDelete);
        }
    }
}
