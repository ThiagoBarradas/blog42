using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Web.Security;
using PagedList;
using Blog42.Security;
using Blog42.DataAccess;
using Blog42.Models;

namespace Blog42.Controllers
{
    [ValidateInput(false)]
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
        public ActionResult All(int? page)
        {
            // Verifica se página recebida menor que o mínimo (1), se for, atribui o valor mínimo
            if ((page ?? 1) < 1)
                page = 1;
            
            // Declara listagem de postagens
            IQueryable<Post> posts;

            // Inicializa posts. Se for admin, recupera todas as postagens, senão, apenas do próprio usuário
            if (Roles.GetRolesForUser().Contains("Admin"))
                posts = postDAO.SelectAllPosts().AsQueryable();
            else
                posts = postDAO.SelectPostsByAuthor(User.Identity.Name).AsQueryable();

            // Cria modelo com paginação de 10 itens por página
            IPagedList<Post> model = posts.ToPagedList(page ?? 1, 10);

            // Se página inválida
            if (page != null && page > 1 && model.Count == 0)
                return RedirectToAction("All", "Post", new { page = 1 }); //Redireciona para pagina de todas as postagens

            return View(model);
        }

        //
        // GET: /Admin/Post/Preview
        [HttpPost]
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult Preview(int postId, string title, string content, bool changeAuthor)
        {
            // Declara variaveis locais
            Post post;
            
            // Cria objeto de persistencia de usuário e recupera dados do usuário
            UserDAO userDAO = new UserDAO();
            User user = userDAO.GetUser(User.Identity.Name);

            // Verifica se recebeu usuário, senão redireciona para página de erro
            if (user == null)
                return RedirectToAction("Index", "Error");

            // Verifica se é pré visualização na criação ou na edição
            if (postId > 0)
            {
                // Recebe post original
                post = postDAO.GetPost(postId);
                
                // Verifica se conseguiu receber post, se não recebeu, redireciona para página de erro
                if (post == null)
                    return RedirectToAction("Index", "Error");
                
                // Se admin quer mudar o autor para si mesmo, ele altera na edição também
                if(changeAuthor)
                    post.User = user;

                // Atualiza data de modificação
                post.LastUpdateAt = DateTime.Now;

            }
            else
            {
                // Cria post vazio
                post = new Post();
                // Atualiza data de modificação e usuário autor
                post.CreatedAt = DateTime.Now;
                post.User = user;
            }

            // altera propriedades em comum
            post.Title = title;
            post.Content = content;            

            // Passa post para a view
            ViewBag.Post = post;
            return View();
        }

        //
        // GET: /Admin/Post/New
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult New()
        {
            return View();
        }

        //
        // POST: /Admin/Post/New
        [HttpPost]
        [PermissionFilter(Roles = "Author, Admin")]
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

                // Tenta persistir novo usuário, se conseguir sinaliza sucesso e passa id do post para a view, senão, adiciona mensagem de erro
                if (postDAO.CreatePost(post))
                {
                    ViewBag.Success = true;
                    ViewBag.PostId = post.Id;
                }
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
            // Cria variavel de modelo que será passado a view
            PostEdit postEdit = new PostEdit();

            // Recupera informações do post 
            Post post = postDAO.GetPost(id); 

            // Se post não encontrado, redireciona para página de erro
            if (post == null)
                return RedirectToAction("Index", "Error");

            // Copia as informações recebidas para modelo da página de edição
            postEdit.PostId = post.Id;
            postEdit.Title = post.Title;
            postEdit.Content = post.Content;
            postEdit.ChangeAuthor = false;

            // Se estiver editando seu próprio post, sinaliza
            if (post.User.Username == User.Identity.Name)
                ViewBag.EditMyPost = true;
            else if (!Roles.GetRolesForUser().Contains("Admin")) // Se não tiver editando próprio post e não for admin, redireciona para página de erro
                return RedirectToAction("Index", "Error");

            return View(postEdit);
        }

         //
        //GET: /Admin/Post/Edit/{id}
        [HttpPost]
        [PermissionFilter(Roles = "Author, Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PostEdit postEdit)
        {
            // Recupera informações do post 
            Post post = postDAO.GetPost(postEdit.PostId);

            // Se post não encontrado, redireciona para página de erro
            if (post == null)
                return RedirectToAction("Index", "Error");

            // Se estiver editando seu próprio post, sinaliza
            if (post.User.Username == User.Identity.Name)
                ViewBag.EditMyPost = true;
            else if (!Roles.GetRolesForUser().Contains("Admin")) // Se não tiver editando próprio post e não for admin, redireciona para página de erro
                return RedirectToAction("Index", "Error");

            // Se dados do formulário são válidos, tenta editar
            if (ModelState.IsValid)
            {
                // Atribui novos valores a usuário editado
                post.Title = postEdit.Title;
                post.Content = postEdit.Content;
                post.LastUpdateAt = DateTime.Now;

                // Verifica se é para alterar o autor para ele mesmo
                if (postEdit.ChangeAuthor)
                {
                    // Inicializa objeto de persistencia de usuário e recupera dados do usuário atual
                    UserDAO userDAO = new UserDAO();
                    User user = userDAO.GetUser(User.Identity.Name);
                    post.CreatedBy = user.Id;
                }

                // Tenta editar, se conseguir, sinaliza sucesso, senão, adiciona erro
                if (postDAO.UpdatePost(post))
                    ViewBag.Success = true;
                else
                    ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");
            }
            return View(postEdit);
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

            var roles = Roles.GetRolesForUser();

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
