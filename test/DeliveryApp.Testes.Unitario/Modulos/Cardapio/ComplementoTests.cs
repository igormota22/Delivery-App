
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Modulos.Cardapio;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class ComplementoTests
{
    [TestMethod]
    public void DeveRetornar_ErroQuandoNomePossuirMenosDeDoisCaracteres()
    {
        // Arrange
        var complemento = CriarComplementoValido(
            nome: "A"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            complemento.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Complemento.Nome))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoNomePossuirMaisDeCemCaracteres()
    {
        // Arrange
        string nome = new('A', 101);

        var complemento = CriarComplementoValido(
            nome: nome
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            complemento.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Complemento.Nome))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoPrecoAdicionalForNegativo()
    {
        // Arrange
        var complemento = CriarComplementoValido(
            precoAdicional: -1
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            complemento.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Complemento.PrecoAdicional))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoPrecoAdicionalPossuirMaisDeDuasCasasDecimais()
    {
        // Arrange
        var complemento = CriarComplementoValido(
            precoAdicional: 5.123m
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            complemento.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Complemento.PrecoAdicional))
        );
    }

    [TestMethod]
    public void DeveRetornar_ErroQuandoProdutoNaoForInformado()
    {
        // Arrange
        var complemento = CriarComplementoValido(
            produtoId: Guid.Empty
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            complemento.Validar();

        // Assert
        Assert.IsTrue(
            erros.Any(e =>
                e.Campo == nameof(Complemento.ProdutoId))
        );
    }

    [TestMethod]
    public void DeveSerValido_QuandoComplementoPossuirDadosValidos()
    {
        // Arrange
        var complemento = CriarComplementoValido();

        // Act
        IReadOnlyList<ErroValidacao> erros =
            complemento.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void DeveAceitar_PrecoAdicionalIgualAZero()
    {
        // Arrange
        var complemento = CriarComplementoValido(
            precoAdicional: 0
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            complemento.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void DeveAtualizar_DadosDoComplemento()
    {
        // Arrange
        Guid complementoId = Guid.CreateVersion7();
        Guid produtoId = Guid.CreateVersion7();

        var complemento = CriarComplementoValido(
            id: complementoId,
            produtoId: produtoId,
            nome: "Bacon",
            precoAdicional: 5.00m
        );

        var complementoAtualizado = new Complemento(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            "Queijo",
            7.50m
        );

        // Act
        complemento.Atualizar(
            complementoAtualizado
        );

        // Assert
        Assert.AreEqual(
            complementoId,
            complemento.Id
        );

        Assert.AreEqual(
            produtoId,
            complemento.ProdutoId
        );

        Assert.AreEqual(
            "Queijo",
            complemento.Nome
        );

        Assert.AreEqual(
            7.50m,
            complemento.PrecoAdicional
        );
    }

    private static Complemento CriarComplementoValido(
        Guid? id = null,
        Guid? produtoId = null,
        string nome = "Bacon",
        decimal precoAdicional = 5.00m)
    {
        return new Complemento(
            id ?? Guid.CreateVersion7(),
            produtoId ?? Guid.CreateVersion7(),
            nome,
            precoAdicional
        );
    }
}
