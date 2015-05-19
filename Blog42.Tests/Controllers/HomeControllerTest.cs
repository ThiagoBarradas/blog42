using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PagedList;
using Blog42;
using Blog42.Models;
using Blog42.Controllers;
using Blog42.Tests.Common;

namespace Blog42.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : Common.GeneralSetup
    {
        private HomeController controller;
        
        // Inicializa controller para testes
        [TestInitialize]
        public void Setup() {
            controller = new HomeController();
        }

        // Testa pesquisa com valor nulo
        [TestMethod]
        public void Search_Null_HomeIndex()
        {
            // Inicia pagina inicial com pagina 1 e string de busca nula
            ViewResult result = (ViewResult)controller.Index(1, null);

            // Recupera valor da busca
            string search = (string)result.ViewBag.search;

            // Testa se a busca é vazia
            Assert.AreEqual(search, "");
        }

        // Testa paginação com valor nulo
        [TestMethod]
        public void PaginationAll_Null_HomeIndex()
        {
            // Recupera modelo do resultado
            IPagedList model = (controller.Index(null, "") as ViewResult).Model as IPagedList;

            // Testa a página atual, para sucesso deve ser igual a 1
            Assert.AreEqual(model.PageNumber, 1);
        }

        // Testa paginação com valor menor que 1
        [TestMethod]
        public void PaginationAll_LessThanOne_HomeIndex()
        {
            // Recupera modelo do resultado
            IPagedList model = (controller.Index(0, "") as ViewResult).Model as IPagedList;

            // Testa a página atual, para sucesso deve ser igual a 1
            Assert.AreEqual(model.PageNumber, 1);
        }

        // Testa paginação com valor maior que o total de páginas possíveis
        [TestMethod]
        public void PaginationAll_BiggerThenTotalPages_HomeIndex()
        {
            // Recupera modelo do resultado
            IPagedList model = (controller.Index(1, "") as ViewResult).Model as IPagedList;

            // Recupera valor máximo de páginas existentes
            int max = model.PageCount;

            // Recupera modelo do resultado passando parametro acima do maximo
            RedirectToRouteResult redirect = controller.Index(max+1, "") as RedirectToRouteResult;

            // Checa teste
            Assert.AreEqual(redirect.RouteValues["controller"], "Home");
            Assert.AreEqual(redirect.RouteValues["action"], "Index");
            Assert.AreEqual(1, int.Parse(redirect.RouteValues["page"].ToString()));
        }
    }

}
