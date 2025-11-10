using API.Contratual.Application;
using API.Contratual.CrossCutting.Notificador;
using API.Contratual.Data.Mysql.Connections;
using API.Contratual.Data.Mysql.Repository;
using API.Contratual.Domain.Configuracao;
using API.Contratual.Domain.Empresa;
using API.Contratual.Domain.ExtracaoTexto;
using API.Contratual.Domain.Interface.Repository;
using API.Contratual.Domain.Pesquisa;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Contratual.IoC;

public static class DependencyInjectionsConfig
{
    public static void ResolverDependencias(this IServiceCollection services)
    {
        services.AddSingleton<StringConnections>();
        
        services.AddScoped<INotificador, Notificador>();
        services.AddScoped(typeof(ExtracaoTextoApplication));
        services.AddScoped(typeof(PesquisaApplication));
        services.AddScoped(typeof(EmpresaApplication));
        services.AddScoped(typeof(ConfiguracaoApplication));
        services.AddScoped(typeof(IExtracaoTextoService), typeof(ExtracaoTextoService));
        services.AddScoped(typeof(IEmpresaService), typeof(EmpresaService));
        services.AddScoped(typeof(IPesquisaService), typeof(PesquisaService));
        services.AddScoped(typeof(IConfiguracaoService), typeof(ConfiguracaoService));
        //integration usar scoped
        
        
        //facadade usar scoped
        
        
        //Repositories
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddTransient<IDocumentoRepository, DocumentoRepository>();
        services.AddTransient<IEmpresaRepository, EmpresaRepository>();
        services.AddTransient<IConfiguracaoReposity, ConfiguracaoReposity>();
    }
}

