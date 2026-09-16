
using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Testes.Integracao.Compartilhado.Orm;

namespace DeliveryApp.Testes.Integracao.Modulos.Cardapio;

[TestClass]
public class RepositorioCategoriaTestEmOrm : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public async Task DeveCadastrar_Categoria()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Categoria categoria = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Lanches"
        );

        // Act
        await repositorioCategoria.CadastrarAsync(categoria);

        // Assert
        Categoria? categoriaSelecionada =
            await repositorioCategoria.SelecionarPorIdAsync(
                categoria.Id,
                estabelecimentoId
            );

        Assert.IsNotNull(categoriaSelecionada);

        Assert.AreEqual(
            categoria.Id,
            categoriaSelecionada.Id
        );

        Assert.AreEqual(
            estabelecimentoId,
            categoriaSelecionada.EstabelecimentoId
        );

        Assert.AreEqual(
            "Lanches",
            categoriaSelecionada.Nome
        );
    }

    [TestMethod]
    public async Task DeveSelecionar_CategoriaPorId()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Categoria categoria = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Lanches"
        );

        await repositorioCategoria.CadastrarAsync(categoria);

        // Act
        Categoria? categoriaSelecionada =
            await repositorioCategoria.SelecionarPorIdAsync(
                categoria.Id,
                estabelecimentoId
            );

        // Assert
        Assert.IsNotNull(categoriaSelecionada);

        Assert.AreEqual(
            categoria.Id,
            categoriaSelecionada.Id
        );

        Assert.AreEqual(
            categoria.EstabelecimentoId,
            categoriaSelecionada.EstabelecimentoId
        );

        Assert.AreEqual(
            categoria.Nome,
            categoriaSelecionada.Nome
        );
    }

    [TestMethod]
    public async Task DeveRetornarNull_QuandoCategoriaNaoPertencerAoEstabelecimento()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid outroEstabelecimentoId = Guid.NewGuid();

        Categoria categoria = new(
            Guid.NewGuid(),
            outroEstabelecimentoId,
            "Lanches"
        );

        await repositorioCategoria.CadastrarAsync(categoria);

        // Act
        Categoria? categoriaSelecionada =
            await repositorioCategoria.SelecionarPorIdAsync(
                categoria.Id,
                estabelecimentoId
            );

        // Assert
        Assert.IsNull(categoriaSelecionada);
    }

    [TestMethod]
    public async Task DeveSelecionarTodos_CategoriasDoEstabelecimento()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid outroEstabelecimentoId = Guid.NewGuid();

        Categoria categoria1 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Lanches"
        );

        Categoria categoria2 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Bebidas"
        );

        Categoria categoriaOutroEstabelecimento = new(
            Guid.NewGuid(),
            outroEstabelecimentoId,
            "Sobremesas"
        );

        await repositorioCategoria.CadastrarAsync(categoria1);
        await repositorioCategoria.CadastrarAsync(categoria2);
        await repositorioCategoria.CadastrarAsync(
            categoriaOutroEstabelecimento
        );

        // Act
        List<Categoria> categorias =
            await repositorioCategoria.SelecionarTodosAsync(
                estabelecimentoId
            );

        // Assert
        Assert.HasCount(2, categorias);

        Assert.IsTrue(
            categorias.All(c =>
                c.EstabelecimentoId == estabelecimentoId)
        );
    }


    [TestMethod]
    public async Task DeveRetornarFalse_QuandoNomeExistirEmOutroEstabelecimento()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();
        Guid outroEstabelecimentoId = Guid.NewGuid();

        Categoria categoria = new(
            Guid.NewGuid(),
            outroEstabelecimentoId,
            "Lanches"
        );

        await repositorioCategoria.CadastrarAsync(categoria);

        // Act
        bool resultado =
            await repositorioCategoria.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches"
            );

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public async Task DeveIgnorar_CategoriaInformadaNaVerificacaoDeNome()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Categoria categoria = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Lanches"
        );

        await repositorioCategoria.CadastrarAsync(categoria);

        // Act
        bool resultado =
            await repositorioCategoria.ExisteComNomeAsync(
                estabelecimentoId,
                "Lanches",
                categoria.Id
            );

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public async Task DeveEditar_Categoria()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        Categoria categoria = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Lanches"
        );

        await repositorioCategoria.CadastrarAsync(categoria);

        Categoria categoriaAtualizada = new(
            categoria.Id,
            estabelecimentoId,
            "Bebidas"
        );

        // Act
        bool resultado =
            await repositorioCategoria.EditarAsync(
                categoria.Id,
                estabelecimentoId,
                categoriaAtualizada
            );

        // Assert
        Assert.IsTrue(resultado);

        Categoria? categoriaSelecionada =
            await repositorioCategoria.SelecionarPorIdAsync(
                categoria.Id,
                estabelecimentoId
            );

        Assert.IsNotNull(categoriaSelecionada);

        Assert.AreEqual(
            categoria.Id,
            categoriaSelecionada.Id
        );

        Assert.AreEqual(
            "Bebidas",
            categoriaSelecionada.Nome
        );

        Assert.AreEqual(
            estabelecimentoId,
            categoriaSelecionada.EstabelecimentoId
        );
    }

    [TestMethod]
    public async Task DeveRetornarFalse_AoEditarCategoriaInexistente()
    {
        // Arrange
        Guid categoriaId = Guid.NewGuid();
        Guid estabelecimentoId = Guid.NewGuid();

        Categoria categoriaAtualizada = new(
            categoriaId,
            estabelecimentoId,
            "Bebidas"
        );

        // Act
        bool resultado =
            await repositorioCategoria.EditarAsync(
                categoriaId,
                estabelecimentoId,
                categoriaAtualizada
            );

        // Assert
        Assert.IsFalse(resultado);
    }
}
