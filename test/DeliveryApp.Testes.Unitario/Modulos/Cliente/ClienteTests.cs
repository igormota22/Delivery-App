using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Modulos.Clientes;

namespace DeliveryApp.Testes.Unidade.Modulos.Clientes;

[TestClass]
public class ClienteTests
{
    [TestMethod]
    public void Deve_Criar_Cliente_Com_Dados_Validos()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        string nome = "Cliente Teste";
        string cpf = "12345678901";

        // Act
        Cliente cliente = new(id, nome, cpf);

        // Assert
        Assert.AreEqual(id, cliente.Id);
        Assert.AreEqual(nome, cliente.Nome);
        Assert.AreEqual(cpf, cliente.Cpf);
    }

    [TestMethod]
    public void Deve_Remover_Formatacao_Do_Cpf_Ao_Criar_Cliente()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        string cpf = "123.456.789-01";

        // Act
        Cliente cliente = new(id, "Cliente Teste", cpf);

        // Assert
        Assert.AreEqual("12345678901", cliente.Cpf);
    }

    [TestMethod]
    public void Deve_Remover_Espacos_Do_Cpf_Ao_Criar_Cliente()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        string cpf = "123 456 789 01";

        // Act
        Cliente cliente = new(id, "Cliente Teste", cpf);

        // Assert
        Assert.AreEqual("12345678901", cliente.Cpf);
    }

    [TestMethod]
    public void Deve_Remover_Espacos_Do_Nome_Ao_Criar_Cliente()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        // Act
        Cliente cliente = new(
            id,
            "   Cliente Teste   ",
            "12345678901"
        );

        // Assert
        Assert.AreEqual("Cliente Teste", cliente.Nome);
    }

    [TestMethod]
    public void Nao_Deve_Apresentar_Erros_Com_Dados_Validos()
    {
        // Arrange
        Cliente cliente = new(
            Guid.CreateVersion7(),
            "Cliente Teste",
            "12345678901"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Nome_Possuir_Menos_De_2_Caracteres()
    {
        // Arrange
        Cliente cliente = new(
            Guid.CreateVersion7(),
            "A",
            "12345678901"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Cliente.Nome)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Nome_Possuir_Mais_De_100_Caracteres()
    {
        // Arrange
        string nome = new('A', 101);

        Cliente cliente = new(
            Guid.CreateVersion7(),
            nome,
            "12345678901"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Cliente.Nome)));
    }

    [TestMethod]
    public void Deve_Aceitar_Nome_Com_Exatamente_2_Caracteres()
    {
        // Arrange
        Cliente cliente = new(
            Guid.CreateVersion7(),
            "AB",
            "12345678901"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsFalse(erros.Any(e =>
            e.Campo == nameof(Cliente.Nome)));
    }

    [TestMethod]
    public void Deve_Aceitar_Nome_Com_Exatamente_100_Caracteres()
    {
        // Arrange
        string nome = new('A', 100);

        Cliente cliente = new(
            Guid.CreateVersion7(),
            nome,
            "12345678901"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsFalse(erros.Any(e =>
            e.Campo == nameof(Cliente.Nome)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Cpf_Possuir_Menos_De_11_Digitos()
    {
        // Arrange
        Cliente cliente = new(
            Guid.CreateVersion7(),
            "Cliente Teste",
            "1234567890"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Cliente.Cpf)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Cpf_Possuir_Mais_De_11_Digitos()
    {
        // Arrange
        Cliente cliente = new(
            Guid.CreateVersion7(),
            "Cliente Teste",
            "123456789012"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Cliente.Cpf)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Cpf_Possuir_Caracteres_Nao_Numericos()
    {
        // Arrange
        Cliente cliente = new(
            Guid.CreateVersion7(),
            "Cliente Teste",
            "1234567890A"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Cliente.Cpf)));
    }

    [TestMethod]
    public void Deve_Aceitar_Cpf_Com_Exatamente_11_Digitos()
    {
        // Arrange
        Cliente cliente = new(
            Guid.CreateVersion7(),
            "Cliente Teste",
            "12345678901"
        );

        // Act
        IReadOnlyList<ErroValidacao> erros = cliente.Validar();

        // Assert
        Assert.IsFalse(erros.Any(e =>
            e.Campo == nameof(Cliente.Cpf)));
    }

    [TestMethod]
    public void Deve_Atualizar_Nome_E_Cpf()
    {
        // Arrange
        Cliente cliente = new(
            Guid.CreateVersion7(),
            "Cliente Antigo",
            "12345678901"
        );

        Cliente clienteAtualizado = new(
            cliente.Id,
            "   Cliente Novo   ",
            "987.654.321-00"
        );

        // Act
        cliente.Atualizar(clienteAtualizado);

        // Assert
        Assert.AreEqual("Cliente Novo", cliente.Nome);
        Assert.AreEqual("98765432100", cliente.Cpf);
    }

    [TestMethod]
    public void Deve_Manter_Id_Ao_Atualizar_Cliente()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        Cliente cliente = new(
            id,
            "Cliente Antigo",
            "12345678901"
        );

        Cliente clienteAtualizado = new(
            Guid.CreateVersion7(),
            "Cliente Novo",
            "98765432100"
        );

        // Act
        cliente.Atualizar(clienteAtualizado);

        // Assert
        Assert.AreEqual(id, cliente.Id);
    }
}