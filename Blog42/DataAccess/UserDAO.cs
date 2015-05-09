using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog42.Models;

namespace Blog42.DataAccess
{
    public class UserDAO
    {
        private Blog42Entities entities;

        public UserDAO() 
        { 
            // Inicializa o modelo EF
            entities = new Blog42Entities();
        }

        /*
         * Verifica se o usuário existe, a senha está correta e é um usuário ativo.
         * Se estiver tudo ok, retorna true, caso usuário ou senha incorretos ou usuário inativo, retorna false.
         */
        public bool AuthUser(String username, String password)
        {
            //Define a query de consulta para a combinação de usuário/senha e para usuários ativos
            var query = (from u in entities.User
                         where u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password && u.IsActive
                         select u).SingleOrDefault();

            // Retorna true caso recupere um usuário ativo que o username e a senha combinem ou false caso contrário.
            return (query != null);
        }

        /*
         * Método que retorna um usuário de acordo com seu username
         * Caso não possua nenhum usuário com username, o retorno é null.
         */
        public User GetUser(string username)
        {
            // captura os dados do usuário e retorna
            User user =  (from u in entities.User
                         where u.Username == username
                         select u).SingleOrDefault();
            return user;
        }

        /*
         * Método que retorna um usuário de acordo com seu id
         * Caso não possua nenhum usuário com esse id, o retorno é null.
         */
        public User GetUser(int id)
        {
            // captura os dados do usuário e retorna
            var user = entities.User.Find(id);
            return user;
        }

        /*
         * Cria um novo registro de usuário
         * Recebe um User e retorna um bool informando sucesso
         */
        public bool CreateUser(User user)
        {
            try
            {
                // Inseri e salva as alterações
                entities.User.Add(user);
                entities.SaveChanges();
                // Se ocorrer tudo bem, retorna true
                return true;
            }
            catch (Exception)
            {
                // Caso ocorra algum problema retorna false
                return false;
            }
        }

        /*
         * Atualiza um registro de usuário
         * Recebe um User e retorna um bool informando sucesso
         */
        public bool UpdateUser(User user)
        {
            try
            {
                // recebe dados originais
                var original = entities.User.Find(user.Id);

                // se localizou registro e recuperou dados, atualiza
                if (original != null)
                {
                    // Atualiza dados e salva
                    original.Username = user.Username;
                    original.Name = user.Name;
                    original.Email = user.Email;
                    original.Password = user.Password;
                    original.IsActive = user.IsActive;
                    original.IsAdmin = user.IsAdmin;
                    original.IsFirstEntry = user.IsFirstEntry;
                    entities.SaveChanges();
                    // Se ocorrer tudo bem, retorna true
                    return true;
                }
                else // Se não localizou retorna false
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // Caso ocorra algum problema retorna false
                return false;
            }
        }

        /*
         * Deleta um registro de usuário
         * Recebe um int (id do usuário) e retorna um bool informando sucesso
         */
        public bool DeleteUser(int id)
        {
            try
            {
                // Recebe usuário 
                var userSelected = entities.User.Find(id);
                // Remove usuário e salva
                entities.User.Remove(userSelected);
                entities.SaveChanges();
                // Se ocorrer tudo bem, retorna true
                return true;                
            }
            catch (Exception)
            {
                // Caso ocorra algum problema retorna false
                return false;
            }
        }

        /*
         * Seleciona todos os usuários
         * Recebe uma lista de User
         */
        public List<User> SelectAllUsers()
        {
            // Recebe todos os usuários ordenados pelo nome e retorna em lista
            var users = entities.User.OrderBy(m => m.Name).ToList<User>();
            return users;
        }

    }
}