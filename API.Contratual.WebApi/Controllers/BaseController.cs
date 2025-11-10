using Microsoft.AspNetCore.Mvc;
using System.Net;
using API.Contratual.CrossCutting.Notificador;

namespace API.Contratual.WebApi.Controllers
{
    [ApiController]
    public class BaseController(INotificador notificador) : Controller
    {
        protected IActionResult CreateResponse(object? result = null)
        {
            var primeiraNotificacao =
                notificador.ObterNotificacoes().FirstOrDefault(n => n.StatusCode != HttpStatusCode.OK);

            if (primeiraNotificacao != null && primeiraNotificacao.StatusCode != HttpStatusCode.OK)
            {
                return new ObjectResult(result)
                {
                    StatusCode = (int)primeiraNotificacao.StatusCode,
                    Value = new
                    {
                        payload = result,
                        alertas = notificador.ObterAlertas()
                    }
                };
            }
            
            //var alertas = _notificador.ObterAlertas();

            if (notificador.ObterNotificacoes().Count != 0)
                return new ObjectResult(result)
                {
                    StatusCode = 200,
                    Value = new
                    {
                        payload = result,
                        alertas = notificador.ObterAlertas()
                    }
                };

            return new ObjectResult(result)
            {
                StatusCode = 200,
                Value = new
                {
                    payload = result,
                    alertas = notificador.ObterAlertas()
                }
            };
        }
    }
}