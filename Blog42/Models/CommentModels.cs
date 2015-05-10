using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog42.Models
{

    /*
     * Modelo para view de deleção de comentário
     */
    public class CommentDelete
    {
        public int CommentId { get; set; }
        public string Comment { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public string PostTitle { get; set; }
    }
}