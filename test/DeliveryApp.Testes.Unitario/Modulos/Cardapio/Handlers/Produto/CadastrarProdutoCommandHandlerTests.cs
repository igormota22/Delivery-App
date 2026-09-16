using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class CadastrarProdutoCommandHandlerTests
{
    private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
    private Mock<IRepositorioCategoria> repositorioCategoriaMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;

    private CadastrarProdutoCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioProdutoMock = new Mock<IRepositorioProduto>();
        repositorioCategoriaMock = new Mock<IRepositorioCategoria>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        handler = new CadastrarProdutoCommandHandler(
            repositorioProdutoMock.Object,
            repositorioCategoriaMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    [TestMethod]
    public async Task DeveCadastrar_ProdutoQuandoUsuarioForAutorizado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        CadastrarProdutoCommand command = new(
            estabelecimentoId,
            categoriaId,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m,
            [
                new ComplementoInput("Bacon", 5.00m)
            ]
        );

        Categoria categoria = new(
            categoriaId,
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(categoria);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreNotEqual(Guid.Empty, resultado.Value);

        repositorioCategoriaMock.Verify(
            x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        repositorioProdutoMock.Verify(
            x => x.CadastrarAsync(
                It.Is<Produto>(produto =>
                    produto.Id == resultado.Value &&
                    produto.EstabelecimentoId == estabelecimentoId &&
                    produto.CategoriaId == categoriaId &&
                    produto.Nome == "X-Burger" &&
                    produto.Descricao == "Hambúrguer artesanal" &&
                    produto.Preco == 25.90m &&
                    produto.Complementos.Count == 1 &&
                    produto.Complementos[0].Nome == "Bacon" &&
                    produto.Complementos[0].PrecoAdicional == 5.00m
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

        CadastrarProdutoCommand command = new(
            estabelecimentoId,
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
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioCategoriaMock.Verify(
            x => x.SelecionarPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        repositorioProdutoMock.Verify(
            x => x.CadastrarAsync(
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
        Guid categoriaId = Guid.NewGuid();

        CadastrarProdutoCommand command = new(
            estabelecimentoId,
            categoriaId,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m,
            []
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((Categoria?)null);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioProdutoMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Produto>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoProdutoForInvalido()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        CadastrarProdutoCommand command = new(
            estabelecimentoId,
            categoriaId,
            "X",
            "Hambúrguer artesanal",
            25.90m,
            []
        );

        Categoria categoria = new(
            categoriaId,
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(categoria);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioProdutoMock.Verify(
            x => x.CadastrarAsync(
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
        Guid categoriaId = Guid.NewGuid();

        CadastrarProdutoCommand command = new(
            estabelecimentoId,
            categoriaId,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m,
            []
        );

        Categoria categoria = new(
            categoriaId,
            estabelecimentoId,
            "Lanches"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarPorIdAsync(
                categoriaId,
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(categoria);

        repositorioProdutoMock
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Produto>(),
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new ConflitoDePersistenciaException("Ja existe esse produto", new Exception()));

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);
    }


}
