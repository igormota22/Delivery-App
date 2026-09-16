using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class EditarProdutoCommandHandlerTests
{
    private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
    private Mock<IRepositorioCategoria> repositorioCategoriaMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;

    private EditarProdutoCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioProdutoMock = new Mock<IRepositorioProduto>();
        repositorioCategoriaMock = new Mock<IRepositorioCategoria>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        handler = new EditarProdutoCommandHandler(
            repositorioProdutoMock.Object,
            repositorioCategoriaMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    [TestMethod]
    public async Task DeveEditar_ProdutoQuandoUsuarioForAutorizado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        EditarProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            categoriaId,
            "X-Burger Especial",
            "Hambúrguer artesanal especial",
            32.90m,
            [
                new ComplementoInput("Bacon", 5.00m)
            ]
        );

        Produto produto = new(
            produtoId,
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        Categoria categoria = new(
            categoriaId,
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.SelecionarPorIdAsync(
                produtoId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(produto);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(categoria);

        repositorioProdutoMock
            .Setup(x => x.EditarAsync(
                produtoId,
                estabelecimentoId,
                It.IsAny<Produto>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        repositorioProdutoMock.Verify(
            x => x.SelecionarPorIdAsync(
                produtoId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        repositorioCategoriaMock.Verify(
            x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        repositorioProdutoMock.Verify(
            x => x.EditarAsync(
                produtoId,
                estabelecimentoId,
                It.Is<Produto>(produtoAtualizado =>
                    produtoAtualizado.Id == produtoId &&
                    produtoAtualizado.EstabelecimentoId == estabelecimentoId &&
                    produtoAtualizado.CategoriaId == categoriaId &&
                    produtoAtualizado.Nome == "X-Burger Especial" &&
                    produtoAtualizado.Descricao == "Hambúrguer artesanal especial" &&
                    produtoAtualizado.Preco == 32.90m &&
                    produtoAtualizado.Complementos.Count == 1
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

        EditarProdutoCommand command = new(
            estabelecimentoId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m,
            []
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(usuarioId);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioProdutoMock.Verify(
            x => x.SelecionarPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
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

        EditarProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m,
            []
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.SelecionarPorIdAsync(
                produtoId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((Produto?)null);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioCategoriaMock.Verify(
            x => x.SelecionarPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        repositorioProdutoMock.Verify(
            x => x.EditarAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Produto>(),
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
        Guid produtoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        EditarProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            categoriaId,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m,
            []
        );

        Produto produto = new(
            produtoId,
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.SelecionarPorIdAsync(
                produtoId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(produto);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((Categoria?)null);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioProdutoMock.Verify(
            x => x.EditarAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Produto>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoProdutoAtualizadoForInvalido()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        EditarProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            categoriaId,
            "X",
            "Hambúrguer artesanal",
            25.90m,
            []
        );

        Produto produto = new(
            produtoId,
            estabelecimentoId,
            categoriaId,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        Categoria categoria = new(
            categoriaId,
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.SelecionarPorIdAsync(
                produtoId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(produto);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(categoria);

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioProdutoMock.Verify(
            x => x.EditarAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Produto>(),
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
        Guid produtoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        EditarProdutoCommand command = new(
            estabelecimentoId,
            produtoId,
            categoriaId,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m,
            []
        );

        Produto produto = new(
            produtoId,
            estabelecimentoId,
            categoriaId,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        Categoria categoria = new(
            categoriaId,
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.SelecionarPorIdAsync(
                produtoId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(produto);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(categoria);

        repositorioProdutoMock
            .Setup(x => x.EditarAsync(
                produtoId,
                estabelecimentoId,
                It.IsAny<Produto>(),
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new ConflitoDePersistenciaException("Ja existe esse produto", new Exception()));

        // Act
        Result resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
    }


}
