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
    public class CommentController : Controller
    {
        // Cria e inicializa objeto de acesso aos dados dos comentários
        private CommentDAO commentDAO = new CommentDAO();

        //
        // GET: /Admin/Comment/
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult All()
        {
            // Declara listagem de comentários
            List<Comment> comments;

            // Inicializa posts. Se for admin, recupera todos os comentários, senão, apenas dos posts do próprio usuário
            if (Roles.GetRolesForUser().Contains("Admin"))
                comments = commentDAO.SelectAllComments().ToList<Comment>();
            else
                comments = commentDAO.SelectCommentsByAuthor(User.Identity.Name).ToList<Comment>();

            // Passa listagem para view
            ViewBag.Comments = comments;
            return View();
        }

        [HttpPost]
        public ActionResult New()
        {
            return View();
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
