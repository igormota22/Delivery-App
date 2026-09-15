using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Clientes;
using DeliveryApp.Dominio.Compartilhado.Auth;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Clientes;

[TestClass]
public class AutenticarClienteCommandHandlerTests
{
    private Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidadeMock = null!;
    private Mock<IEmissorDeTokens> emissorDeTokensMock = null!;
    private AutenticarClienteCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        gerenciadorDeIdentidadeMock = new Mock<IGerenciadorDeIdentidade>();
        emissorDeTokensMock = new Mock<IEmissorDeTokens>();

        handler = new AutenticarClienteCommandHandler(
            gerenciadorDeIdentidadeMock.Object,
            emissorDeTokensMock.Object
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoCredenciaisForemInvalidas()
    {
        // Arrange
        AutenticarClienteCommand command = new(
            "igor@email.com",
            "senha-invalida"
        );

        gerenciadorDeIdentidadeMock
            .Setup(x => x.ChecarValidadeDeSenhaAsync(
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ))
            .ReturnsAsync((UsuarioDto?)null);

        // Act
        Result<AccessTokenDoUsuarioDto> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        Assert.IsTrue(
            resultado.Errors.Any()
        );

        gerenciadorDeIdentidadeMock.Verify(
            x => x.ChecarValidadeDeSenhaAsync(
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ),
            Times.Once
        );

        emissorDeTokensMock.Verify(
            x => x.CriarToken(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<TipoUsuario>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_TokenQuandoCredenciaisForemValidas()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();
        string email = "igor@email.com";

        AutenticarClienteCommand command = new(
            email,
            "Senha123!"
        );

        UsuarioDto usuario = new(
            usuarioId,
            email
        );

        gerenciadorDeIdentidadeMock
            .Setup(x => x.ChecarValidadeDeSenhaAsync(
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ))
            .ReturnsAsync(usuario);

        DateTime dataExpiracao = DateTime.UtcNow.AddHours(1);

        AccessToken accessToken = new(
            "token-jwt-teste",
            dataExpiracao
        );

        emissorDeTokensMock
            .Setup(x => x.CriarToken(
                usuarioId,
                email,
                TipoUsuario.Cliente
            ))
            .Returns(accessToken);

        // Act
        Result<AccessTokenDoUsuarioDto> resultado =
            await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);

        Assert.AreEqual(
            usuarioId,
            resultado.Value.UsuarioId
        );

        Assert.AreEqual(
            "token-jwt-teste",
            resultado.Value.Token
        );

        Assert.AreEqual(
            dataExpiracao,
            resultado.Value.DataExpiracaoEmUtc
        );

        gerenciadorDeIdentidadeMock.Verify(
            x => x.ChecarValidadeDeSenhaAsync(
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ),
            Times.Once
        );

        emissorDeTokensMock.Verify(
            x => x.CriarToken(
                usuarioId,
                email,
                TipoUsuario.Cliente
            ),
            Times.Once
        );
    }
}