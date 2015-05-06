using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog42.Controllers
{
    public class HomeController : Controller
    {
        /*
         * Página inicial
         * GET: / 
         */
        public ActionResult Index()
        {
            return View();
        }

        /*
         * Página Sobre
         * GET: /Sobre 
         */
        public ActionResult Sobre()
        {
            return View();
        }

        /*
         * Página de Contatos
         * GET: /Contato
         */
        public ActionResult Contato()
        {
            return View();
        }
    }
}
