using System;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PagedList;
using Moq;
using Blog42;
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
        private CommentNew commentNew;

        // Inicializa controller para testes
        [TestInitialize]
        public void Setup()
        {
            controller = new CommentController();
            commentNew = new CommentNew() // Cria modelo para criação com dados válidos
            {
                Author = "Autor Teste",
                Email = "Teste@gmail.com",
                Comment = "Comentário Teste",
                PostId = 1
            };
        }

        // Testa criação de comentário, request via Ajax POST e Local
        [TestMethod]
        public void SubmitByAjaxPostLocalReturnSuccess_CommentNew()
        {
            // Atribui o contexto passando flags de Ajax, Post e Local
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Ajax, RequestHelper.TypeRequest.Post, RequestHelper.TypeRequest.Local);

            // Executa action passando modelo e, null para indicar que não é pre visualização
            var result = controller.New(commentNew, null);

            // Tem que receber um resultado em Json
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            // Converte o Json para objeto do tipo de modelo de criação e tem que ter a flag IsSuccess como true
            commentNew = ((result as JsonResult).Data as CommentNew);
            Assert.IsTrue(commentNew.IsSuccess);
        }

        // Testa criação de comentário, via Ajax GET e Local
        [TestMethod]
        public void SubmitByGETRedirectError_CommentNew()
        {
            // Atribui o contexto passando apenas flag de request ajax e local (method padrão GET)
            controller.ControllerContext = RequestHelper.BuildHttpContext(RequestHelper.TypeRequest.Ajax, RequestHelper.TypeRequest.Local);

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
            
            // Executa action passando modelo e, null para indicar que não é pre visualização
            RedirectToRouteResult redirect = (RedirectToRouteResult)controller.New(commentNew, null);

            // Verifica se recebeu corretamente a Action e Controller
            Assert.IsTrue(redirect.RouteValues.ContainsKey("action"));
            Assert.IsTrue(redirect.RouteValues.ContainsKey("controller"));
            // Verifica se os valores redirecionam para página inicial
            Assert.AreEqual("Index", redirect.RouteValues["action"].ToString());
            Assert.AreEqual("Error", redirect.RouteValues["controller"].ToString());
        }

        // Testes de Paginação

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

        // Testa se usuário não logado consegue acessar
        [TestMethod]
        public void UserAccess_HttpUnauthorized_CommentAll() {
            AuthorizationContext authorizationContext = RequestHelper.BuildHttpContext() as AuthorizationContext;    
            Security.PermissionFilter permissionFilter = new Security.PermissionFilter();
            permissionFilter.OnAuthorization(authorizationContext);

            //Tem que ser uma requisição não autorizada
            Assert.IsInstanceOfType(authorizationContext.Result,typeof(HttpUnauthorizedResult));
        }
    }
}
