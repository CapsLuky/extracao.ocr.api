using API.Contratual.CrossCutting.Common;

namespace API.Contratual.Domain.Entity;

public class ArquivoPaginaEntity
{
    public Guid Id { get;  set; }
    public Guid ArquivoId { get;  set; }
    public short NumeroPagina { get;  set; }
    public string ConteudoTexto { get;  set; }
    public DateTime DataCriacao { get;  set; }
    public DateTime? DataAtualizacao { get;  set; }

  
    protected ArquivoPaginaEntity(){}
    
    public static class ArquivoPaginaFactory
    {
        public static ArquivoPaginaEntity NovoArquivo(Guid arquivoId, short numeroPagina, string conteudoTexto)
        {
            var arquivo = new ArquivoPaginaEntity
            {
                Id = Guid.CreateVersion7(),
                ArquivoId = arquivoId,
                NumeroPagina = numeroPagina,
                ConteudoTexto = conteudoTexto,
                DataCriacao = DateTime.UtcNow.ToDateTimeBrazil()
            };

            return arquivo;
        }
    }
}