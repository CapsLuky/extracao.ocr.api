using API.Contratual.Domain.Interface;
using API.Contratual.Domain.Interface.Repository;
using API.Contratual.Test.Helper;
using AutoFixture;
using Moq;
using Shouldly;
using Xunit;

namespace API.Contratual.Test.Service;

public class AtividadeServiceTest
{
    // #region Properties
    //
    // private readonly IFixture _fixture;
    // private readonly Mock<ISupabaseRespository> _mockSupabaseRepository;
    //
    // #endregion
    //
    // #region Constructor
    //
    // public AtividadeServiceTest()
    // {
    //     //Precisa disso?
    //     _fixture = AutoMoqFixtureFactory.CreateFixture();
    //     
    //     _mockSupabaseRepository = new Mock<ISupabaseRespository>();
    // }
    //
    // #endregion
    //
    // [Fact]
    // [Trait("AtividadeService", "BuscarListaDeAtividadeAsync_Sucesso")]
    // public async Task BuscarListaDeAtividadeAsync()
    // {
    //     #region Arrange
    //
    //     //_mockSupabaseRepository.Setup(r => r.BuscarListaDeAtividadeAsync()).ReturnsAsync("");
    //     _fixture.Inject(_mockSupabaseRepository);
    //
    //     #endregion
    //
    //     #region Act
    //
    //     var supabaseService = _fixture.Create<AtividadeService>();
    //     var resonse = await supabaseService.BuscarListaDeTarefaAsync();
    //
    //     #endregion
    //
    //     #region Assert
    //
    //     resonse.ShouldNotBeNull();
    //
    //     #endregion
    // }
}