using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog42.Models;
using Blog42.DataAccess;

namespace Blog42.Controllers
{
    public class HomeController : Controller
    {
        /*
         * Início - Postagens
         * GET: /
         */
        public ActionResult Index()
        {
             // Cria e inicializa objeto de acesso aos dados das postagens
            PostDAO postDAO = new PostDAO();
            // Recebe todas as postagens
            List<Post> posts = postDAO.SelectAllPosts().ToList<Post>();
            // Passa listagem para view
            ViewBag.Posts = posts;
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
