using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
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
        public ActionResult Index(int? page)
        {            
            // Cria e inicializa objeto de acesso aos dados das postagens
            PostDAO postDAO = new PostDAO();
            // Recebe todas as postagens
            IQueryable<Post> posts = postDAO.SelectAllPosts().AsQueryable();
            // Cria modelo com paginação de 5 itens por página
            IPagedList<Post> model = posts.ToPagedList(page ?? 1, 5);
            
            // Se página inválida
            if (page != null && page > 1 && model.Count == 0)
                return RedirectToAction("Index", "Home"); //Redireciona para o início

            return View(model);
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
