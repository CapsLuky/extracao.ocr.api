using System.Net;

namespace API.Contratual.CrossCutting.Notificador;

public class Notificacao
{
    public string Titulo { get; }
    public string Mensagem { get; }
    public TipoNotificacao Tipo { get; }
    public HttpStatusCode StatusCode { get; }

    public Notificacao(string titulo, string mensagem, TipoNotificacao tipo, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        Titulo = titulo;
        Mensagem = mensagem;
        Tipo = tipo;
        StatusCode = statusCode;
    }

    public Notificacao(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }
}