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
    public class PostControllerTest : Common.GeneralSetup
    {
        private PostController controller;

        // Inicializa controller para testes
        [TestInitialize]
        public void Setup()
        {
            controller = new PostController();
        }

        #region DeleteTests

        // Testa se usuário admin consegue deletar postagem de outro usuario, deve poder
        [TestMethod]
        public void AdminCanDeletePostAnotherAuthor_PostDelete()
        {
            // Cria contexto e autentica usuario Author
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Admin);

            // Cria postagem para outro usuario e insere post que sera testado na deleção
            UserDAO userDAO = new UserDAO();

            User user = new User() { Username = "Teste", Email = "Teste@teste.com", Name = "Teste", Password = "123456", IsActive = true, IsAdmin = false };
            if (!userDAO.CreateUser(user)) user = userDAO.GetUser("Teste");

            PostDAO postDAO = new PostDAO();
            Post post = new Post() { CreatedBy = user.Id, Content = "Teste", Title = "Teste" };
            postDAO.CreatePost(post);

            // Tenta deletar
            ViewResult result = controller.Delete(new PostDelete() { PostId = post.Id }) as ViewResult;

            // Verifica view recebeu sucesso
            Assert.IsNotNull(result.ViewBag.Success);

            // Checa se post foi deletado
            Assert.IsNull(postDAO.GetPost(post.Id));

        }

        // Testa se usuário author consegue deletar postagem de outro usuario, nao pode
        [TestMethod]
        public void AuthorCantDeletePostAnotherAuthor_PostDelete() {
            // Cria contexto e autentica usuario Author
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Author);

            // Cria postagem para outro usuario e insere post que sera testado na deleção
            UserDAO userDAO = new UserDAO();
            User user = new User() { Username = "Teste", Email = "Teste@teste.com", Name = "Teste", Password = "123456", IsActive = true, IsAdmin = false };
            if (!userDAO.CreateUser(user)) user = userDAO.GetUser("Teste");

            PostDAO postDAO = new PostDAO();
            Post post = new Post() { CreatedBy = user.Id, Content = "Teste", Title = "Teste"};
            postDAO.CreatePost(post);
            
            // Tenta deletar
            RedirectToRouteResult result = controller.Delete(new PostDelete() { PostId = post.Id }) as RedirectToRouteResult;
            
            // Checa se post ainda existe
            Assert.IsNotNull(postDAO.GetPost(post.Id));

            // Verifica se foi redirecionado para página de erro
            Assert.AreEqual("Error",result.RouteValues["controller"].ToString());
            Assert.AreEqual("Index", result.RouteValues["action"].ToString());
        }

        // Testa se usuário author consegue deletar propria postagem, tem que poder
        [TestMethod]
        public void AuthorCanDeleteYourPost_PostDelete()
        {
            // Cria contexto e autentica usuario Author
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Author);

            // Cria postagem para outro usuario e insere post que sera testado na deleção
            UserDAO userDAO = new UserDAO();
            User user = userDAO.GetUser(PermissionHelper.getNameAuthor());
            
            PostDAO postDAO = new PostDAO();
            Post post = new Post() { CreatedBy = user.Id, Content = "Teste", Title = "Teste" };
            postDAO.CreatePost(post);

            // Tenta deletar
            ViewResult result = controller.Delete(new PostDelete() { PostId = post.Id }) as ViewResult;

            // Verifica view recebeu sucesso
            Assert.IsNotNull(result.ViewBag.Success);

            // Checa se post foi deletado
            Assert.IsNull(postDAO.GetPost(post.Id));
        }
        
        #endregion

        #region PaginationTests

        // Testa paginação com valor nulo
        [TestMethod]
        public void PaginationAll_Null_PostAll()
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
        public void PaginationAll_LessThanOne_PostAll()
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
        public void PaginationAll_BiggerThenTotalPages_PostAll()
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
            Assert.AreEqual(redirect.RouteValues["controller"], "Post");
            Assert.AreEqual(redirect.RouteValues["action"], "All");
            Assert.AreEqual(1, int.Parse(redirect.RouteValues["page"].ToString()));
        }

        #endregion

    }
        
}