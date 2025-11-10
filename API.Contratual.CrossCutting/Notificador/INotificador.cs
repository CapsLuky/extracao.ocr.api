namespace API.Contratual.CrossCutting.Notificador;

public interface INotificador
{
    List<Notificacao> ObterNotificacoes();
    List<Alerta> ObterAlertas();
    void Handle(Notificacao notificacao);
    void ApagarNotificacoes();
}