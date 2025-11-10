using API.Contratual.Domain;
using API.Contratual.Domain.Interface.Repository;
using API.Contratual.Test.Helper;
using AutoFixture;
using Moq;
using Shouldly;
using Xunit;


namespace API.Contratual.Test.Service;

public class ExtracaoTextoServiceTest
{
    #region Propriedades
    
    private readonly IFixture _fixture;
    private readonly Mock<IDocumentoRepository> _documentoRepository;
    
    #endregion
    
    #region Constructor
    
    public ExtracaoTextoServiceTest()
    {
        _fixture = AutoMoqFixtureFactory.CreateFixture();
        _documentoRepository = new Mock<IDocumentoRepository>();
    }
    
    #endregion
    
    // [Fact]
    // [Trait("MotorBuscaService", "SelecionarArquivosParaExtrairTextoAsync_Sucesso")]
    // public async Task SelecionarArquivosParaExtrairTextoAsync_Sucesso()
    // {
    //     #region Arrange
    //     
    //     var arquivo = _fixture.Create<ArquivoEntity>();
    //     
    //     
    //     _documentoRepository.Setup(r => r.BuscarArquivosProcessadosPorNome(It.IsAny<string>())).ReturnsAsync(arquivo);
    //     
    //     _fixture.Inject(_documentoRepository);
    //
    //     #endregion
    //
    //     #region Act
    //
    //     var supabaseService = _fixture.Create<MotorBuscaService>();
    //     var resonse = await supabaseService.SelecionarArquivosParaExtrairTextoAsync();
    //
    //     #endregion
    //
    //     #region Assert
    //
    //     resonse.ShouldBeOfType<List<ArquivoEntity>>();
    //     resonse.ShouldNotBeNull();
    //
    //     #endregion
    // }
}