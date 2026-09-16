
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Estabelecimentos;

[TestClass]
public class EditarEstabelecimentoCommandHandlerTests
{
    private Mock<IRepositorioEstabelecimento> repositorioEstabelecimento = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuario = null!;
    private EditarEstabelecimentoCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioEstabelecimento = new Mock<IRepositorioEstabelecimento>();
        provedorDeUsuario = new Mock<IProvedorDeUsuario>();

        handler = new EditarEstabelecimentoCommandHandler(
            repositorioEstabelecimento.Object,
            provedorDeUsuario.Object
        );
    }

    [TestMethod]
    public async Task Deve_EditarEstabelecimento_ComDadosValidos()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioEstabelecimento
            .Setup(x => x.EditarAsync(
                estabelecimentoId,
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = CriarCommandValido(estabelecimentoId);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioEstabelecimento.Verify(
            x => x.EditarAsync(
                estabelecimentoId,
                It.Is<Estabelecimento>(e =>
                    e.Id == estabelecimentoId &&
                    e.NomeComercial == command.NomeComercial &&
                    e.Documento == command.Documento &&
                    e.Endereco == command.Endereco &&
                    e.Telefone == command.Telefone &&
                    e.AreaAtendimento == command.AreaAtendimento &&
                    e.HorarioAbertura == command.HorarioAbertura &&
                    e.HorarioFechamento == command.HorarioFechamento &&
                    e.TaxaEntrega == command.TaxaEntrega),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoEstabelecimentoNaoPertencerAoUsuario()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();
        Guid outroEstabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .SetupGet(x => x.Id)
            .Returns(outroEstabelecimentoId);

        var command = CriarCommandValido(estabelecimentoId);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.EditarAsync(
                It.IsAny<Guid>(),
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoDadosDoEstabelecimentoForemInvalidos()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        var command = CriarCommandValido(estabelecimentoId) with
        {
            NomeComercial = ""
        };

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.EditarAsync(
                It.IsAny<Guid>(),
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoEstabelecimentoNaoForEncontrado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioEstabelecimento
            .Setup(x => x.EditarAsync(
                estabelecimentoId,
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = CriarCommandValido(estabelecimentoId);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.EditarAsync(
                estabelecimentoId,
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoOcorrerConflitoDePersistencia()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        provedorDeUsuario
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioEstabelecimento
            .Setup(x => x.EditarAsync(
                estabelecimentoId,
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new ConflitoDePersistenciaException("Ocorreu um conflito ao persistir o estabelecimento.", new Exception()));

        var command = CriarCommandValido(estabelecimentoId);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.EditarAsync(
                estabelecimentoId,
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private static EditarEstabelecimentoCommand CriarCommandValido(Guid estabelecimentoId)
    {
        return new EditarEstabelecimentoCommand(
            estabelecimentoId,
            "Restaurante do Igor",
            "12345678000199",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            5.00m
        );
    }
}