using DeliveryApp.Dominio.Modulos.Clientes;
using DeliveryApp.Testes.Integracao.Compartilhado;
using DeliveryApp.Testes.Integracao.Compartilhado.Orm;

namespace DeliveryApp.Testes.Integracao.Modulos.Clientes;

[TestClass]
public class RepositorioClienteTestEmOrm : RepositorioBaseEmOrmTests
{
    [TestMethod]
    public async Task DeveCadastrar_Cliente()
    {
        // Arrange
        Cliente cliente = new(
            Guid.NewGuid(),
            "Igor Mello",
            "123.456.789-01"
        );

        // Act
        await repositorioCliente.CadastrarAsync(cliente);

        // Assert
        Cliente? clienteSelecionado =
            await repositorioCliente.SelecionarPorIdAsync(cliente.Id);

        Assert.IsNotNull(clienteSelecionado);
        Assert.AreEqual(cliente.Id, clienteSelecionado.Id);
        Assert.AreEqual("Igor Mello", clienteSelecionado.Nome);
        Assert.AreEqual("12345678901", clienteSelecionado.Cpf);
    }

    [TestMethod]
    public async Task DeveSelecionar_ClientePorId()
    {
        // Arrange
        Cliente cliente = new(
            Guid.NewGuid(),
            "Igor Mello",
            "123.456.789-01"
        );

        await repositorioCliente.CadastrarAsync(cliente);

        // Act
        Cliente? clienteSelecionado =
            await repositorioCliente.SelecionarPorIdAsync(cliente.Id);

        // Assert
        Assert.IsNotNull(clienteSelecionado);

        Assert.AreEqual(cliente.Id, clienteSelecionado.Id);
        Assert.AreEqual(cliente.Nome, clienteSelecionado.Nome);
        Assert.AreEqual(cliente.Cpf, clienteSelecionado.Cpf);
    }

    [TestMethod]
    public async Task DeveEditar_Cliente()
    {
        // Arrange
        Cliente cliente = new(
            Guid.NewGuid(),
            "Igor Mello",
            "123.456.789-01"
        );

        await repositorioCliente.CadastrarAsync(cliente);

        Cliente clienteAtualizado = new(
            cliente.Id,
            "Tiago Santini",
            "012.334.455-01"
        );

        // Act
        await repositorioCliente.EditarAsync(
            cliente.Id,
            clienteAtualizado
        );

        // Assert
        Cliente? clienteSelecionado =
            await repositorioCliente.SelecionarPorIdAsync(cliente.Id);

        Assert.IsNotNull(clienteSelecionado);

        Assert.AreEqual(cliente.Id, clienteSelecionado.Id);
        Assert.AreEqual(clienteAtualizado.Nome, clienteSelecionado.Nome);
        Assert.AreEqual(clienteAtualizado.Cpf, clienteSelecionado.Cpf);
    }

    [TestMethod]
    public async Task DeveSelecionarTodos_Clientes()
    {
        // Arrange
        Cliente cliente1 = new(
            Guid.NewGuid(),
            "Igor Mello",
            "123.456.789-01"
        );

        Cliente cliente2 = new(
            Guid.NewGuid(),
            "Tiago Santini",
            "012.334.455-01"
        );

        await repositorioCliente.CadastrarAsync(cliente1);
        await repositorioCliente.CadastrarAsync(cliente2);

        // Act
        List<Cliente> clientes =
            await repositorioCliente.SelecionarTodosAsync();

        // Assert
        Assert.HasCount(2, clientes);
    }

    [TestMethod]
    public async Task DeveExcluir_Cliente()
    {
        // Arrange
        Cliente cliente = new(
            Guid.NewGuid(),
            "Igor Mello",
            "123.456.789-01"
        );

        await repositorioCliente.CadastrarAsync(cliente);

        // Act
        bool resultado =
            await repositorioCliente.ExcluirAsync(cliente.Id);

        // Assert
        Assert.IsTrue(resultado);

        Cliente? clienteSelecionado =
            await repositorioCliente.SelecionarPorIdAsync(cliente.Id);

        Assert.IsNull(clienteSelecionado);
    }
}