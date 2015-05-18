using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PagedList;
using Moq;
using Blog42;
using Blog42.Models;
using Blog42.Controllers;
using Blog42.Tests.Helpers;

namespace Blog42.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : Common.GeneralSetup
    {
        private HomeController controller;
        
        // Inicializa controller para testes
        [TestInitialize]
        public void SetupController() {
            controller = new HomeController();
        }

        ///<summary>
        ///Testa paginação com valor nulo, deve obter a primeira página
        ///</summary>
        [TestMethod]
        public void PaginacaoNula_Index()
        {
            // Inicia pagina inicial com pagina nula e string de busca vazia
            ViewResult result = (ViewResult)controller.Index(null, "");

            // Recupera modelo do resultado
            IPagedList<Post> model = (IPagedList<Post>)result.Model;

            // Testa a página atual, com nulo deve ser página 1 obrigatoriamente
            Assert.AreEqual(model.PageNumber,1);
        }

        ///<summary>
        ///Testa paginação com valor menor que 1, deve obter a primeira página
        ///</summary>
        [TestMethod]
        public void PaginacaoMenorQue1_Index()
        {
            // Inicia pagina inicial com pagina menor que 1 e string de busca vazia
            ViewResult result = (ViewResult)controller.Index(0, "");

            // Recupera modelo do resultado
            IPagedList<Post> model = (IPagedList<Post>)result.Model;

            // Testa a página atual, com valor menor que 1 deve ser página 1 obrigatoriamente
            Assert.AreEqual(model.PageNumber, 1);
        }

        ///<summary>
        ///Testa paginação com valor maior que o total de páginas possíveis, deve obter a primeira página
        ///</summary>
        [TestMethod]
        public void PaginacaoMaiorQueTotalDePaginas_Index()
        {
            // Inicia pagina inicial valor padrão para recuperar numero máximo de páginas
            ViewResult result = (ViewResult)controller.Index(1, "");

            // Recupera modelo do resultado
            IPagedList<Post> model = (IPagedList<Post>)result.Model;
            
            // Recupera valor máximo de páginas existentes
            int max = model.PageCount;

            // Executa novamente a action passando um valor acima do máximo existente, deve receber um redirecionamento para a página inicial sem parâmetros
            RedirectToRouteResult redirect  = (RedirectToRouteResult)controller.Index(max+1, "");

            // Verifica se recebeu corretamente a Action e Controller
            Assert.IsTrue(redirect.RouteValues.ContainsKey("action")); 
            Assert.IsTrue(redirect.RouteValues.ContainsKey("controller"));
            // Verifica se os valores redirecionam para página inicial
            Assert.AreEqual("Index", redirect.RouteValues["action"].ToString());
            Assert.AreEqual("Home", redirect.RouteValues["controller"].ToString());
            // E se a página referenciada é a página 1
            Assert.AreEqual(1, int.Parse(redirect.RouteValues["page"].ToString()));
        }

        ///<summary>
        ///Testa pesquisa com valor nulo, não deve aplicar filtro
        ///</summary>
        [TestMethod]
        public void PesquisaNula_Index()
        {
            // Inicia pagina inicial com pagina 1 e string de busca nula
            ViewResult result = (ViewResult)controller.Index(1, null);

            // Recupera valor da busca
            string search = (string)result.ViewBag.search;

            // Testa se a busca é vazia
            Assert.AreEqual(search, "");
        }
    }
}
