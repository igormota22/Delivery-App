
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos.DTOs;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Estabelecimentos;

[TestClass]
public class ListarEstabelecimentosDisponiveisQueryHandlerTests
{
    private Mock<IRepositorioEstabelecimento> repositorioEstabelecimento = null!;
    private ListarEstabelecimentosDisponiveisQueryHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioEstabelecimento = new Mock<IRepositorioEstabelecimento>();

        handler = new ListarEstabelecimentosDisponiveisQueryHandler(
            repositorioEstabelecimento.Object
        );
    }

    [TestMethod]
    public async Task Deve_RetornarEstabelecimentosDisponiveis()
    {
        // Arrange
        var estabelecimento1 = CriarEstabelecimentoValido();
        var estabelecimento2 = CriarEstabelecimentoValido();

        repositorioEstabelecimento
            .Setup(x => x.SelecionarDisponiveisAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Estabelecimento>
            {
                estabelecimento1,
                estabelecimento2
            });

        var query = new ListarEstabelecimentosDisponiveisQuery();

        // Act
        Result<IReadOnlyList<EstabelecimentoDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);

        Assert.HasCount(2, resultado.Value);

        Assert.AreEqual(
            estabelecimento1.Id,
            resultado.Value[0].Id
        );

        Assert.AreEqual(
            estabelecimento1.NomeComercial,
            resultado.Value[0].NomeComercial
        );

        Assert.AreEqual(
            estabelecimento2.Id,
            resultado.Value[1].Id
        );

        Assert.AreEqual(
            estabelecimento2.NomeComercial,
            resultado.Value[1].NomeComercial
        );

        repositorioEstabelecimento.Verify(
            x => x.SelecionarDisponiveisAsync(
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_RetornarListaVazia_QuandoNaoHouverEstabelecimentosDisponiveis()
    {
        // Arrange
        repositorioEstabelecimento
            .Setup(x => x.SelecionarDisponiveisAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Estabelecimento>());

        var query = new ListarEstabelecimentosDisponiveisQuery();

        // Act
        Result<IReadOnlyList<EstabelecimentoDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.IsEmpty(resultado.Value);

        repositorioEstabelecimento.Verify(
            x => x.SelecionarDisponiveisAsync(
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_ConverterEstabelecimentoParaDto()
    {
        // Arrange
        var estabelecimento = CriarEstabelecimentoValido();

        repositorioEstabelecimento
            .Setup(x => x.SelecionarDisponiveisAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Estabelecimento>
            {
                estabelecimento
            });

        var query = new ListarEstabelecimentosDisponiveisQuery();

        // Act
        Result<IReadOnlyList<EstabelecimentoDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        EstabelecimentoDto dto = resultado.Value[0];

        Assert.AreEqual(estabelecimento.Id, dto.Id);
        Assert.AreEqual(estabelecimento.NomeComercial, dto.NomeComercial);
        Assert.AreEqual(estabelecimento.Documento, dto.Documento);
        Assert.AreEqual(estabelecimento.Endereco, dto.Endereco);
        Assert.AreEqual(estabelecimento.Telefone, dto.Telefone);
        Assert.AreEqual(estabelecimento.AreaAtendimento, dto.AreaAtendimento);
        Assert.AreEqual(estabelecimento.HorarioAbertura, dto.HorarioAbertura);
        Assert.AreEqual(estabelecimento.HorarioFechamento, dto.HorarioFechamento);
        Assert.AreEqual(estabelecimento.TaxaEntrega, dto.TaxaEntrega);
        Assert.AreEqual(estabelecimento.Ativo, dto.Ativo);
    }

    private static Estabelecimento CriarEstabelecimentoValido()
    {
        return new Estabelecimento(
            Guid.CreateVersion7(),
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
