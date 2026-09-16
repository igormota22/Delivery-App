
using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using DeliveryApp.Testes.Integracao.Compartilhado;
using DeliveryApp.Testes.Integracao.Compartilhado.Orm;

namespace DeliveryApp.Testes.Integracao.Modulos.Cardapio;

[TestClass]
public class RepositorioProdutoTestEmOrm : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public async Task DeveCadastrar_Produto()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        // Act
        await repositorioProduto.CadastrarAsync(produto);

        // Assert
        Produto? produtoSelecionado =
            await repositorioProduto.SelecionarPorIdAsync(
                produto.Id,
                estabelecimentoId
            );

        Assert.IsNotNull(produtoSelecionado);
        Assert.AreEqual(produto.Id, produtoSelecionado.Id);
    }

    [TestMethod]
    public async Task DeveSelecionar_ProdutoPorId()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        await repositorioProduto.CadastrarAsync(produto);

        // Act
        Produto? produtoSelecionado =
            await repositorioProduto.SelecionarPorIdAsync(
                produto.Id,
                estabelecimentoId
            );

        // Assert
        Assert.IsNotNull(produtoSelecionado);
        Assert.AreEqual(produto.Id, produtoSelecionado.Id);
    }

    [TestMethod]
    public async Task DeveSelecionarTodos_ProdutosDoEstabelecimento()
    {
        // Arrange
        Estabelecimento? estabelecimento = new Estabelecimento(
            Guid.CreateVersion7(),
            "Pizzaria",
            "01366999202",
            "Rua Finado Da Silva",
            "(49) 99999-9999",
            "Lages",
            new TimeOnly(19),
            new TimeOnly(20),
            0

        );

        Categoria categoria = new Categoria(
            Guid.CreateVersion7(),
            estabelecimento.Id,
            "Lanches"
        );

        Produto produto1 = new(
            Guid.NewGuid(),
            estabelecimento.Id,
            categoria.Id,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        Produto produto2 = new(
            Guid.NewGuid(),
            estabelecimento.Id,
            categoria.Id,
            "Batata Frita",
            "Porção de batatas",
            15.90m
        );

        await repositorioEstabelecimento.CadastrarAsync(estabelecimento);
        await repositorioCategoria.CadastrarAsync(categoria);
        await repositorioProduto.CadastrarAsync(produto1);
        await repositorioProduto.CadastrarAsync(produto2);


        // Act
        List<Produto> produtos =
            await repositorioProduto.SelecionarTodosAsync(
                estabelecimento.Id
            );

        // Assert
        Assert.HasCount(2, produtos);

        Assert.IsTrue(
            produtos.Any(produto => produto.Id == produto1.Id)
        );

        Assert.IsTrue(
            produtos.Any(produto => produto.Id == produto2.Id)
        );
    }

    [TestMethod]
    public async Task DeveSelecionarTodos_ProdutosOrdenadosPorCategoriaENome()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Categoria categoriaBebidas = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Bebidas"
        );

        Categoria categoriaLanches = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Lanches"
        );

        await repositorioCategoria.CadastrarAsync(categoriaBebidas);
        await repositorioCategoria.CadastrarAsync(categoriaLanches);

        Produto produto1 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            categoriaLanches.Id,
            "X-Salada",
            "Hambúrguer com salada",
            27.90m
        );

        Produto produto2 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            categoriaBebidas.Id,
            "Coca-Cola",
            "Refrigerante",
            8.00m
        );

        Produto produto3 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            categoriaLanches.Id,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        await repositorioProduto.CadastrarAsync(produto1);
        await repositorioProduto.CadastrarAsync(produto2);
        await repositorioProduto.CadastrarAsync(produto3);

        // Act
        List<Produto> produtos =
            await repositorioProduto.SelecionarTodosAsync(
                estabelecimentoId
            );

        // Assert
        Assert.HasCount(3, produtos);

        Assert.AreEqual(produto2.Id, produtos[0].Id);
        Assert.AreEqual(produto3.Id, produtos[1].Id);
        Assert.AreEqual(produto1.Id, produtos[2].Id);
    }

    [TestMethod]
    public async Task DeveSelecionarCardapio_ApenasProdutosAtivos()
    {
        // Arrange
        Estabelecimento? estabelecimento = new Estabelecimento(
            Guid.CreateVersion7(),
            "Pizzaria",
            "01366999202",
            "Rua Finado Da Silva",
            "(49) 99999-9999",
            "Lages",
            new TimeOnly(19),
            new TimeOnly(20),
            0

        );

        Categoria categoria = new Categoria(
            Guid.CreateVersion7(),
            estabelecimento.Id,
            "Lanches"
        );

        Produto produto1 = new(
            Guid.NewGuid(),
            estabelecimento.Id,
            categoria.Id,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        Produto produto2 = new(
            Guid.NewGuid(),
            estabelecimento.Id,
            categoria.Id,
            "Batata Frita",
            "Porção de batatas",
            15.90m
        );

        produto2.Desativar();

        await repositorioEstabelecimento.CadastrarAsync(estabelecimento);
        await repositorioCategoria.CadastrarAsync(categoria);
        await repositorioProduto.CadastrarAsync(produto1);
        await repositorioProduto.CadastrarAsync(produto2);

        // Act
        List<Produto> produtos =
            await repositorioProduto.SelecionarCardapioAsync(
                estabelecimento.Id
            );

        // Assert
        Assert.HasCount(1, produtos);
        Assert.AreEqual(produto1.Id, produtos[0].Id);
    }

    [TestMethod]
    public async Task DeveSelecionar_ProdutoComCategoriaEComplementos()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Categoria categoria = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Lanches"
        );

        await repositorioCategoria.CadastrarAsync(categoria);

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            categoria.Id,
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        Complemento complemento = new(
            Guid.NewGuid(),
            produto.Id,
            "Bacon",
            5.00m
        );

        produto.SubstituirComplementos([complemento]);

        await repositorioProduto.CadastrarAsync(produto);

        // Act
        Produto? produtoSelecionado =
            await repositorioProduto.SelecionarPorIdAsync(
                produto.Id,
                estabelecimentoId
            );

        // Assert
        Assert.IsNotNull(produtoSelecionado);

        Assert.IsNotNull(produtoSelecionado.Complementos);
        Assert.HasCount(1, produtoSelecionado.Complementos);

        Assert.AreEqual(
            complemento.Id,
            produtoSelecionado.Complementos[0].Id
        );
    }

    [TestMethod]
    public async Task DeveEditar_Produto()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        await repositorioProduto.CadastrarAsync(produto);

        Produto produtoAtualizado = new(
            produto.Id,
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger Especial",
            "Hambúrguer artesanal especial",
            32.90m
        );

        // Act
        bool resultado =
            await repositorioProduto.EditarAsync(
                produto.Id,
                estabelecimentoId,
                produtoAtualizado
            );

        // Assert
        Assert.IsTrue(resultado);

        Produto? produtoSelecionado =
            await repositorioProduto.SelecionarPorIdAsync(
                produto.Id,
                estabelecimentoId
            );

        Assert.IsNotNull(produtoSelecionado);
        Assert.AreEqual(produto.Id, produtoSelecionado.Id);
        Assert.AreEqual(
            produtoAtualizado.CategoriaId,
            produtoSelecionado.CategoriaId
        );
        Assert.AreEqual(
            produtoAtualizado.Nome,
            produtoSelecionado.Nome
        );
    }

    [TestMethod]
    public async Task DeveEditar_ProdutoSubstituindoComplementos()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        Complemento complementoOriginal = new(
            Guid.NewGuid(),
            produto.Id,
            "Bacon",
            5.00m
        );

        produto.SubstituirComplementos([complementoOriginal]);

        await repositorioProduto.CadastrarAsync(produto);

        Complemento complementoAtualizado = new(
            Guid.NewGuid(),
            produto.Id,
            "Cheddar",
            4.00m
        );

        Produto produtoAtualizado = new(
            produto.Id,
            estabelecimentoId,
            produto.CategoriaId,
            produto.Nome,
            produto.Descricao,
            produto.Preco,
            [complementoAtualizado]
        );

        // Act
        bool resultado =
            await repositorioProduto.EditarAsync(
                produto.Id,
                estabelecimentoId,
                produtoAtualizado
            );

        // Assert
        Assert.IsTrue(resultado);

        Produto? produtoSelecionado =
            await repositorioProduto.SelecionarPorIdAsync(
                produto.Id,
                estabelecimentoId
            );

        Assert.IsNotNull(produtoSelecionado);
        Assert.HasCount(1, produtoSelecionado.Complementos);

        Assert.AreEqual(
            complementoAtualizado.Id,
            produtoSelecionado.Complementos[0].Id
        );
    }

    [TestMethod]
    public async Task DeveAlterarAtivo_ParaDesativado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        await repositorioProduto.CadastrarAsync(produto);

        // Act
        bool resultado =
            await repositorioProduto.AlterarAtivoAsync(
                produto.Id,
                estabelecimentoId,
                false
            );

        // Assert
        Assert.IsTrue(resultado);

        Produto? produtoSelecionado =
            await repositorioProduto.SelecionarPorIdAsync(
                produto.Id,
                estabelecimentoId
            );

        Assert.IsNotNull(produtoSelecionado);
        Assert.IsFalse(produtoSelecionado.Ativo);
    }

    [TestMethod]
    public async Task DeveAlterarAtivo_ParaAtivado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        produto.Desativar();

        await repositorioProduto.CadastrarAsync(produto);

        // Act
        bool resultado =
            await repositorioProduto.AlterarAtivoAsync(
                produto.Id,
                estabelecimentoId,
                true
            );

        // Assert
        Assert.IsTrue(resultado);

        Produto? produtoSelecionado =
            await repositorioProduto.SelecionarPorIdAsync(
                produto.Id,
                estabelecimentoId
            );

        Assert.IsNotNull(produtoSelecionado);
        Assert.IsTrue(produtoSelecionado.Ativo);
    }

    [TestMethod]
    public async Task DeveObterProdutosParaPedido()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Produto produto1 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        Produto produto2 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Salada",
            "Hambúrguer com salada",
            27.90m
        );

        await repositorioProduto.CadastrarAsync(produto1);
        await repositorioProduto.CadastrarAsync(produto2);

        // Act
        IReadOnlyList<Produto> produtos =
            await repositorioProduto.ObterParaPedidoAsync(
                estabelecimentoId,
                [produto1.Id, produto2.Id],
                CancellationToken.None
            );

        // Assert
        Assert.HasCount(2, produtos);

        Assert.IsTrue(
            produtos.Any(produto => produto.Id == produto1.Id)
        );

        Assert.IsTrue(
            produtos.Any(produto => produto.Id == produto2.Id)
        );
    }

    [TestMethod]
    public async Task DeveExcluir_Produto()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Produto produto = new(
            Guid.NewGuid(),
            estabelecimentoId,
            Guid.NewGuid(),
            "X-Burger",
            "Hambúrguer artesanal",
            25.90m
        );

        await repositorioProduto.CadastrarAsync(produto);

        // Act
        bool resultado =
            await repositorioProduto.ExcluirAsync(produto.Id);

        // Assert
        Assert.IsTrue(resultado);

        Produto? produtoSelecionado =
            await repositorioProduto.SelecionarPorIdAsync(
                produto.Id,
                estabelecimentoId
            );

        Assert.IsNull(produtoSelecionado);
    }
}