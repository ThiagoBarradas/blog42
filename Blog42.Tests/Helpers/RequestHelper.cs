using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Security.Principal;
using Moq;

namespace Blog42.Tests.Helpers
{
    class RequestHelper
    {
        // Enumerador para tipo de HTTP Request
        public enum TypeRequest
        {
            Ajax,
            Post,
            Local,
            Admin,
            Author
        };

        // Cria contexto para simular requisição HTTP
        // Recebe o(s) tipo(s) de requisição(ões) que deseja simular usando o Enum TypeRequest
        public static ControllerContext BuildHttpContext(params TypeRequest[] type)
        {
            // Inicializa mockings
            var controllerContext = new Mock<ControllerContext>();
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
			var response = new Mock<HttpResponseBase>();
			var session = new Mock<HttpSessionStateBase>();
			var server = new Mock<HttpServerUtilityBase>();
            var actionDescriptor = new Mock<ActionDescriptor>();
            context.Setup(c => c.Response).Returns(response.Object);
			context.Setup(c => c.Session).Returns(session.Object);
			context.Setup(c => c.Server).Returns(server.Object);
            context.Setup(c => c.Items).Returns(new Mock<IDictionary>().Object);
            context.Setup(c => c.Response.Cache).Returns(new Mock<HttpCachePolicyBase>().Object);

            // Cria coleção de opções para cabeçalho HTTP
            WebHeaderCollection webHeader = new WebHeaderCollection();

            // Verifica flag de Ajax
            if(type.Contains(TypeRequest.Ajax)) 
            { 
                webHeader.Set("X-Requested-With", "XMLHttpRequest" ); 
            }

            // Verifica flag de post
            if (type.Contains(TypeRequest.Post))
            {
                request.SetupGet(x => x.HttpMethod).Returns("POST");
            }
            else
            {
                request.SetupGet(x => x.HttpMethod).Returns("GET");
            }

            // Verifica flag locals
            if(type.Contains(TypeRequest.Local)) 
            { 
                request.SetupGet(x => x.IsLocal).Returns(true);
            }

            // Verifica flag author e admin
            if (type.Contains(TypeRequest.Author) || type.Contains(TypeRequest.Admin))
            {
                // Inicializa helper para provider
                PermissionHelper.Init();
                // Cria uma identidade simulada
                var fakeIdentity = new GenericIdentity(
                        (type.Contains(TypeRequest.Admin)) ? PermissionHelper.getNameAdmin() : PermissionHelper.getNameAuthor()
                    );
                var principal = new GenericPrincipal(fakeIdentity, null);
                // Altera no contexto
                context.Setup(t => t.User).Returns(principal);

                // Seta (fake) usuário atual 
                PermissionHelper.SetCurrentUsername((type.Contains(TypeRequest.Admin)) ? PermissionHelper.getNameAdmin() : PermissionHelper.getNameAuthor());
            }
            else
            {
                var fakeIdentity = new GenericIdentity(String.Empty);
                var principal = new GenericPrincipal(fakeIdentity, null);
                // Altera no contexto
                context.Setup(t => t.User).Returns(principal);
                
                // Seta (fake) usuário atual 
                PermissionHelper.SetCurrentUsername(String.Empty);
            }

            // Atribui parâmetros para request que será definida no contexto
            request.SetupGet(x => x.Headers).Returns(webHeader);

            // Configura o contexto para na requisição retornar 'request' criado anteriormente
            context.SetupGet(x => x.Request).Returns(request.Object);

            controllerContext.Setup(c => c.HttpContext).Returns(context.Object);
            actionDescriptor.Setup(d => d.ControllerDescriptor).Returns(new Mock<ControllerDescriptor>().Object);
            return new AuthorizationContext(controllerContext.Object, actionDescriptor.Object);
            //return context.Object;
        }
    }
}
