
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.DTOs;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Estabelecimentos;

[TestClass]
public class ObterEstabelecimentoPorIdQueryHandlerTests
{
    private Mock<IRepositorioEstabelecimento> repositorioEstabelecimento = null!;
    private ObterEstabelecimentoPorIdQueryHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioEstabelecimento = new Mock<IRepositorioEstabelecimento>();

        handler = new ObterEstabelecimentoPorIdQueryHandler(
            repositorioEstabelecimento.Object
        );
    }

    [TestMethod]
    public async Task Deve_RetornarEstabelecimento_QuandoExistir()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        var estabelecimento = CriarEstabelecimentoValido(estabelecimentoId);

        repositorioEstabelecimento
            .Setup(x => x.SelecionarPorIdAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(estabelecimento);

        var query = new ObterEstabelecimentoPorIdQuery(
            estabelecimentoId
        );

        // Act
        Result<EstabelecimentoDto> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);

        Assert.AreEqual(
            estabelecimento.Id,
            resultado.Value.Id
        );

        Assert.AreEqual(
            estabelecimento.NomeComercial,
            resultado.Value.NomeComercial
        );

        Assert.AreEqual(
            estabelecimento.Documento,
            resultado.Value.Documento
        );

        Assert.AreEqual(
            estabelecimento.Endereco,
            resultado.Value.Endereco
        );

        Assert.AreEqual(
            estabelecimento.Telefone,
            resultado.Value.Telefone
        );

        Assert.AreEqual(
            estabelecimento.AreaAtendimento,
            resultado.Value.AreaAtendimento
        );

        Assert.AreEqual(
            estabelecimento.HorarioAbertura,
            resultado.Value.HorarioAbertura
        );

        Assert.AreEqual(
            estabelecimento.HorarioFechamento,
            resultado.Value.HorarioFechamento
        );

        Assert.AreEqual(
            estabelecimento.TaxaEntrega,
            resultado.Value.TaxaEntrega
        );

        Assert.AreEqual(
            estabelecimento.Ativo,
            resultado.Value.Ativo
        );

        repositorioEstabelecimento.Verify(
            x => x.SelecionarPorIdAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoEstabelecimentoNaoForEncontrado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.CreateVersion7();

        repositorioEstabelecimento
            .Setup(x => x.SelecionarPorIdAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Estabelecimento?)null);

        var query = new ObterEstabelecimentoPorIdQuery(
            estabelecimentoId
        );

        // Act
        Result<EstabelecimentoDto> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.SelecionarPorIdAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private static Estabelecimento CriarEstabelecimentoValido(Guid estabelecimentoId)
    {
        return new Estabelecimento(
            estabelecimentoId,
            "Restaurante do Igor",
            "12345678000199",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            5.00m
        );
    }
}
