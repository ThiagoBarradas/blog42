using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using Blog42.Models;
using Blog42.DataAccess;

namespace Blog42.Security
{
    public class PermissionProvider : RoleProvider
    {
        /*
         * Método que retornará as permissões de um usuário.
         * Para o blog, o usuário possui apenas 3 tipos de permissões;
         * - FirstEntry: Obriga o usuário a resetar sua senha;
         * - Author: Permite a publicação, edição e exclusão de postagem e a exclusão de comentários;
         * - Admin: Permite tudo que o Author faz, acrecentado da criação, edição e exclusão de usuários;
         */
        public override string[] GetRolesForUser(string username)
        {
            // Cria objeto de acesso aos dados e recebe usuário de acordo com o seu username
            UserDAO dao = new UserDAO();
            User user = dao.getUser(username);

            // cria variável local para as permissões
            string[] role;

            // Se consultou e usuário é ativo
            if (user != null && user.IsActive)
            {
                if (user.IsFirstEntry)
                    role = new string[] { "FirstEntry" };
                else if (user.IsAdmin)
                    role = new string[] { "Admin", "Author" };
                else
                    role = new string[] { "Author" };
            }
            else // retorna nenhuma permissão
            {
                // Força o logout 
                FormsAuthentication.SignOut();
                // Atribui nenhuma permissão
                role = new string[] { "" };
            }

            // retorna permissão
            return role;
        }

        /*
         * Os demais métodos não necessitam ser implantados para o funcionamento do sistema
         */
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}