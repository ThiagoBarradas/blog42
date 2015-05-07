using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using System.Web.Security;
using Blog42.DataAccess;
using Blog42.Models;
using Blog42.Security;
using Blog42.Helpers;

namespace Blog42.Controllers
{
    public class UserController : Controller
    {
        // Cria objeto de acesso aos dados
        private UserDAO userDAO = new UserDAO();

        //
        // GET: /Admin/User/Login
        public ActionResult Login()
        {
            //Se existe algum usuário já logado, redireciona 
            if (User.Identity.Name != "")
                return RedirectLogged();

            // Retorna view de login
            return View();
        }

        //
        // POST: /Admin/User/Login
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Login(UserLogin user)
        {
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
        // GET: /Admin/User/Logout
        public ActionResult Logout()
        {
            // Remove a autenticação e sessão do usuário
            FormsAuthentication.SignOut();
            return View();
        }

        //
        // GET: /Admin/User/
        [PermissionFilter(Roles = "Admin")]
        public ActionResult All()
        {
            // Recebe todos os usuários, menos o atual
            List<User> users = userDAO.SelectAllUsers().Where(u => u.Username != User.Identity.Name).ToList();

            // Passa para view
            ViewBag.Users = users;
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

        //
        // GET: /Admin/User/Delete/{id}
        [PermissionFilter(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            // recupera dados do usuário
            User user = userDAO.GetUser(id);
            
            // Passa as informações que serão usadas na view para o modelo
            UserDelete userDelete = new UserDelete();
            userDelete.Id = user.Id;
            userDelete.Name = user.Name;
            userDelete.Username = user.Username;
            
            // Se usuário não encontrado, pagina de erro
            if (user == null)
                return RedirectToAction("Index", "Error");

            // Se o próprio usuário tentar deletar ele mesmo, redireciona para listagem de usuários
            if (user.Username == User.Identity.Name)
                return RedirectToAction("All","User");

            // Passa modelo de usuário de deleção para view
            return View(userDelete);
        }

        //
        // POST: /Admin/User/Delete/{id}
        [HttpPost]
        [PermissionFilter(Roles = "Admin")]
        public ActionResult Delete(UserDelete userDelete)
        {
            // recupera dados do usuário
            User user = userDAO.GetUser(userDelete.Id);

            // Se usuário não encontrado, pode ser um refresh, pagina de erro
            if (user == null)
                return RedirectToAction("Index", "Error");

            // Se o próprio usuário tentar deletar ele mesmo, redireciona para listagem de usuários
            if (user.Username == User.Identity.Name)
                return RedirectToAction("All", "User");
            
            // Tenta deletar, se conseguir, sinaliza sucesso, senão, mensagem de erro.
            if (userDAO.DeleteUser(userDelete.Id))
                @ViewBag.Success = true;
            else
                ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");

            // Retorna view e modelo
            return View(userDelete);
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
            // Recupera informações originais
            User user = userDAO.GetUser(User.Identity.Name);

            // verifica se não conseguiu receber usuário
            if (user == null)
            {
                // Desloga e redireciona para página de login
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "User");
            }
            else if(ModelState.IsValid) // Se recebeu usuário e dados são válidos
            { 
                // criptografa a nova senha                
                string newpassword = CryptHelper.CryptPassword(passwords.Password);

                // Se a nova senha for igual a antiga, retorna erro
                if (newpassword == user.Password)
                {
                    ModelState.AddModelError("", "Você deve cadastrar uma senha diferente da atual.");
                }
                else
                {
                    // atribui nova senha e seta primeiro acesso como false
                    user.Password = newpassword;
                    user.IsFirstEntry = false;

                    // tenta atualizar, se não conseguir informa erro 
                    if (userDAO.UpdateUser(user))
                        ViewBag.Success = true;
                    else
                        ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");
                }
            }            
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
