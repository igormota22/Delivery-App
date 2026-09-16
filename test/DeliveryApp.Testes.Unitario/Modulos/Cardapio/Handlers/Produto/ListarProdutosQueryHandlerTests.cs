using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class ListarProdutosQueryHandlerTests
{
    private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;

    private ListarProdutosQueryHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioProdutoMock = new Mock<IRepositorioProduto>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        handler = new ListarProdutosQueryHandler(
            repositorioProdutoMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ProdutosQuandoUsuarioForAutorizado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        ListarProdutosQuery query = new(estabelecimentoId);

        Categoria categoria = new(
            categoriaId,
            estabelecimentoId,
            "Lanches"
        );

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            categoriaId,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        typeof(Produto)
            .GetProperty(nameof(Produto.Categoria))!
            .SetValue(produto, categoria);

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.SelecionarTodosAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync([produto]);

        // Act
        Result<IReadOnlyList<ProdutoDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        IReadOnlyList<ProdutoDto> produtos = resultado.Value;

        Assert.HasCount(1, produtos);

        Assert.AreEqual(produto.Id, produtos[0].Id);
        Assert.AreEqual(produto.EstabelecimentoId, produtos[0].EstabelecimentoId);
        Assert.AreEqual(produto.CategoriaId, produtos[0].CategoriaId);
        Assert.AreEqual("Lanches", produtos[0].CategoriaNome);
        Assert.AreEqual(produto.Nome, produtos[0].Nome);
        Assert.AreEqual(produto.Descricao, produtos[0].Descricao);
        Assert.AreEqual(produto.Preco, produtos[0].Preco);
        Assert.AreEqual(produto.Ativo, produtos[0].Ativo);

        repositorioProdutoMock.Verify(
            x => x.SelecionarTodosAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ListaVaziaQuandoNaoHouverProdutos()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        ListarProdutosQuery query = new(estabelecimentoId);

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioProdutoMock
            .Setup(x => x.SelecionarTodosAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync([]);

        // Act
        Result<IReadOnlyList<ProdutoDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        IReadOnlyList<ProdutoDto> produtos = resultado.Value;

        Assert.HasCount(0, produtos);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoUsuarioNaoForAutorizado()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();
        Guid estabelecimentoId = Guid.NewGuid();

        ListarProdutosQuery query = new(estabelecimentoId);

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(usuarioId);

        // Act
        Result<IReadOnlyList<ProdutoDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioProdutoMock.Verify(
            x => x.SelecionarTodosAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

}
