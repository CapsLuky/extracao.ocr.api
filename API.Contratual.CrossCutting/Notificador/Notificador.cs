using System.Net;

namespace API.Contratual.CrossCutting.Notificador;

public class StatusCodeHttp
{
    public HttpStatusCode StatusCode { get; }

    public StatusCodeHttp(string titulo, string mensagem, TipoNotificacao tipo, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        StatusCode = statusCode;
    }
}

public class Alerta
{
    public string Titulo { get; set; }
    public string Mensagem { get; set; }
    public TipoNotificacao Tipo { get; set; }

    public static implicit operator Alerta(Notificacao notificacao)
    {
        return new Alerta
        {
            Mensagem = notificacao.Mensagem,
            Tipo = notificacao.Tipo,
            Titulo = notificacao.Titulo
        };
    }
}

public class Notificador : INotificador
{
    private readonly List<Alerta> _alertas;

    private readonly List<Notificacao> _notificacoes;
    
    public Notificador()
    {
        _notificacoes = new List<Notificacao>();
    }

    public List<Notificacao> ObterNotificacoes()
    {
        if (_notificacoes == null)
        {
            return new List<Notificacao>();
        }
        
        return _notificacoes;
    }

    public List<Alerta> ObterAlertas()
    {
        if (_notificacoes == null)
        {
            return new List<Alerta>();
        }
        
        var alertas = _notificacoes.Select(n => (Alerta)n).ToList();

        alertas.RemoveAll(alerta => alerta.Mensagem == null);
        
        return alertas;
    }

    public void Handle(Notificacao notificacao)
    {
        _notificacoes.Add(notificacao);
        
    }

    public void ApagarNotificacoes()
    {
        _notificacoes.Clear();
    }
    
}