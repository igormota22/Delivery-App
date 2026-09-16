
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Modulos.Cardapio;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class ProdutoTests
{
    [TestMethod]
    public void DeveRetornar_ErroQuandoEstabelecimentoNaoForInformado()
    {
        // Arrange
        var produto = CriarProdutoValido(
            estabelecimentoId: Guid.Empty
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.EstabelecimentoId))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoCategoriaNaoForInformada()
    {
        // Arrange
        var produto = CriarProdutoValido(
            categoriaId: Guid.Empty
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.CategoriaId))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoNomePossuirMenosDeDoisCaracteres()
    {
        // Arrange
        var produto = CriarProdutoValido(
            nome: "A"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.Nome))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoNomePossuirMaisDeCemCaracteres()
    {
        // Arrange
        string nome = new('A', 101);

        var produto = CriarProdutoValido(
            nome: nome
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.Nome))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoDescricaoNaoForInformada()
    {
        // Arrange
        var produto = CriarProdutoValido(
            descricao: string.Empty
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.Descricao))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoDescricaoPossuirMaisDeMilCaracteres()
    {
        // Arrange
        string descricao = new('A', 1001);

        var produto = CriarProdutoValido(
            descricao: descricao
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.Descricao))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoPrecoForMenorOuIgualAZero()
    {
        // Arrange
        var produto = CriarProdutoValido(
            preco: 0
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.Preco))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoPrecoPossuirMaisDeDuasCasasDecimais()
    {
        // Arrange
        var produto = CriarProdutoValido(
            preco: 10.123m
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.Preco))
        );
    }

    [TestMethod]
    public void DeveSerValido_QuandoProdutoPossuirDadosValidos()
    {
        // Arrange
        var produto = CriarProdutoValido();

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void DeveIniciar_ProdutoComoAtivo()
    {
        // Arrange
        var produto = CriarProdutoValido();

        // Act
        bool ativo = produto.Ativo;

        // Assert
        Assert.IsTrue(ativo);
    }

    [TestMethod]
    public void DeveDesativar_Produto()
    {
        // Arrange
        var produto = CriarProdutoValido();

        // Act
        produto.Desativar();

        // Assert
        Assert.IsFalse(produto.Ativo);
    }

    [TestMethod]
    public void DeveAtivar_ProdutoDesativado()
    {
        // Arrange
        var produto = CriarProdutoValido();

        produto.Desativar();

        // Act
        produto.Ativar();

        // Assert
        Assert.IsTrue(produto.Ativo);
    }

    [TestMethod]
    public void DeveSubstituir_ComplementosDoProduto()
    {
        // Arrange
        var complementoInicial =
            CriarComplementoValido("Bacon");

        var novoComplemento =
            CriarComplementoValido("Queijo");

        var produto = CriarProdutoValido(
            complementos: [complementoInicial]
        );

        // Act
        produto.SubstituirComplementos(
            [novoComplemento]
        );

        // Assert
        Assert.HasCount(
            1,
            produto.Complementos
        );

        Assert.AreSame(
            novoComplemento,
            produto.Complementos[0]
        );

        Assert.DoesNotContain(
            complementoInicial,
            produto.Complementos
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoExistiremComplementosComMesmoNome()
    {
        // Arrange
        var complemento1 =
            CriarComplementoValido("Bacon");

        var complemento2 =
            CriarComplementoValido("bacon");

        var produto = CriarProdutoValido(
            complementos: [
                complemento1,
                complemento2
            ]
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Produto.Complementos))
        );
    }

    [TestMethod]
    public void DeveValidar_ComplementosDoProduto()
    {
        // Arrange
        var complementoInvalido =
            CriarComplementoValido(
                nome: "A",
                precoAdicional: -5
            );

        var produto = CriarProdutoValido(
            complementos: [complementoInvalido]
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            produto.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Complemento.Nome))
        );

        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Complemento.PrecoAdicional))
        );
    }

    [TestMethod]
    public void DeveAtualizar_DadosDoProduto()
    {
        // Arrange
        Guid produtoId = Guid.CreateVersion7();
        Guid estabelecimentoId = Guid.CreateVersion7();
        Guid categoriaId = Guid.CreateVersion7();

        var produto = CriarProdutoValido(
            id: produtoId,
            estabelecimentoId: estabelecimentoId
        );

        var produtoAtualizado = new Produto(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            categoriaId,
            "Produto Atualizado",
            "Descrição atualizada",
            25.50m
        );

        // Act
        produto.Atualizar(produtoAtualizado);

        // Assert
        Assert.AreEqual(
            produtoId,
            produto.Id
        );

        Assert.AreEqual(
            estabelecimentoId,
            produto.EstabelecimentoId
        );

        Assert.AreEqual(
            categoriaId,
            produto.CategoriaId
        );

        Assert.AreEqual(
            "Produto Atualizado",
            produto.Nome
        );

        Assert.AreEqual(
            "Descrição atualizada",
            produto.Descricao
        );

        Assert.AreEqual(
            25.50m,
            produto.Preco
        );
    }

    private static Produto CriarProdutoValido(
        Guid? id = null,
        Guid? estabelecimentoId = null,
        Guid? categoriaId = null,
        string nome = "Hambúrguer",
        string descricao = "Hambúrguer artesanal",
        decimal preco = 25.90m,
        IEnumerable<Complemento>? complementos = null)
    {
        return new Produto(
            id ?? Guid.CreateVersion7(),
            estabelecimentoId ?? Guid.CreateVersion7(),
            categoriaId ?? Guid.CreateVersion7(),
            nome,
            descricao,
            preco,
            complementos
        );
    }

    private static Complemento CriarComplementoValido(
        string nome = "Bacon",
        decimal precoAdicional = 5.00m)
    {
        return new Complemento(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            nome,
            precoAdicional
        );
    }
}
