using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Blog42.Security
{
    public class PermissionFilter : AuthorizeAttribute
    {
        /*
         * Método sobrescrito para dizer a aplicação o que fazer se um usuário for negado
         * Durante uma auorização
         */
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            // Se o resultado da autorização for negado
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                // redireciona para a página de login
                filterContext.HttpContext.Response.Redirect("/Admin/User/Login");
            }
        }
    }
}