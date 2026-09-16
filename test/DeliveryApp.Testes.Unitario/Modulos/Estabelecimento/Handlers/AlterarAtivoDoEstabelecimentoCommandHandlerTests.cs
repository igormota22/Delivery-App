using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Estabelecimentos;

[TestClass]
public class AlterarAtivoDoEstabelecimentoCommandHandlerTests
{
    private Mock<IRepositorioEstabelecimento> repositorioEstabelecimento = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuario = null!;
    private AlterarAtivoDoEstabelecimentoCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioEstabelecimento = new Mock<IRepositorioEstabelecimento>();
        provedorDeUsuario = new Mock<IProvedorDeUsuario>();

        handler = new AlterarAtivoDoEstabelecimentoCommandHandler(
            repositorioEstabelecimento.Object,
            provedorDeUsuario.Object
        );
    }

    [TestMethod]
    public async Task Deve_AlterarEstabelecimentoParaAtivo()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .Setup(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioEstabelecimento
            .Setup(x => x.AlterarAtivoAsync(
                estabelecimentoId,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new AlterarAtivoDoEstabelecimentoCommand(
            estabelecimentoId,
            true
        );

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioEstabelecimento.Verify(
            x => x.AlterarAtivoAsync(
                estabelecimentoId,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_AlterarEstabelecimentoParaInativo()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .Setup(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioEstabelecimento
            .Setup(x => x.AlterarAtivoAsync(
                estabelecimentoId,
                false,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new AlterarAtivoDoEstabelecimentoCommand(
            estabelecimentoId,
            false
        );

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioEstabelecimento.Verify(
            x => x.AlterarAtivoAsync(
                estabelecimentoId,
                false,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErroNaoAutorizado_QuandoUsuarioNaoForDonoDoEstabelecimento()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();
        Guid usuarioId = Guid.CreateVersion7();

        provedorDeUsuario
            .Setup(x => x.Id)
            .Returns(usuarioId);

        var command = new AlterarAtivoDoEstabelecimentoCommand(
            estabelecimentoId,
            true
        );

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.AlterarAtivoAsync(
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErroNaoEncontrado_QuandoEstabelecimentoNaoExistir()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .Setup(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioEstabelecimento
            .Setup(x => x.AlterarAtivoAsync(
                estabelecimentoId,
                true,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new AlterarAtivoDoEstabelecimentoCommand(
            estabelecimentoId,
            true
        );

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.AlterarAtivoAsync(
                estabelecimentoId,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErroDeConflito_QuandoOcorrerConflitoDePersistencia()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .Setup(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioEstabelecimento
            .Setup(x => x.AlterarAtivoAsync(
                estabelecimentoId,
                true,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflitoDePersistenciaException("Ocorreu um conflito ao persistir o estabelecimento.", new Exception()));

        var command = new AlterarAtivoDoEstabelecimentoCommand(
            estabelecimentoId,
            true
        );

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.AlterarAtivoAsync(
                estabelecimentoId,
                true,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}