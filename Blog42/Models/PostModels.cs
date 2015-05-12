using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog42.Models
{
    
    
    /*
    * Modelo para view de deleção de usuário
    */
    public class PostDelete
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /*
     * Modelo para view de nova postagem
     */
    public class PostNew
    {
        [Required]
        [Display(Name = "Título")]
        [StringLength(100, ErrorMessage = "O campo Título deve conter entre 3 e 100 caracteres.", MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Postagem")]
        [StringLength(8000, ErrorMessage = "O campo Postagem deve conter entre 3 e 8000 caracteres.", MinimumLength = 6)]
        public string Content { get; set; }
    }
}