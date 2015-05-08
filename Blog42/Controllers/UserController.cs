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
        // Cria e inicializa objeto de acesso aos dados
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
        [ValidateAntiForgeryToken()]
        public ActionResult Login(UserLogin userLogin)
        {
            // Verifica se os dados de login são válidos e autentica o usuário passando username e senha criptografada
            if (ModelState.IsValid && userDAO.AuthUser(userLogin.Username, CryptHelper.CryptPassword(userLogin.Password)))
            {
                // Autentica o usuário no sistema e redireciona
                FormsAuthentication.SetAuthCookie(userLogin.Username, false);
                return RedirectLogged();
            }
            else if(ModelState.IsValid)
            {
                // Se o formulário estiver com dados válidos mas não logou, adiciona mensagem de erro
                ModelState.AddModelError("", "Usuário e/ou senha incorretos.");
            }
            return View(userLogin);
        }

        //
        // GET: /Admin/User/Logout
        public ActionResult Logout()
        {
            // Remove a autenticação do usuário
            FormsAuthentication.SignOut();
            return View();
        }

        //
        // GET: /Admin/User/
        [PermissionFilter(Roles = "Admin")]
        public ActionResult All()
        {
            // Recebe todos os usuários, exceto o que está solicitando atual
            List<User> users = userDAO.SelectAllUsers()
                                      .Where(u => u.Username != User.Identity.Name)
                                      .ToList();
            // Passa listagem para view
            ViewBag.Users = users;
            return View();
        }

        //
        // GET: /Admin/User/New
        [PermissionFilter(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        //
        // GET: /Admin/User/New
        [HttpPost]
        [PermissionFilter(Roles = "Admin")]
        [ValidateAntiForgeryToken()]
        public ActionResult New(UserNew userNew)
        {
            // Verifica se Username é válido e se já existe usuário cadastrado com esse Username.
            if(ModelState.IsValidField("Username") && userDAO.GetUser(userNew.Username)!=null) 
            {
                // Se existir, adiciona erro
                ModelState.AddModelError("", "Usuário já existe, escolha outro nome de Usuário.");
            }
            else if (ModelState.IsValid) // Se não existir Username cadastrado e dados do formulário forem válidos
            {
                // Cria usuário e atribui valores a novo usuário
                User user = new User();
                user.Username = userNew.Username;
                user.Password = CryptHelper.CryptPassword(userNew.Password); //atribui senha criptografada
                user.Name = userNew.Name;
                user.Email = userNew.Email;
                user.IsAdmin = userNew.IsAdmin;
                user.IsActive = true; // usuário novo sempre é ativo
                user.IsFirstEntry = true; // usuário novo sempre é primeiro acesso
                    
                // Tenta persistir novo usuário, se conseguir sinaliza sucesso para a view, senão, adiciona mensagem de erro
                if (userDAO.CreateUser(user))
                    ViewBag.Success = true;
                else
                    ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");
            }
            return View(userNew);
        }

        // 
        // GET: /Admin/User/Edit/{id}
        [PermissionFilter(Roles = "Author, Admin")]
        public ActionResult Edit(int id)
        {
            // Declara variáveis locais
            User user;            
            UserEdit userEdit = new UserEdit();

            // Inicializa user. 
            // Verfica se usuário não é Admin (Autor pode somente editar a si mesmo), ou recebeu id 0 (também para editar a si mesmo para Admins)
            if (!Roles.GetRolesForUser().Contains("Admin") || id == 0)
                user = userDAO.GetUser(User.Identity.Name); // Recupera usuário pelo username
            else
                user = userDAO.GetUser(id); // Senão recupera usuário pelo id recebido

            // Se usuário não encontrado, redireciona para página de erro
            if (user == null)
                return RedirectToAction("Index", "Error");

            // Copia as informações recebidas para modelo da página de edição
            userEdit.UserId = user.Id;
            userEdit.Username = user.Username;
            userEdit.Name = user.Name;
            userEdit.Email = user.Email;
            userEdit.IsActive = user.IsActive;
            userEdit.IsAdmin = user.IsAdmin;

            // Se estiver editando seus próprios dados, sinaliza auto edição
            if (user.Username == User.Identity.Name)
                ViewBag.EditMyself = true;
             
            return View(userEdit);
        }

        // 
        // POST: /Admin/User/Edit/{id}
        [HttpPost]
        [PermissionFilter(Roles = "Author, Admin")]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(UserEdit userEdit)
        {
            User user = userDAO.GetUser(userEdit.UserId); // Recupera informações do usuário

            // Se usuário não encontrado, redireciona para página de erro
            if (user == null)
                return RedirectToAction("Index", "Error");

            // Se estiver editando seus próprios dados, sinaliza auto edição
            if (user.Username == User.Identity.Name)
                ViewBag.EditMyself = true;
            else if(!Roles.GetRolesForUser().Contains("Admin")) // Se estiver editando outro usuário, mas não for Admin
                return RedirectToAction("Index","Error"); // Redireciona para página de erro

            // Se dados do formulário são válidos, tenta editar
            if(ModelState.IsValid)
            {
                // Atribui novos valores a usuário editado
                user.Name = userEdit.Name;
                user.Email = userEdit.Email;
                
                // Verifica se é para alterar senha
                if (userEdit.Password != null && userEdit.Password != "")
                {
                    string newPassword = CryptHelper.CryptPassword(userEdit.Password); // Criptografa nova senha
                    // Verifica se senha é igual anterior, se for, adiciona erro e retorna imediatamente
                    if (newPassword == user.Password)
                    {
                        ModelState.AddModelError("", "Você deve cadastrar uma senha diferente da atual.");
                        return View(userEdit);
                    }
                    user.Password = newPassword; // Altera a senha
                }

                // Se não for auto edição, altera dados avançados do usuário
                if (user.Username != User.Identity.Name)
                {
                    user.IsActive = userEdit.IsActive;
                    user.IsAdmin = userEdit.IsAdmin;
                    // Se editou a senha de outro usuário, deve-se solicitar novamente primeiro acesso para a troca da senha
                    if (userEdit.Password != null && userEdit.Password != "")
                        user.IsFirstEntry = true;
                }

                // Tenta editar, se conseguir, sinaliza sucesso, senão, adiciona erro
                if(userDAO.UpdateUser(user))
                    ViewBag.Success = true;
                else
                    ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");
            }
            return View(userEdit);
        }

        //
        // GET: /Admin/User/Delete/{id}
        [PermissionFilter(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            // Recupera informações do usuário a ser deletado
            User user = userDAO.GetUser(id);

            // Se usuário não encontrado, redireciona para página de erro
            if (user == null)
                return RedirectToAction("Index", "Error");

            // Se o próprio usuário tentar deletar ele mesmo, redireciona para listagem de usuários
            if (user.Username == User.Identity.Name)
                return RedirectToAction("All", "User");

            // Copia as informações recebidas para modelo da página de deleção
            UserDelete userDelete = new UserDelete();
            userDelete.Id = user.Id;
            userDelete.Name = user.Name;
            userDelete.Username = user.Username;
            
            return View(userDelete);
        }

        //
        // POST: /Admin/User/Delete/{id}
        [HttpPost]
        [PermissionFilter(Roles = "Admin")]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(UserDelete userDelete)
        {
            // Recupera informações do usuário a ser deletado
            User user = userDAO.GetUser(userDelete.Id);

            // Se usuário não encontrado, redireciona para página de erro
            if (user == null)
                return RedirectToAction("Index", "Error");

            // Se o próprio usuário tentar deletar ele mesmo, redireciona para listagem de usuários
            if (user.Username == User.Identity.Name)
                return RedirectToAction("All", "User");

            // TODO: Implementar POSTDAO
            // Verifica se usuário que está deletando deseja assumir suas publicações
            // if (!userDelete.Delete)
            // {
            //     PostDAO postDAO = new PostDAO(); // Objeto de persistencia de postagens
            //     postDAO.ChangeAuthor(user.Username, User.Identity.Name); // Altera autor das postagens
            // }

            // Tenta deletar, se conseguir, sinaliza sucesso, senão, adiciona erro
            if (userDAO.DeleteUser(userDelete.Id))
                ViewBag.Success = true;
            else
                ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");

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
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(UserResetPassword userResetPassword)
        {
            // Recupera informações do usuário
            User currentUser = userDAO.GetUser(User.Identity.Name);

            // Verifica se ocorreu erro ao obter dados de usuário, e caso tenha ocorrido, faz auto-logout
            if (currentUser == null)
                return RedirectToAction("Logout", "User");

            if (ModelState.IsValid) // Se recebeu dados do usuário e dados do formulário são válidos
            {
                // Criptografa a nova senha                
                string newPassword = CryptHelper.CryptPassword(userResetPassword.Password);

                // Se não houve mudanças na senha, adiciona erro
                if (newPassword == currentUser.Password)
                {
                    ModelState.AddModelError("", "Você deve cadastrar uma senha diferente da atual.");
                }
                else // Se houve mudanças na senha, tenta alterar
                {
                    // Altera senha do usuário e altera flag de primeiro acesso
                    currentUser.Password = newPassword;
                    currentUser.IsFirstEntry = false;

                    // Tenta persistir alterações, se conseguir sinaliza sucesso para view, senão, adiciona erro 
                    if (userDAO.UpdateUser(currentUser))
                        ViewBag.Success = true;
                    else
                        ModelState.AddModelError("", "Ops! Ocorreu um erro durante o processamento. Tente novamente.");
                }
            }            
            return View(userResetPassword);
        }

        #region Helpers
        /*
         * Método que verifica permissão do usuário logado e para onde deve redireciona-lo.
         * 
         */ 
        private ActionResult RedirectLogged()
        {
            // recupera as permissões do usuário
            string[] roles = Roles.GetRolesForUser();

            // Verifica se é primeiro acesso ou não
            if (roles.Contains("FirstEntry")) 
                return RedirectToAction("ResetPassword", "User"); // Redireciona para página de primeiro acesso
            else
                return RedirectToAction("All", "Post"); // Redireciona para Painel Administrativo
        }
        #endregion
    }
}
