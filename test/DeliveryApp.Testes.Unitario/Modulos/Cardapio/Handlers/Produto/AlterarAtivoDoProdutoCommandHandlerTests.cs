using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class AlterarAtivoDoProdutoCommandHandlerTests
{
    private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;

    private AlterarAtivoDoProdutoCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioProdutoMock = new Mock<IRepositorioProduto>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        handler = new AlterarAtivoDoProdutoCommandHandler(
            repositorioProdutoMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    [TestMethod]
    public async Task DeveAlterarAtivo_QuandoUsuarioForAutorizado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        AlterarAtivoDoProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            false
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.AlterarAtivoAsync(
                produtoId,
                estabelecimentoId,
                false,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioProdutoMock.Verify(
            x => x.AlterarAtivoAsync(
                produtoId,
                estabelecimentoId,
                false,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoUsuarioNaoForAutorizado()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();
        Guid estabelecimentoId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        AlterarAtivoDoProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            false
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(usuarioId);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioProdutoMock.Verify(
            x => x.AlterarAtivoAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoProdutoNaoForEncontrado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        AlterarAtivoDoProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            false
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.AlterarAtivoAsync(
                produtoId,
                estabelecimentoId,
                false,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioProdutoMock.Verify(
            x => x.AlterarAtivoAsync(
                produtoId,
                estabelecimentoId,
                false,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoOcorrerConflitoDePersistencia()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        AlterarAtivoDoProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            true
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.AlterarAtivoAsync(
                produtoId,
                estabelecimentoId,
                true,
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new ConflitoDePersistenciaException("Ja existe esse produto", new Exception()));

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
    }

}
