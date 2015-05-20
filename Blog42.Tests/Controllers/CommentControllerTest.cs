using System;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PagedList;
using Moq;
using Blog42;
using Blog42.DataAccess;
using Blog42.Models;
using Blog42.Controllers;
using Blog42.Tests.Helpers;
using Blog42.Tests.Common;

namespace Blog42.Tests.Controllers
{
    [TestClass]
    public class CommentControllerTest : Common.GeneralSetup
    {
        private CommentController controller;

        // Inicializa controller para testes
        [TestInitialize]
        public void Setup()
        {
            controller = new CommentController();
        }

        #region CreateTests

        // Testa criação de comentário, request via Ajax POST e Local
        [TestMethod]
        public void SubmitByAjaxPostLocalReturnSuccess_CommentNew()
        {
            // Atribui o contexto passando flags de Ajax, Post e Local
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Ajax, RequestHelper.TypeRequest.Post, RequestHelper.TypeRequest.Local);

            // Cria novo commentariopara postagem 1 (já existete no banco de testes)
            CommentNew commentNew = new CommentNew() { Author = "Teste" , Comment = "Teste" , PostId = 1 };

            // Executa action passando modelo e, null para indicar que não é pre visualização
            var result = controller.New(commentNew, null);

            // Tem que receber um resultado em Json
            Assert.IsInstanceOfType(result, typeof(JsonResult));

            // Converte o Json para objeto do tipo de modelo de criação e tem que ter a flag IsSuccess como true
            commentNew = ((result as JsonResult).Data as CommentNew);
            Assert.IsTrue(commentNew.IsSuccess);
            
            // Verifica se objeto foi criado
            CommentDAO commentDAO = new CommentDAO();
            var comments = commentDAO.SelectCommentsByPost(1);
            Assert.AreEqual("Teste", comments[0].Author);
        }

        // Testa criação de comentário, via Ajax GET e Local
        [TestMethod]
        public void SubmitByGETRedirectError_CommentNew()
        {
            // Atribui o contexto passando apenas flag de request ajax e local (method padrão GET)
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Ajax, RequestHelper.TypeRequest.Local);

            // Cria novo commentariopara postagem 1 (já existete no banco de testes)
            CommentNew commentNew = new CommentNew() { Author = "Teste", Comment = "Teste", PostId = 1 };

            // Executa action passando modelo e, null para indicar que não é pre visualização
            RedirectToRouteResult redirect = (RedirectToRouteResult)controller.New(commentNew, null);

            // Verifica se recebeu corretamente a Action e Controller
            Assert.IsTrue(redirect.RouteValues.ContainsKey("action"));
            Assert.IsTrue(redirect.RouteValues.ContainsKey("controller"));
            // Verifica se os valores redirecionam para página inicial
            Assert.AreEqual("Index", redirect.RouteValues["action"].ToString());
            Assert.AreEqual("Error", redirect.RouteValues["controller"].ToString());
        }

        // Testa criação de comentário, via request não-local
        [TestMethod]
        public void SubmitByNotLocalRedirectError_CommentNew()
        {
            // Atribui o contexto passando apenas flag de request ajax e post (requisição não é local, apenas quando criado passando TypeRequest.Local)
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Ajax, RequestHelper.TypeRequest.Post);

            // Cria novo commentariopara postagem 1 (já existete no banco de testes)
            CommentNew commentNew = new CommentNew() { Author = "Teste", Comment = "Teste", PostId = 1 };

            // Executa action passando modelo e, null para indicar que não é pre visualização
            RedirectToRouteResult redirect = (RedirectToRouteResult)controller.New(commentNew, null);

            // Verifica se recebeu corretamente a Action e Controller
            Assert.IsTrue(redirect.RouteValues.ContainsKey("action"));
            Assert.IsTrue(redirect.RouteValues.ContainsKey("controller"));
            // Verifica se os valores redirecionam para página inicial
            Assert.AreEqual("Index", redirect.RouteValues["action"].ToString());
            Assert.AreEqual("Error", redirect.RouteValues["controller"].ToString());
        }

        // Testa criação de comentário, que não pode ser feito se não via ajax.
        [TestMethod]
        public void SubmitByNotAjaxRedirectError_CommentNew()
        {
            // Atribui o contexto passando apenas flag de request post e local (padrão requisição ajax)
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Post, RequestHelper.TypeRequest.Local);

            // Cria novo commentariopara postagem 1 (já existete no banco de testes)
            CommentNew commentNew = new CommentNew() { Author = "Teste", Comment = "Teste", PostId = 1 };

            // Executa action passando modelo e, null para indicar que não é pre visualização
            RedirectToRouteResult redirect = (RedirectToRouteResult)controller.New(commentNew, null);

            // Verifica se recebeu corretamente a Action e Controller
            Assert.IsTrue(redirect.RouteValues.ContainsKey("action"));
            Assert.IsTrue(redirect.RouteValues.ContainsKey("controller"));
            // Verifica se os valores redirecionam para página inicial
            Assert.AreEqual("Index", redirect.RouteValues["action"].ToString());
            Assert.AreEqual("Error", redirect.RouteValues["controller"].ToString());
        }

        #endregion
       
        #region DeleteTests

        // Testa se usuário admin consegue deletar comentário de postagem de outro usuario, nao pode
        [TestMethod]
        public void AdminCanDeleteCommentInPostAnotherAuthor_CommentDelete()
        {
            // Cria contexto e autentica usuario Author
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Admin);

            // Cria postagem para outro usuario e insere comentario que sera testado na deleção
            UserDAO userDAO = new UserDAO();
            
            User user = new User() { Username = "Teste", Email = "Teste@teste.com", Name = "Teste", Password = "123456", IsActive = true, IsAdmin = false };
            if (!userDAO.CreateUser(user)) user = userDAO.GetUser("Teste");

            PostDAO postDAO = new PostDAO();
            Post post = new Post() { CreatedBy = user.Id, Content = "Teste", Title = "Teste" };
            postDAO.CreatePost(post);

            CommentDAO commentDAO = new CommentDAO();
            Comment comment = new Comment() { Author = "Teste", Comment1 = "Teste", PostId = post.Id };
            commentDAO.CreateComment(comment);

            // Tenta deletar
            var _result = controller.Delete(new CommentDelete() { CommentId = comment.Id });
            ViewResult result = _result as ViewResult;

            // Verifica view recebeu sucesso
            Assert.IsNotNull(result.ViewBag.Success);

            // Checa se comentário foi deletado
            Assert.IsNull(commentDAO.GetComment(comment.Id));
        }

        // Testa se usuário author consegue deletar comentário de postagem de outro usuario, nao pode
        [TestMethod]
        public void AuthorCantDeleteCommentInPostAnotherAuthor_CommentDelete() {
            // Cria contexto e autentica usuario Author
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Author);

            // Cria postagem para outro usuario e insere comentario que sera testado na deleção
            UserDAO userDAO = new UserDAO();
            User user = new User() { Username = "Teste", Email = "Teste@teste.com", Name = "Teste", Password = "123456", IsActive = true, IsAdmin = false };
            if (!userDAO.CreateUser(user)) user = userDAO.GetUser("Teste");

            PostDAO postDAO = new PostDAO();
            Post post = new Post() { CreatedBy = user.Id, Content = "Teste", Title = "Teste"};
            postDAO.CreatePost(post);
            
            CommentDAO commentDAO = new CommentDAO();
            Comment comment = new Comment() { Author = "Teste",Comment1 = "Teste", PostId = post.Id};
            commentDAO.CreateComment(comment);

            // Tenta deletar
            var _result = controller.Delete(new CommentDelete() { CommentId = comment.Id });
            RedirectToRouteResult result = _result as RedirectToRouteResult;
            
            // Checa se comentário ainda existe
            Assert.IsNotNull(commentDAO.GetComment(comment.Id));

            // Verifica se foi redirecionado para página de erro
            Assert.AreEqual("Error",result.RouteValues["controller"].ToString());
            Assert.AreEqual("Index", result.RouteValues["action"].ToString());
        }

        // Testa se usuário author consegue deletar postagem na propria postagem, tem que poder
        [TestMethod]
        public void AuthorCanDeleteCommentInYourPost_CommentDelete()
        {
            // Cria contexto e autentica usuario Author
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Author);

            // Cria postagem para outro usuario e insere comentario que sera testado na deleção
            UserDAO userDAO = new UserDAO();
            User user = userDAO.GetUser(PermissionHelper.getNameAuthor());
            
            PostDAO postDAO = new PostDAO();
            Post post = new Post() { CreatedBy = user.Id, Content = "Teste", Title = "Teste" };
            postDAO.CreatePost(post);
            
            CommentDAO commentDAO = new CommentDAO();
            Comment comment = new Comment() { Author = "Teste", Comment1 = "Teste", PostId = post.Id };
            commentDAO.CreateComment(comment);

            // Tenta deletar
            ViewResult result = controller.Delete(new CommentDelete() { CommentId = comment.Id }) as ViewResult;

            // Verifica view recebeu sucesso
            Assert.IsNotNull(result.ViewBag.Success);

            // Checa se comentário foi deletado
            Assert.IsNull(commentDAO.GetComment(comment.Id));
        }

        #endregion

        #region PaginationTests

        // Testa paginação com valor nulo
        [TestMethod]
        public void PaginationAll_Null_CommentAll()
        {
            //Atera contexto para usuário autenticado
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Author);
            // Recupera modelo do resultado
            IPagedList model = (controller.All(null) as ViewResult).Model as IPagedList;

            // Testa a página atual, para sucesso deve ser igual a 1
            Assert.AreEqual(model.PageNumber, 1);
        }

        // Testa paginação com valor menor que 1
        [TestMethod]
        public void PaginationAll_LessThanOne_CommentAll()
        {
            //Atera contexto para usuário autenticado
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Author);
            // Recupera modelo do resultado
            IPagedList model = (controller.All(0) as ViewResult).Model as IPagedList;

            // Testa a página atual, para sucesso deve ser igual a 1
            Assert.AreEqual(model.PageNumber, 1);
        }

        // Testa paginação com valor maior que o total de páginas possíveis
        [TestMethod]
        public void PaginationAll_BiggerThenTotalPages_CommentAll()
        {
            //Atera contexto para usuário autenticado
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Author);

            // Recupera modelo do resultado
            IPagedList model = (controller.All(1) as ViewResult).Model as IPagedList;

            // Recupera valor máximo de páginas existentes
            int max = model.PageCount;

            // Recupera modelo do resultado passando parametro acima do maximo
            RedirectToRouteResult redirect = controller.All(max + 1) as RedirectToRouteResult;

            // Checa teste
            Assert.AreEqual(redirect.RouteValues["controller"], "Comment");
            Assert.AreEqual(redirect.RouteValues["action"], "All");
            Assert.AreEqual(1, int.Parse(redirect.RouteValues["page"].ToString()));
        }

        #endregion
    }
}
