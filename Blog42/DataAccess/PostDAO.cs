using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog42.Models;

namespace Blog42.DataAccess
{
    public class PostDAO
    {
        private Blog42Entities entities;

        public PostDAO() {}

        // Inicializa entity
        private void Init()
        {
            entities = new Blog42Entities();
        }

        /*
         * Método que retorna uma postagem de acordo com seu id
         * Caso não possua nenhuma postagem com esse id, o retorno é null.
         */
        public Post GetPost(int id)
        {
            Init(); // Atualiza antes de executar operação
            // Captura os dados do comentário e retorna
            var post = entities.Post.Find(id);
            return post;
        }

        /*
         * Cria um novo registro de postagem
         * Recebe um Post e retorna um bool informando sucesso
         */
        public bool CreatePost(Post post)
        {
            Init(); // Atualiza antes de executar operação
            try
            {
                // Atribui o momento atual como data/hora de criação
                post.CreatedAt = DateTime.Now;
                // Inseri e salva as alterações
                entities.Post.Add(post);
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
         * Atualiza um registro de postagem
         * Recebe um Post e retorna um bool informando sucesso
         */
        public bool UpdatePost(Post post)
        {
            Init(); // Atualiza antes de executar operação
            try
            {
                // recebe dados originais
                var original = entities.Post.Find(post.Id);

                // se localizou registro e recuperou dados, atualiza
                if (original != null)
                {
                    // Atualiza dados e salva
                    original.Title = post.Title;
                    original.Content = post.Content;
                    original.CreatedBy = post.CreatedBy;
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
        * Troca todas as postagens de um autor para outro autor
        * Recebe username do autor de origem e de destino e retorna um bool informando sucesso
        */
        public bool ChangeAuthor(string usernameFrom, string usernameTo)
        {
            Init(); // Atualiza antes de executar operação
            try
            {
                // Recebe dados do usuário de destino
                var userTo = entities.User.Where(m => m.Username == usernameTo).Single();
                // Recebe postagens do usuário de origem
                var posts = entities.Post.Where(m => m.User.Username == usernameFrom).ToList<Post>();
                // Altera todos posts do usuário de origem para o usuário de destino
                posts.ForEach(m => m.CreatedBy = userTo.Id);

                //Salva alterações
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
         * Deleta um registro de postagem
         * Recebe um int (id da postagem) e retorna um bool informando sucesso
         */
        public bool DeletePost(int id)
        {
            Init(); // Atualiza antes de executar operação
            try
            {
                // Recebe comentário
                var postSelected = entities.Post.Find(id);
                // Remove comentário e salva
                entities.Post.Remove(postSelected);
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
         * Seleciona todas as Postagens
         * Retorna lista de Postagens
         */
        public List<Post> SelectAllPosts()
        {
            Init(); // Atualiza antes de executar operação
            // Recebe todas as postagens ordenadas pelo Id e retorna em lista
            var posts = entities.Post.OrderByDescending(m => m.Id).ToList<Post>();
            return posts;
        }

        /*
         * Seleciona todas as postagens de um usuário pelo username do usuário
         * Recebe string (username do usuário) e retorna lista de Postagens
         */
        public List<Post> SelectPostsByAuthor(string username)
        {
            Init(); // Atualiza antes de executar operação
            // Recebe todas as postagens de um usuário ordenadas pelo id e retorna em lista
            var posts = entities.Post.Where(m => m.User.Username == username)
                                   .OrderByDescending(m => m.Id)
                                   .ToList<Post>();
            return posts;
        }

    }
}