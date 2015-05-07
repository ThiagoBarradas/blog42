using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog42.Models
{
    /*
     * Modelo para view de Login de Usuário
     */
    public class UserLogin
    {
        [Required]
        [Display(Name = "Usuário")]
        [StringLength(20, ErrorMessage = "O campo Usuário deve conter entre 3 e 20 caracteres.", MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Senha")]
        [StringLength(20, ErrorMessage = "O campo Senha deve conter entre 6 e 20 caracteres.", MinimumLength = 6)]
        public string Password { get; set; }
    }

    /*
     * Modelo para view de Primeiro Acesso (Cadastrar Nova Senha) de Usuário
     */
    public class UserResetPassword
    {
        [Required]
        [Display(Name = "Nova Senha")]
        [StringLength(20, ErrorMessage = "O campo Nova Senha deve conter entre 6 e 20 caracteres.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirmar Nova Senha")]
        [Compare("Password", ErrorMessage = "O campo Confirmar Nova Senha deve ser igual ao campo Nova Senha.")]
        public string ConfirmPassword { get; set; }
    }

    /*
    * Modelo para view de deleção de usuário
    */
    public class UserDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public bool Delete { get; set; }
    }


    /*
     * Modelo para criação de Usuário
     */
    public class UserNew
    {
        [Required]
        [Display(Name = "Usuário")]
        [StringLength(20, ErrorMessage = "O campo Usuário deve conter entre 3 e 20 caracteres.", MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Senha")]
        [StringLength(20, ErrorMessage = "O campo Senha deve conter entre 6 e 20 caracteres.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirmar Senha")]
        [Compare("Password", ErrorMessage = "O campo Confirmar Senha deve ser igual ao campo Senha.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O campo Nome deve conter entre 3 e 100 caracteres.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Email inválido!")]
        [StringLength(100, ErrorMessage = "O campo Email deve conter entre 5 e 100 caracteres.", MinimumLength = 5)]
        public string Email { get; set; }

        [Display(Name = "Administrador?")]
        public bool IsAdmin { get; set; }
    }

    /*
     * Modelo para edição de Usuário
     */
    public class UserEdit
    {
        public int Id { get; set; }

        [Display(Name = "Usuário")]
        [StringLength(20, ErrorMessage = "O campo Usuário deve conter entre 3 e 20 caracteres.", MinimumLength = 3)]
        public string Username { get; set; }

        [Display(Name = "Nova Senha (opcional)")]
        [StringLength(20, ErrorMessage = "O campo Nova Senha deve conter entre 6 e 20 caracteres.", MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "Confirmar Nova Senha")]
        [Compare("Password", ErrorMessage = "O campo Confirmar Nova Senha deve ser igual ao campo Senha.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O campo Nome deve conter entre 3 e 100 caracteres.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Email inválido!")]
        [StringLength(100, ErrorMessage = "O campo Email deve conter entre 5 e 100 caracteres.", MinimumLength = 5)]
        public string Email { get; set; }

        [Display(Name = "Administrador?")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Ativo?")]
        public bool IsActive { get; set; }
    }

    /*
     * Modelo completo de Usuário
     */
    public class UserFull
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        [StringLength(20, ErrorMessage = "O campo Usuário deve conter entre 3 e 20 caracteres.", MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Senha")]
        [StringLength(20, ErrorMessage = "O campo Senha deve conter entre 6 e 20 caracteres.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O campo Nome deve conter entre 3 e 100 caracteres.", MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Email inválido!")]
        [StringLength(100, ErrorMessage = "O campo Email deve conter entre 5 e 100 caracteres.", MinimumLength = 5)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Administrador")]
        public bool IsAdmin { get; set; }

        [Required]
        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Primeiro Acesso")]
        public bool IsFirstEntry { get; set; }
    }
}