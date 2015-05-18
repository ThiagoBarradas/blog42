using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;

namespace Blog42.Tests.Helpers
{
    class RequestHelper
    {
        // Enumerador para tipo de HTTP Request
        public enum TypeRequest
        {
            AjaxPost,
            AjaxOnly,
            PostOnly,
        };

        // Cria contexto para simular requisição HTTP
        // Recebe o tipo de requisição que deseja simular usando o Enum TypeRequest
        public static HttpContextBase BuildHttpContext(TypeRequest type)
        {
            // Inicializa mocking para request e contexto
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();

            // Define o tipo de requisição que deseja criar e atribui parâmetros para request
            switch(type) {
                case TypeRequest.AjaxOnly:
                    request.SetupGet(x => x.Headers).Returns(
                        new System.Net.WebHeaderCollection {
                            {"X-Requested-With", "XMLHttpRequest"}
                        });
                    break;
                case TypeRequest.PostOnly:
                    request.SetupGet(x => x.Headers).Returns(
                        new System.Net.WebHeaderCollection {
                            {"Method", "POST"}
                        });
                    break;
                case TypeRequest.AjaxPost:
                    request.SetupGet(x => x.Headers).Returns(
                        new System.Net.WebHeaderCollection {
                            {"X-Requested-With", "XMLHttpRequest"},
                            {"Method", "POST"}
                        });
                    break;
            }

            // Configura o contexto para na requisição retornar request criado anteriormente
            context.SetupGet(x => x.Request).Returns(request.Object);
 
            // Retorna contexto
            return context.Object;
        }
    }
}
