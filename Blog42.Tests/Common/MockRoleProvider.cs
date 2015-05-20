using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration.Provider;
using System.Web.Security;
using System.Text;
using System.Threading.Tasks;

namespace Blog42.Tests.Common
{
    public class MockRoleProvider : RoleProvider
    {
        public MockRoleProvider()
            : base()
        {
            Users = new List<User>();
            Roles = new List<Role>();
        }

        #region RoleProvider members
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (usernames == null) throw new ArgumentNullException("usernames");
            if (roleNames == null) throw new ArgumentNullException("roleNames");

            foreach (string role in roleNames)
            {
                if (!RoleExists(role)) throw new ProviderException("Role name does not exist.");
            }

            foreach (string user in usernames)
            {
                if (Users.FirstOrDefault(u => u.Username == user) == null) throw new ProviderException("Username does not exist.");
            }

            foreach (string username in usernames)
            {
                User user = Users.FirstOrDefault(u => u.Username == username);
                if (user == null) continue;
                foreach (var rolename in roleNames)
                {
                    Role role = Roles.FirstOrDefault(r => r.RoleName == rolename);
                    user.Roles.Add(role);
                    role.Users.Add(user);
                }
            }
        }

        public override string ApplicationName { get; set; }

        public override void CreateRole(string roleName)
        {
            if (roleName == null || roleName == "") throw new ProviderException("Role name cannot be empty or null.");
            if (roleName.Contains(",")) throw new ArgumentException("Role names cannot contain commas.");
            if (RoleExists(roleName)) throw new ProviderException("Role name already exists.");

            Roles.Add(new Role { RoleName = roleName });
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (roleName == null || roleName == "") throw new ProviderException("Role name cannot be empty or null.");

            Role role = Roles.FirstOrDefault(r => r.RoleName == roleName);

            if (role == null) throw new ProviderException("Role name does not exist.");
            if (throwOnPopulatedRole && GetUsersInRole(roleName).Length > 0) throw new ProviderException("Cannot delete a populated role.");

            Roles.Remove(Roles.FirstOrDefault(r => r.RoleName == roleName));
            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return GetUsersInRole(roleName).Where(n => n.Contains(usernameToMatch)).ToArray();
        }

        public override string[] GetAllRoles()
        {
            return Roles.Select(r => r.RoleName).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            username = Helpers.PermissionHelper.GetCurrentUsername();

            if (username == null || username == "")
            {
                FormsAuthentication.SignOut();
                return new string[0];
            }

            User user = Users.FirstOrDefault(u => u.Username == username);

            if (user == null) return new string[0];

            return user.Roles.Select(r => r.RoleName).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (roleName == null || roleName == "") throw new ProviderException("Role name cannot be empty or null.");

            Role role = Roles.FirstOrDefault(r => r.RoleName == roleName);

            if (role == null) throw new ProviderException("Role '" + roleName + "' does not exist.");

            return role.Users.Select(u => u.Username).OrderBy(n => n).ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (username == null || username == "") throw new ProviderException("User name cannot be empty or null.");
            if (roleName == null || roleName == "") throw new ProviderException("Role name cannot be empty or null.");

            Role role = Roles.FirstOrDefault(r => r.RoleName == roleName);

            return role != null && role.Users.FirstOrDefault(u => u.Username == username) != null;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (string roleName in roleNames)
            {
                if (roleName == null || roleName == "") throw new ProviderException("Role name cannot be empty or null.");
                if (!RoleExists(roleName)) throw new ProviderException("Role name not found.");
            }

            foreach (string username in usernames)
            {
                if (username == null || username == "") throw new ProviderException("User name cannot be empty or null.");

                foreach (string roleName in roleNames)
                {
                    if (!IsUserInRole(username, roleName)) throw new ProviderException("User is not in role.");
                }
            }

            foreach (string username in usernames)
            {
                User user = Users.FirstOrDefault(u => u.Username == username);
                if (user == null) continue;
                foreach (string roleName in roleNames)
                {
                    Role role = user.Roles.FirstOrDefault(r => r.RoleName == roleName);
                    role.Users.Remove(user);
                    user.Roles.Remove(role);
                }
            }
        }

        public override bool RoleExists(string roleName)
        {
            if (roleName == null || roleName == "") throw new ProviderException("Role name cannot be empty or null.");

            return Roles.FirstOrDefault(r => r.RoleName == roleName) != null;
        }
        #endregion

        public void ClearAll()
        {
            Users = new List<User>();
            Roles = new List<Role>();
        }

        public void ClearRoles()
        {
            Roles = new List<Role>();
            Users.ForEach(u => u.Roles = new List<Role>());
        }

        public void ClearUsers()
        {
            Users = new List<User>();
            Roles.ForEach(r => r.Users = new List<User>());
        }

        public void CreateUser(string username)
        {
            if (username == null || username == "") throw new ProviderException("User name cannot be empty or null.");
            if (UserExists(username)) throw new ProviderException("User name already exists.");

            Users.Add(new User { Username = username });
        }

        public bool DeleteUser(string username)
        {
            if (username == null || username == "") throw new ProviderException("User name cannot be empty or null.");

            User user = Users.FirstOrDefault(u => u.Username == username);

            if (user == null) throw new ProviderException("User name does not exist.");

            foreach (Role role in user.Roles)
            {
                role.Users.Remove(user);
            }
            Users.Remove(user);
            return true;
        }

        public bool UserExists(string username)
        {
            if (username == null || username == "") throw new ProviderException("User name cannot be empty or null.");

            return Users.FirstOrDefault(u => u.Username == username) != null;
        }

        private List<Role> Roles { get; set; }

        private List<User> Users { get; set; }

        private class Role
        {
            public Role()
            {
                Users = new List<User>();
            }

            public string RoleName { get; set; }
            public List<User> Users { get; set; }
        }

        private class User
        {
            public User()
            {
                Roles = new List<Role>();
            }

            public string Username { get; set; }
            public List<Role> Roles { get; set; }
        }
    }
}
