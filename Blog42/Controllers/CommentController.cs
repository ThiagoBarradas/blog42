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
    public class CommentController : Controller
    {
        // Cria e inicializa objeto de acesso aos dados dos comentários
        private CommentDAO commentDAO = new CommentDAO();

        //
        // GET: /Admin/Comment/
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult All(int? page)
        {
            // Declara listagem de comentários
            IQueryable<Comment> comments;

            // Inicializa posts. Se for admin, recupera todos os comentários, senão, apenas dos posts do próprio usuário
            if (Roles.GetRolesForUser().Contains("Admin"))
                comments = commentDAO.SelectAllComments().AsQueryable();
            else
                comments = commentDAO.SelectCommentsByAuthor(User.Identity.Name).AsQueryable();

            // Cria modelo com paginação de 10 itens por página
            IPagedList<Comment> model = comments.ToPagedList(page ?? 1, 10);

            // Se página inválida
            if (page != null && page > 1 && model.Count == 0)
                return RedirectToAction("All", "Comment"); // Redireciona para pagina de todos os comentários

            return View(model);
        }

        //
        // GET: /Comment/ByPost/{id}
        public ActionResult ByPost(bool? preview, int postId = 0)
        {
            // Se não for uma requisição feita via ajax ou uma view parcial requisitada localmente, redireciona para erro
            if (!(Request.IsAjaxRequest() || ControllerContext.IsChildAction) || !Request.IsLocal)
                return RedirectToAction("Index", "Error"); // redireciona para página de erro

            // Declara listagem de comentários
            List<Comment> comments = commentDAO.SelectCommentsByPost(postId).ToList<Comment>();

            // Verifica se existe usuário logado e algum comentário foi retornado, se existir se pode deletar (sendo admin ou autor da postagem) 
            if (Request.IsAuthenticated && comments.Count>0 && (Roles.GetRolesForUser().Contains("Admin") || comments[0].Post.User.Username == User.Identity.Name) && preview==null)
                ViewBag.canDelete = true; // sinaliza para view

            // Passa listagem para view
            ViewBag.Comments = comments;
            return PartialView();
        }

        //
        // GET: /NewComment
        [ChildActionOnly]
        public ActionResult New(int postId = 0)
        {
            // Retorna view parcial passando modelo com Id do post
            return PartialView(new CommentNew() { PostId = postId });
        }

        //
        // POST: /NewComment
        [HttpPost]
        public ActionResult New(CommentNew commentNew, bool? preview)
        {
            // Se for preview (preview é feita via POST, cai aqui), retorna formulário
            if (preview != null)
            {
                ViewBag.IsPreview = true; // Sinaliza View
                ModelState.Clear(); // Remove erros do form
                return New(0); // Retorna método GET
            }

            // Se não for uma requisição ajax feita pelo próprio servidor, redireciona para página de erro
            if (!(Request.IsAjaxRequest() && Request.IsLocal))
                return RedirectToAction("Index", "Error");

            // Recebe validação de modelo para sinalizar sucesso
            commentNew.IsSuccess = ModelState.IsValid;

            // Se modelo for válido, retorna mensagem de sucesso
            if (ModelState.IsValid)
            {
                //  Tenta criar comentário e salvar no banco
                commentNew.IsSuccess = commentDAO.CreateComment(new Comment()
                {
                    Author = commentNew.Author,
                    Email = commentNew.Email,
                    Comment1 = commentNew.Comment,
                    PostId = commentNew.PostId
                });
                // Se conseguiu criar o comentário, mensagem de sucesso, senão, mensagem de erro
                if(commentNew.IsSuccess)
                    commentNew.Message = "Comentário feito com sucesso!!!";
                else
                    commentNew.Message = "Eita! Infelizmente algum problema ocorreu por aqui. <br><br>Tenta novamente, vai!";
            }
            else // senão, monta mensagem de erro
            {
                // Obtem erros
                var errorList = ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage).ToList();
                
                // Monta mensagem de erro em lista
                string message = "<ul>";
                foreach (var error in errorList) 
                    message += "<li>"+error+"</li>";
                commentNew.Message = message+"</ul>";
            }            

            // Retorna em Json
            return Json(commentNew);
        }

        // 
        // GET: /Admin/Comment/Delete/{id}
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult Delete(int id)
        {
            // Recupera informações do comentário a ser deletado
            Comment comment = commentDAO.GetComment(id);

            // Se comentário não encontrado, ou usuário não tem permissão para deletar redireciona para página de erro 
            if (comment == null || !(Roles.GetRolesForUser().Contains("Admin") || comment.Post.User.Username == User.Identity.Name))
                return RedirectToAction("Index", "Error");

            // Copia as informações recebidas para modelo da página de deleção
            CommentDelete commentDelete = new CommentDelete();
            commentDelete.CommentId = comment.Id;
            commentDelete.Comment = comment.Comment1;
            commentDelete.Author = comment.Author;
            commentDelete.Email = comment.Email;
            commentDelete.CreatedAt = comment.CreatedAt;
            commentDelete.PostId = comment.Post.Id;
            commentDelete.PostTitle = comment.Post.Title;

            return View(commentDelete);
        }

        // 
        // POST: /Admin/Comment/Delete/{id}
        [HttpPost]
        [PermissionFilter(Roles = "Author, Admin")]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(CommentDelete commentDelete) {
            // Recupera informações do comentário a ser deletado
            Comment comment = commentDAO.GetComment(commentDelete.CommentId);

            // Se comentário não encontrado, ou usuário não tem permissão para deletar redireciona para página de erro 
            if (comment == null || !(Roles.GetRolesForUser().Contains("Admin") || comment.Post.User.Username == User.Identity.Name))
                return RedirectToAction("Index", "Error");

            // Tenta deletar, se conseguir, sinaliza sucesso, senão, adiciona erro
            if (commentDAO.DeleteComment(commentDelete.CommentId))
                ViewBag.Success = true;
            else
                ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");

            return View(commentDelete);
        }
    }
}
