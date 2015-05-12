using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    /*
    * Modelo para view de criação de comentário
    */
    public class CommentNew
    {
        [Required]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O campo Nome deve conter entre 3 e 100 caracteres.", MinimumLength = 3)]
        public string Author { get; set; }

        [Display(Name = "E-mail (opcional)")]
        [EmailAddress(ErrorMessage = "Email inválido!")]
        [StringLength(100, ErrorMessage = "O campo Email deve conter no máximo 100 caracteres.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Comentário")]
        [StringLength(800, ErrorMessage = "O campo Comentário deve conter entre 3 e 800 caracteres.", MinimumLength = 3)]
        public string Comment { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int PostId { get; set; }
    }
}