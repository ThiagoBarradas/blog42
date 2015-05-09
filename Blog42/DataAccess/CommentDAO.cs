using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog42.Models;

namespace Blog42.DataAccess
{
    public class CommentDAO
    {
        private Blog42Entities entities;

        public CommentDAO() 
        { 
            // Inicializa o modelo EF
            entities = new Blog42Entities();
        }

        /*
         * Método que retorna um comentário de acordo com seu id
         * Caso não possua nenhum comentário com esse id, o retorno é null.
         */
        public Comment GetComment(int id)
        {
            // Captura os dados do comentário e retorna
            var comment = entities.Comment.Find(id);
            return comment;
        }

        /*
         * Cria um novo registro de comentário
         * Recebe um Comment e retorna um bool informando sucesso
         */
        public bool CreateComment(Comment comment)
        {
            try
            {
                // Inseri e salva as alterações
                entities.Comment.Add(comment);
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
         * Deleta um registro de comentário
         * Recebe um int (id do comentário) e retorna um bool informando sucesso
         */
        public bool DeleteComment(int id)
        {
            try
            {
                // Recebe comentário
                var commentSelected = entities.Comment.Find(id);
                // Remove comentário e salva
                entities.Comment.Remove(commentSelected);
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
         * Seleciona todos os comentários
         * Retorna lista de Comentários
         */
        public List<Comment> SelectAllComments()
        {
            // Recebe todos os comentários ordenados pelo Id
            var comments = entities.Comment.OrderBy(m => m.Id).ToList<Comment>();
            return comments;
        }


        /*
         * Seleciona todos os comentários de um post pelo id do post
         * Recebe int (id da postagem) e retorna lista de comentários
         */
        public List<Comment> SelectCommentsByPost(int postId)
        {
            // Recebe todos os comentários de um post ordenados pelo id e retorna em lista
            var comments = entities.Comment.Where(m => m.PostId == postId)
                                   .OrderBy(m => m.Id)
                                   .ToList<Comment>();
            return comments;
        }

        /*
         * Seleciona todos os comentários de um usuário pelo username do usuário
         * Recebe string (username do usuário) e retorna lista de comentários
         */
        public List<Comment> SelectCommentsByAuthor(string username)
        {
            // Recebe todos os comentários de um usuário ordenados pelo id e retorna em lista
            var comments = entities.Comment.Where(m => m.Post.User.Username == username)
                                   .OrderBy(m => m.Id)
                                   .ToList<Comment>();
            return comments;
        }

    }
}