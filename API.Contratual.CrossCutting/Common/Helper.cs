using API.Contratual.CrossCutting.Common.Model;

namespace API.Contratual.CrossCutting.Common;

public static class Helper
{
    public static string FormatarNomePasta(string nomePasta)
    {
        if (string.IsNullOrWhiteSpace(nomePasta))
            return string.Empty;

        const int maxLength = 100;

        // Caracteres inválidos em ambos os sistemas
        char[] caracteresInvalidos = Path.GetInvalidFileNameChars()
            .Concat(Path.GetInvalidPathChars())
            .Concat(new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|', ' ' })
            .Distinct()
            .ToArray();

        // Substitui caracteres inválidos por underscore
        string nomeFormatado = caracteresInvalidos.Aggregate(nomePasta, (current, c) => current.Replace(c, '_'));
    
        // Remove underscores múltiplos
        nomeFormatado = string.Join("_", nomeFormatado.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries));
    
        // Remove espaços em branco
        nomeFormatado = nomeFormatado.Replace(" ", "");

        // Verifica o tamanho máximo
        if (nomeFormatado.Length > maxLength)
        {
            throw new StringLengthExceededException("nome_original", maxLength, nomeFormatado.Length);
        }

        return nomeFormatado;
    }
    
    // public static (bool válido, List<CaminhoExcedidoInfo> excedidos) ValidarTamanhosCaminhos(List<string> caminhos)
    // {
    //     const int MAX_NOME = 100;
    //     const int MAX_CAMINHO = 250;
    //     var excedidos = new List<CaminhoExcedidoInfo>();
    //
    //     foreach (var caminho in caminhos)
    //     {
    //         var nomeArquivo = Path.GetFileName(caminho);
    //         var caminhoFisico = Path.GetDirectoryName(caminho);
    //
    //         if (nomeArquivo.Length > MAX_NOME || caminhoFisico?.Length > MAX_CAMINHO)
    //         {
    //             excedidos.Add(new CaminhoExcedidoInfo
    //             {
    //                 CaminhoCompleto = caminho,
    //                 NomeArquivo = nomeArquivo,
    //                 TamanhoNome = nomeArquivo.Length,
    //                 CaminhoFisico = caminhoFisico ?? string.Empty,
    //                 TamanhoCaminho = caminhoFisico?.Length ?? 0
    //             });
    //         }
    //     }
    //
    //     return (excedidos.Count == 0, excedidos);
    // }

    public static string ValidarCaminhoFisico(string caminhoFisico)
    {
        if (string.IsNullOrWhiteSpace(caminhoFisico))
            return string.Empty;

        const int maxLength = 250;
        if (caminhoFisico.Length > maxLength)
        {
            throw new StringLengthExceededException("caminho_fisico", maxLength, caminhoFisico.Length);
        }

        return caminhoFisico;
    }
    
    public static (bool válido, List<ArquivoValidacaoInfo> excedidos) ValidarTamanhosCaminhos(List<string> caminhos)
    {
        var excedidos = new List<ArquivoValidacaoInfo>();

        foreach (var caminho in caminhos)
        {
            var nomeArquivo = Path.GetFileName(caminho);
            var caminhoFisico = Path.GetDirectoryName(caminho);
            var fileInfo = new FileInfo(caminho);

            if (nomeArquivo.Length > ColunasConfig.Arquivo.MAX_NOME_ORIGINAL || 
                caminhoFisico?.Length > ColunasConfig.Arquivo.MAX_CAMINHO_FISICO)
            {
                excedidos.Add(new ArquivoValidacaoInfo
                {
                    CaminhoCompleto = caminho,
                    NomeArquivo = nomeArquivo,
                    TamanhoNome = nomeArquivo.Length,
                    LimiteNome = ColunasConfig.Arquivo.MAX_NOME_ORIGINAL,
                    CaminhoFisico = caminhoFisico ?? string.Empty,
                    TamanhoCaminho = caminhoFisico?.Length ?? 0,
                    LimiteCaminho = ColunasConfig.Arquivo.MAX_CAMINHO_FISICO,
                    TamanhoArquivoBytes = fileInfo.Length
                });
            }
        }

        return (excedidos.Count == 0, excedidos);
    }
}