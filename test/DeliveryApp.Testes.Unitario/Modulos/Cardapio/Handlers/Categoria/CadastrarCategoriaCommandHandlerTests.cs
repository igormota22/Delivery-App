using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class CadastrarCategoriaCommandHandlerTests
{
    private Mock<IRepositorioCategoria> repositorioCategoriaMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;
    private CadastrarCategoriaCommandHandler handler = null!;


    [TestInitialize]
    public void Inicializar()
    {
        repositorioCategoriaMock = new Mock<IRepositorioCategoria>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        handler = new CadastrarCategoriaCommandHandler(
            repositorioCategoriaMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    [TestMethod]
    public async Task DeveCadastrar_CategoriaQuandoUsuarioForAutorizado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        CadastrarCategoriaCommand command = new(
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                null,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreNotEqual(Guid.Empty, resultado.Value);

        repositorioCategoriaMock.Verify(
            x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                null,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        repositorioCategoriaMock.Verify(
            x => x.CadastrarAsync(
                It.Is<Categoria>(categoria =>
                    categoria.Id == resultado.Value &&
                    categoria.EstabelecimentoId == estabelecimentoId &&
                    categoria.Nome == "Lanches"
                ),
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

        CadastrarCategoriaCommand command = new(
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(usuarioId);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioCategoriaMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Categoria>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoCategoriaForDuplicada()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        CadastrarCategoriaCommand command = new(
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                null,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioCategoriaMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Categoria>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoCategoriaForInvalida()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        CadastrarCategoriaCommand command = new(
            estabelecimentoId,
            "A"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioCategoriaMock.Verify(
            x => x.ExisteComNomeAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        repositorioCategoriaMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Categoria>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoOcorrerConflitoDePersistencia()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        CadastrarCategoriaCommand command = new(
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                null,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        repositorioCategoriaMock
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Categoria>(),
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new ConflitoDePersistenciaException("Ja existe essa categoria", new Exception()));

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);
    }
}
