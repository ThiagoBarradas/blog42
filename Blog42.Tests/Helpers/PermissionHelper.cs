using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Blog42.Tests.Common;

namespace Blog42.Tests.Helpers
{
    class PermissionHelper
    {
        private static MockRoleProvider mock = Roles.Provider as MockRoleProvider;
        private static string currentUser = "";

        // Retorna provider
        public static void Init()
        {
            // Limpa providers
            mock.ClearAll();

            // Popula com roles, usuarios
            mock.CreateUser("marvin");
            mock.CreateUser("arthur");
            mock.CreateRole("Admin");
            mock.CreateRole("Author");
            mock.AddUsersToRoles(new string[] { "arthur" }, new string[] { "Author" });
            mock.AddUsersToRoles(new string[] { "marvin" }, new string[] { "Author" });
            mock.AddUsersToRoles(new string[] { "marvin" }, new string[] { "Admin" });
        }

        // Retorna nome de author para testes
        public static string getNameAuthor()
        {
            return "arthur";
        }

        // Retorna nome de admin para testes
        public static string getNameAdmin()
        {
            return "marvin";
        }

        // Retorna nome usuário atual
        public static string GetCurrentUsername()
        {
            return currentUser;
        }

        // altera nome de usuário atual
        public static void SetCurrentUsername(string username)
        {
            currentUser = username;
        }
    }
}
