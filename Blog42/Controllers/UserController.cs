using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Web.Security;
using Blog42.DataAccess;
using Blog42.Models;
using Blog42.Security;

namespace Blog42.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /Account/Login
        public ActionResult Login()
        {
            //Se existe algum usuário já logado, redireciona 
            if (System.Web.HttpContext.Current.User.Identity.Name != "")
                return RedirectLogged();

            // Retorna view de login
            return View();
        }

        //
        // POST: /Account/Login
        //
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Login(UserLogin user)
        {
            // Cria objeto de acesso aos dados
            UserDAO userDAO = new UserDAO();
            // Verifica se os dados para Login são válidos e autentica o usuário
            if (ModelState.IsValid && userDAO.AuthUser(user.Username, user.Password))
            {
                // Autentica o usuário no sistema
                FormsAuthentication.SetAuthCookie(user.Username, false);
                // Redireciona usuário logado
                return RedirectLogged();
            }
            else if(ModelState.IsValid)
            {
                // Se o formulário estiver com dados válidos mas não logou
                // Informa mensagem para o usuário
                ModelState.AddModelError("", "Usuário e/ou senha incorretos.");
            }

            // retorna a view com os dados de origem recebidos do formulário de Login
            return View(user);
        }

        //
        // GET: /Account/Logout
        public ActionResult Logout()
        {
            // Remove a autenticação do usuário
            FormsAuthentication.SignOut();
            return View();
        }

        [PermissionFilter(Roles = "Admin")]
        public ActionResult All()
        {
            return View();
        }

        [PermissionFilter(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [PermissionFilter(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        //
        // GET: /Admin/User/ResetPassword
        [PermissionFilter(Roles = "FirstEntry")]
        public ActionResult ResetPassword()
        {
            return View();
        }

        //
        // POST: /Admin/User/ResetPassword
        [HttpPost]
        [PermissionFilter(Roles = "FirstEntry")]
        public ActionResult ResetPassword(UserResetPassword passwords)
        {
            return View(passwords);
        }

        #region Helpers
        /*
         * Método que verifica se a URL recebida é válida para redirecionar.
         * Caso inválida, redireciona para a página inicial do painel de administração.
         * 
         */ 
        private ActionResult RedirectLogged()
        {
            // recupera as permissões do usuário
            string[] roles = Roles.GetRolesForUser();

            if (roles.Contains("FirstEntry")) // Verifica se é usuário com primeiro acesso
            {
                // redireciona para resetar senha
                return RedirectToAction("ResetPassword", "User");
            }
            else // Se não for primeiro acesso
            {
                // redireciona para painel administrativo
                return RedirectToAction("All", "Post");
            }
        }
        #endregion
    }
}
