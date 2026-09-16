using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.Dominio.Compartilhado.Auth;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Estabelecimentos;

[TestClass]
public class AutenticarEstabelecimentoCommandHandlerTests
{
    private Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = null!;
    private Mock<IEmissorDeTokens> emissorDeTokens = null!;
    private AutenticarEstabelecimentoCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        gerenciadorDeIdentidade = new Mock<IGerenciadorDeIdentidade>();
        emissorDeTokens = new Mock<IEmissorDeTokens>();

        handler = new AutenticarEstabelecimentoCommandHandler(
            gerenciadorDeIdentidade.Object,
            emissorDeTokens.Object
        );
    }

    [TestMethod]
    public async Task Deve_AutenticarEstabelecimento_ComCredenciaisValidas()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();
        string email = "estabelecimento@email.com";
        string senha = "Senha@123";
        string token = "token-jwt";

        UsuarioDto usuario = new(
            estabelecimentoId,
            email
        );

        DateTime dataExpiracao = DateTime.UtcNow.AddHours(1);

        var accessToken = new AccessToken(
            token,
            dataExpiracao
        );

        gerenciadorDeIdentidade
            .Setup(x => x.ChecarValidadeDeSenhaAsync(
                email,
                senha,
                TipoUsuario.Estabelecimento))
            .ReturnsAsync(usuario);

        emissorDeTokens
            .Setup(x => x.CriarToken(
                estabelecimentoId,
                email,
                TipoUsuario.Estabelecimento))
            .Returns(accessToken);

        var command = new AutenticarEstabelecimentoCommand(
            email,
            senha
        );

        // Act
        Result<AccessTokenDoUsuarioDto> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);

        Assert.AreEqual(estabelecimentoId, resultado.Value.UsuarioId);
        Assert.AreEqual(token, resultado.Value.Token);
        Assert.AreEqual(dataExpiracao, resultado.Value.DataExpiracaoEmUtc);

        gerenciadorDeIdentidade.Verify(
            x => x.ChecarValidadeDeSenhaAsync(
                email,
                senha,
                TipoUsuario.Estabelecimento),
            Times.Once
        );

        emissorDeTokens.Verify(
            x => x.CriarToken(
                estabelecimentoId,
                email,
                TipoUsuario.Estabelecimento),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoCredenciaisForemInvalidas()
    {
        // Arrange
        string email = "estabelecimento@email.com";
        string senha = "SenhaInvalida";

        gerenciadorDeIdentidade
            .Setup(x => x.ChecarValidadeDeSenhaAsync(
                email,
                senha,
                TipoUsuario.Estabelecimento))
            .ReturnsAsync((UsuarioDto?)null);

        var command = new AutenticarEstabelecimentoCommand(
            email,
            senha
        );

        // Act
        Result<AccessTokenDoUsuarioDto> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        emissorDeTokens.Verify(
            x => x.CriarToken(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<TipoUsuario>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Deve_CriarToken_ComTipoEstabelecimento()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();
        string email = "estabelecimento@email.com";
        string senha = "Senha@123";

        UsuarioDto usuario = new(
            estabelecimentoId,
            email
        );

        var accessToken = new AccessToken(
            "token-jwt",
            DateTime.UtcNow.AddHours(1)
        );

        gerenciadorDeIdentidade
            .Setup(x => x.ChecarValidadeDeSenhaAsync(
                email,
                senha,
                TipoUsuario.Estabelecimento))
            .ReturnsAsync(usuario);

        emissorDeTokens
            .Setup(x => x.CriarToken(
                estabelecimentoId,
                email,
                TipoUsuario.Estabelecimento))
            .Returns(accessToken);

        var command = new AutenticarEstabelecimentoCommand(
            email,
            senha
        );

        // Act
        await handler.Handle(command);

        // Assert
        emissorDeTokens.Verify(
            x => x.CriarToken(
                estabelecimentoId,
                email,
                TipoUsuario.Estabelecimento),
            Times.Once
        );
    }
}