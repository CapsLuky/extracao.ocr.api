using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.ResponseCompression;
using System.Text.Json.Serialization;
using System.IO.Compression;
using API.Contratual.Data.Mysql.Connections;
using API.Contratual.Dto;
using API.Contratual.IoC;

namespace API.Contratual.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient(); // para usar o httpclient factory
        services.ResolverDependencias();
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        #region CarregandoConfiguracoesDeAmbiente

        var connectionStringsSection = Configuration.GetSection("ConnectionStrings");
        services.Configure<ConnectionStrings>(connectionStringsSection);
        
        var appSettingsSection = Configuration.GetSection("AppSettings");
        services.Configure<AppSettings>(appSettingsSection);

        #endregion 

        #region Token JWT

        var appSettings = appSettingsSection.Get<AppSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey =
                    true, // Valida quem está emitindo, é o mesmo que está recebendo, com base na chave.
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = true,
                ValidIssuer = appSettings.Emissor,

                ValidateAudience = false // ValidateAudience = true,
                //ValidAudience = appSettings.ValidoEm,
            };
        });

        #endregion

        #region Swagger

        var apiVersioningBuilder = services.AddApiVersioning(
            options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

        apiVersioningBuilder.AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AdicionarSwaggerConfig();
        services.AddApiVersioning();
        services.AddSwaggerGen();

        #endregion

        #region Compressao Gzip

        services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

        services.AddResponseCompression(options =>
        {
            options.Providers.Add<GzipCompressionProvider>();
            options.EnableForHttps = true;
        });

        #endregion

        #region Serializacao

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

        #endregion
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            //app.UsarSwaggerConfig(provider);
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
        }
        
        loggerFactory.AddLog4Net();
        
        app.UseHttpsRedirection();
        app.UseCors("CorsPolicy");

        app.UseRouting();

        //app.UseAuthentication(); // JWT
        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        
        // app.UseSwagger(c =>
        // {
        //     c.RouteTemplate = "swagger/{documentName}/swagger.json";
        // });
        //
        // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskList"));
    }
}