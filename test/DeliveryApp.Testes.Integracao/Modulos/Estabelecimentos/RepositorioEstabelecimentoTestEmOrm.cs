using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using DeliveryApp.Testes.Integracao.Compartilhado.Orm;

namespace DeliveryApp.Testes.Integracao.Modulos.Estabelecimentos;

[TestClass]
public class RepositorioEstabelecimentoTestEmOrm : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public async Task DeveCadastrar_Estabelecimento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.NewGuid(),
            "Pizzaria Igor",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "(49) 99999-9999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0),
            5.00m
        );

        // Act
        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento
        );

        // Assert
        Estabelecimento? estabelecimentoSelecionado =
            await repositorioEstabelecimento.SelecionarPorIdAsync(
                estabelecimento.Id
            );

        Assert.IsNotNull(estabelecimentoSelecionado);

        Assert.AreEqual(
            estabelecimento.Id,
            estabelecimentoSelecionado.Id
        );

        Assert.AreEqual(
            "Pizzaria Igor",
            estabelecimentoSelecionado.NomeComercial
        );

        Assert.AreEqual(
            "12345678000199",
            estabelecimentoSelecionado.Documento
        );

        Assert.AreEqual(
            "Rua das Flores, 100",
            estabelecimentoSelecionado.Endereco
        );

        Assert.AreEqual(
            "49999999999",
            estabelecimentoSelecionado.Telefone
        );

        Assert.AreEqual(
            "Centro",
            estabelecimentoSelecionado.AreaAtendimento
        );

        Assert.AreEqual(
            new TimeOnly(18, 0),
            estabelecimentoSelecionado.HorarioAbertura
        );

        Assert.AreEqual(
            new TimeOnly(23, 0),
            estabelecimentoSelecionado.HorarioFechamento
        );

        Assert.AreEqual(
            5.00m,
            estabelecimentoSelecionado.TaxaEntrega
        );

        Assert.IsTrue(
            estabelecimentoSelecionado.Ativo
        );
    }

    [TestMethod]
    public async Task DeveSelecionar_EstabelecimentoPorId()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.NewGuid(),
            "Pizzaria Igor",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "(49) 99999-9999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0),
            5.00m
        );

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento
        );

        // Act
        Estabelecimento? estabelecimentoSelecionado =
            await repositorioEstabelecimento.SelecionarPorIdAsync(
                estabelecimento.Id
            );

        // Assert
        Assert.IsNotNull(estabelecimentoSelecionado);

        Assert.AreEqual(
            estabelecimento.Id,
            estabelecimentoSelecionado.Id
        );

        Assert.AreEqual(
            estabelecimento.NomeComercial,
            estabelecimentoSelecionado.NomeComercial
        );

        Assert.AreEqual(
            estabelecimento.Documento,
            estabelecimentoSelecionado.Documento
        );
    }

    [TestMethod]
    public async Task DeveEditar_Estabelecimento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.NewGuid(),
            "Pizzaria Igor",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "(49) 99999-9999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0),
            5.00m
        );

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento
        );

        Estabelecimento estabelecimentoAtualizado = new(
            estabelecimento.Id,
            "Restaurante Igor",
            "98.765.432/0001-00",
            "Avenida Brasil, 500",
            "(48) 98888-7777",
            "Centro e bairros",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            10.50m
        );

        // Act
        await repositorioEstabelecimento.EditarAsync(
            estabelecimento.Id,
            estabelecimentoAtualizado
        );

        // Assert
        Estabelecimento? estabelecimentoSelecionado =
            await repositorioEstabelecimento.SelecionarPorIdAsync(
                estabelecimento.Id
            );

        Assert.IsNotNull(estabelecimentoSelecionado);

        Assert.AreEqual(
            estabelecimento.Id,
            estabelecimentoSelecionado.Id
        );

        Assert.AreEqual(
            estabelecimentoAtualizado.NomeComercial,
            estabelecimentoSelecionado.NomeComercial
        );

        Assert.AreEqual(
            estabelecimentoAtualizado.Documento,
            estabelecimentoSelecionado.Documento
        );

        Assert.AreEqual(
            estabelecimentoAtualizado.Endereco,
            estabelecimentoSelecionado.Endereco
        );

        Assert.AreEqual(
            estabelecimentoAtualizado.Telefone,
            estabelecimentoSelecionado.Telefone
        );

        Assert.AreEqual(
            estabelecimentoAtualizado.AreaAtendimento,
            estabelecimentoSelecionado.AreaAtendimento
        );

        Assert.AreEqual(
            estabelecimentoAtualizado.HorarioAbertura,
            estabelecimentoSelecionado.HorarioAbertura
        );

        Assert.AreEqual(
            estabelecimentoAtualizado.HorarioFechamento,
            estabelecimentoSelecionado.HorarioFechamento
        );

        Assert.AreEqual(
            estabelecimentoAtualizado.TaxaEntrega,
            estabelecimentoSelecionado.TaxaEntrega
        );
    }

    [TestMethod]
    public async Task DeveSelecionarTodos_Estabelecimentos()
    {
        // Arrange
        Estabelecimento estabelecimento1 = new(
            Guid.NewGuid(),
            "Pizzaria Igor",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        Estabelecimento estabelecimento2 = new(
            Guid.NewGuid(),
            "Restaurante Tiago",
            "98.765.432/0001-00",
            "Avenida Brasil, 500",
            "48988887777",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0)
        );

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento1
        );

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento2
        );

        // Act
        List<Estabelecimento> estabelecimentos =
            await repositorioEstabelecimento.SelecionarTodosAsync();

        // Assert
        Assert.HasCount(2, estabelecimentos);
    }

    [TestMethod]
    public async Task DeveExcluir_Estabelecimento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.NewGuid(),
            "Pizzaria Igor",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento
        );

        // Act
        bool resultado =
            await repositorioEstabelecimento.ExcluirAsync(
                estabelecimento.Id
            );

        // Assert
        Assert.IsTrue(resultado);

        Estabelecimento? estabelecimentoSelecionado =
            await repositorioEstabelecimento.SelecionarPorIdAsync(
                estabelecimento.Id
            );

        Assert.IsNull(estabelecimentoSelecionado);
    }

    [TestMethod]
    public async Task DeveSelecionarDisponiveis_EstabelecimentosAtivos()
    {
        // Arrange
        Estabelecimento estabelecimentoAtivo = new(
            Guid.NewGuid(),
            "Pizzaria Ativa",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        Estabelecimento estabelecimentoInativo = new(
            Guid.NewGuid(),
            "Restaurante Inativo",
            "98.765.432/0001-00",
            "Avenida Brasil, 500",
            "48988887777",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0)
        );

        estabelecimentoInativo.Desativar();

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimentoAtivo
        );

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimentoInativo
        );

        // Act
        List<Estabelecimento> estabelecimentos =
            await repositorioEstabelecimento.SelecionarDisponiveisAsync();

        // Assert
        Assert.HasCount(1, estabelecimentos);

        Assert.AreEqual(
            estabelecimentoAtivo.Id,
            estabelecimentos[0].Id
        );

        Assert.IsTrue(
            estabelecimentos[0].Ativo
        );
    }


    [TestMethod]
    public async Task DeveAlterarAtivo_Estabelecimento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.NewGuid(),
            "Pizzaria Igor",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento
        );

        // Act
        bool resultado =
            await repositorioEstabelecimento.AlterarAtivoAsync(
                estabelecimento.Id,
                false
            );

        // Assert
        Assert.IsTrue(resultado);

        Estabelecimento? estabelecimentoSelecionado =
            await repositorioEstabelecimento.SelecionarPorIdAsync(
                estabelecimento.Id
            );

        Assert.IsNotNull(estabelecimentoSelecionado);

        Assert.IsFalse(
            estabelecimentoSelecionado.Ativo
        );
    }

    [TestMethod]
    public async Task DeveAtivar_Estabelecimento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.NewGuid(),
            "Pizzaria Igor",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        estabelecimento.Desativar();

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento
        );

        // Act
        bool resultado =
            await repositorioEstabelecimento.AlterarAtivoAsync(
                estabelecimento.Id,
                true
            );

        // Assert
        Assert.IsTrue(resultado);

        Estabelecimento? estabelecimentoSelecionado =
            await repositorioEstabelecimento.SelecionarPorIdAsync(
                estabelecimento.Id
            );

        Assert.IsNotNull(estabelecimentoSelecionado);

        Assert.IsTrue(
            estabelecimentoSelecionado.Ativo
        );
    }

    [TestMethod]
    public async Task DeveRetornarFalso_AoAlterarAtivo_DeEstabelecimentoInexistente()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        // Act
        bool resultado =
            await repositorioEstabelecimento.AlterarAtivoAsync(
                estabelecimentoId,
                true
            );

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public async Task DeveSelecionar_EstabelecimentoParaPedido()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.NewGuid(),
            "Pizzaria Igor",
            "12.345.678/0001-99",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        await repositorioEstabelecimento.CadastrarAsync(
            estabelecimento
        );

        // Act
        Estabelecimento? estabelecimentoSelecionado =
            await repositorioEstabelecimento.SelecionarParaPedidoAsync(
                estabelecimento.Id,
                CancellationToken.None
            );

        // Assert
        Assert.IsNotNull(estabelecimentoSelecionado);

        Assert.AreEqual(
            estabelecimento.Id,
            estabelecimentoSelecionado.Id
        );

        Assert.AreEqual(
            estabelecimento.NomeComercial,
            estabelecimentoSelecionado.NomeComercial
        );
    }

    [TestMethod]
    public async Task DeveRetornarNulo_AoSelecionarEstabelecimentoParaPedidoInexistente()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        // Act
        Estabelecimento? estabelecimento =
            await repositorioEstabelecimento.SelecionarParaPedidoAsync(
                estabelecimentoId,
                CancellationToken.None
            );

        // Assert
        Assert.IsNull(estabelecimento);
    }
}