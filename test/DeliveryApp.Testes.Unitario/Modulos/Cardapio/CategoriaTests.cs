
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Modulos.Cardapio;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class CategoriaTests
{
    [TestMethod]
    public void DeveRetornar_ErroQuandoNomePossuirMenosDeDoisCaracteres()
    {
        // Arrange
        var categoria = CriarCategoriaValida(
            nome: "A"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            categoria.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Categoria.Nome))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoNomePossuirMaisDeCemCaracteres()
    {
        // Arrange
        string nome = new('A', 101);

        var categoria = CriarCategoriaValida(
            nome: nome
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            categoria.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Categoria.Nome))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoEstabelecimentoNaoForInformado()
    {
        // Arrange
        var categoria = CriarCategoriaValida(
            estabelecimentoId: Guid.Empty
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            categoria.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Categoria.EstabelecimentoId))
        );
    }

    [TestMethod]
    public void DeveSerValida_QuandoPossuirDadosValidos()
    {
        // Arrange
        var categoria = CriarCategoriaValida();

        // Act
        IReadOnlyList<ErroValidacao> erros =
            categoria.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void DeveAtualizar_NomeDaCategoria()
    {
        // Arrange
        Guid categoriaId = Guid.CreateVersion7();
        Guid estabelecimentoId = Guid.CreateVersion7();

        var categoria = CriarCategoriaValida(
            id: categoriaId,
            estabelecimentoId: estabelecimentoId,
            nome: "Categoria Antiga"
        );

        var categoriaAtualizada = new Categoria(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "Categoria Nova"
        );

        // Act
        categoria.Atualizar(
            categoriaAtualizada
        );

        // Assert
        Assert.AreEqual(
            categoriaId,
            categoria.Id
        );

        Assert.AreEqual(
            estabelecimentoId,
            categoria.EstabelecimentoId
        );

        Assert.AreEqual(
            "Categoria Nova",
            categoria.Nome
        );
    }

    [TestMethod]
    public void DeveRemover_EspacosDoNomeAoCriarCategoria()
    {
        // Arrange
        var categoria = new Categoria(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "  Lanches  "
        );

        // Act
        string nome = categoria.Nome;

        // Assert
        Assert.AreEqual(
            "Lanches",
            nome
        );
    }

    [TestMethod]
    public void DeveRemover_EspacosDoNomeAoAtualizarCategoria()
    {
        // Arrange
        var categoria = CriarCategoriaValida(
            nome: "Lanches"
        );

        var categoriaAtualizada = new Categoria(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "  Bebidas  "
        );

        // Act
        categoria.Atualizar(
            categoriaAtualizada
        );

        // Assert
        Assert.AreEqual(
            "Bebidas",
            categoria.Nome
        );
    }

    private static Categoria CriarCategoriaValida(
        Guid? id = null,
        Guid? estabelecimentoId = null,
        string nome = "Lanches")
    {
        return new Categoria(
            id ?? Guid.CreateVersion7(),
            estabelecimentoId ?? Guid.CreateVersion7(),
            nome
        );
    }
}
