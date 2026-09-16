using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class EditarCategoriaCommandHandlerTests
{
    private Mock<IRepositorioCategoria> repositorioCategoriaMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;
    private EditarCategoriaCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioCategoriaMock = new Mock<IRepositorioCategoria>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        handler = new EditarCategoriaCommandHandler(
            repositorioCategoriaMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    [TestMethod]
    public async Task DeveEditar_CategoriaQuandoUsuarioForAutorizado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        EditarCategoriaCommand command = new(
            estabelecimentoId,
            categoriaId,
            "Lanches"
        );

        Categoria categoria = new(
            categoriaId,
            estabelecimentoId,
            "Pizzas"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                categoriaId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        repositorioCategoriaMock
            .Setup(x => x.EditarAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<Categoria>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioCategoriaMock.Verify(
            x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                categoriaId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        repositorioCategoriaMock.Verify(
            x => x.EditarAsync(
                categoriaId,
                estabelecimentoId,
                It.Is<Categoria>(categoriaAtualizada =>
                    categoriaAtualizada.Id == categoriaId &&
                    categoriaAtualizada.EstabelecimentoId == estabelecimentoId &&
                    categoriaAtualizada.Nome == "Lanches"
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
        Guid categoriaId = Guid.NewGuid();

        EditarCategoriaCommand command = new(
            estabelecimentoId,
            categoriaId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(usuarioId);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

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
            x => x.EditarAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
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
        Guid categoriaId = Guid.NewGuid();

        EditarCategoriaCommand command = new(
            estabelecimentoId,
            categoriaId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                categoriaId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioCategoriaMock.Verify(
            x => x.EditarAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
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
        Guid categoriaId = Guid.NewGuid();

        EditarCategoriaCommand command = new(
            estabelecimentoId,
            categoriaId,
            "A"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

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
            x => x.EditarAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Categoria>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoCategoriaNaoForEncontrada()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        EditarCategoriaCommand command = new(
            estabelecimentoId,
            categoriaId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                categoriaId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        repositorioCategoriaMock
            .Setup(x => x.EditarAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<Categoria>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioCategoriaMock.Verify(
            x => x.EditarAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<Categoria>(),
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
        Guid categoriaId = Guid.NewGuid();

        EditarCategoriaCommand command = new(
            estabelecimentoId,
            categoriaId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                categoriaId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        repositorioCategoriaMock
            .Setup(x => x.EditarAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<Categoria>(),
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new ConflitoDePersistenciaException("Ja existe essa categoria", new Exception()));

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
    }
}
