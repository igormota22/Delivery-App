using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;

namespace DeliveryApp.Testes.Unidade.Modulos.Estabelecimentos;

[TestClass]
public class EstabelecimentoTests
{
    [TestMethod]
    public void Deve_Criar_Estabelecimento_Com_Dados_Validos()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        string nomeComercial = "Pizzaria Teste";
        string documento = "12345678000199";
        string endereco = "Rua Teste, 123";
        string telefone = "49999999999";
        string areaAtendimento = "Centro";
        TimeOnly horarioAbertura = new(18, 0);
        TimeOnly horarioFechamento = new(23, 0);
        decimal taxaEntrega = 5.00m;

        // Act
        Estabelecimento estabelecimento = new(
            id,
            nomeComercial,
            documento,
            endereco,
            telefone,
            areaAtendimento,
            horarioAbertura,
            horarioFechamento,
            taxaEntrega
        );

        // Assert
        Assert.AreEqual(id, estabelecimento.Id);
        Assert.AreEqual(nomeComercial, estabelecimento.NomeComercial);
        Assert.AreEqual(documento, estabelecimento.Documento);
        Assert.AreEqual(endereco, estabelecimento.Endereco);
        Assert.AreEqual(telefone, estabelecimento.Telefone);
        Assert.AreEqual(areaAtendimento, estabelecimento.AreaAtendimento);
        Assert.AreEqual(horarioAbertura, estabelecimento.HorarioAbertura);
        Assert.AreEqual(horarioFechamento, estabelecimento.HorarioFechamento);
        Assert.AreEqual(taxaEntrega, estabelecimento.TaxaEntrega);
        Assert.IsTrue(estabelecimento.Ativo);
    }

    [TestMethod]
    public void Deve_Remover_Formatacao_Do_Documento_Ao_Criar_Estabelecimento()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        string documento = "12.345.678/0001-99";

        // Act
        Estabelecimento estabelecimento = new(
            id,
            "Pizzaria Teste",
            documento,
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Assert
        Assert.AreEqual(
            "12345678000199",
            estabelecimento.Documento
        );
    }

    [TestMethod]
    public void Deve_Remover_Formatacao_Do_Telefone_Ao_Criar_Estabelecimento()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();
        string telefone = "(49) 99999-9999";

        // Act
        Estabelecimento estabelecimento = new(
            id,
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            telefone,
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Assert
        Assert.AreEqual(
            "49999999999",
            estabelecimento.Telefone
        );
    }

    [TestMethod]
    public void Deve_Remover_Espacos_Do_Nome_Ao_Criar_Estabelecimento()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        // Act
        Estabelecimento estabelecimento = new(
            id,
            "   Pizzaria Teste   ",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Assert
        Assert.AreEqual(
            "Pizzaria Teste",
            estabelecimento.NomeComercial
        );
    }

    [TestMethod]
    public void Deve_Remover_Espacos_Do_Endereco_Ao_Criar_Estabelecimento()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        // Act
        Estabelecimento estabelecimento = new(
            id,
            "Pizzaria Teste",
            "12345678000199",
            "   Rua Teste, 123   ",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Assert
        Assert.AreEqual(
            "Rua Teste, 123",
            estabelecimento.Endereco
        );
    }

    [TestMethod]
    public void Deve_Remover_Espacos_Da_Area_Atendimento_Ao_Criar_Estabelecimento()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        // Act
        Estabelecimento estabelecimento = new(
            id,
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "   Centro   ",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Assert
        Assert.AreEqual(
            "Centro",
            estabelecimento.AreaAtendimento
        );
    }

    [TestMethod]
    public void Nao_Deve_Apresentar_Erros_Com_Dados_Validos()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0),
            5.00m
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsEmpty(erros);
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Nome_Comercial_Possuir_Menos_De_2_Caracteres()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "A",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.NomeComercial)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Nome_Comercial_Possuir_Mais_De_100_Caracteres()
    {
        // Arrange
        string nomeComercial = new('A', 101);

        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            nomeComercial,
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.NomeComercial)));
    }

    [TestMethod]
    public void Deve_Aceitar_Nome_Comercial_Com_Exatamente_2_Caracteres()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "AB",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsFalse(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.NomeComercial)));
    }

    [TestMethod]
    public void Deve_Aceitar_Nome_Comercial_Com_Exatamente_100_Caracteres()
    {
        // Arrange
        string nomeComercial = new('A', 100);

        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            nomeComercial,
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsFalse(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.NomeComercial)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Documento_Possuir_Menos_De_11_Digitos()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "1234567890",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Documento)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Documento_Possuir_Quantidade_Invalida_De_Digitos()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "123456789012",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Documento)));
    }

    [TestMethod]
    public void Deve_Aceitar_Documento_Com_11_Digitos()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678901",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsFalse(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Documento)));
    }

    [TestMethod]
    public void Deve_Aceitar_Documento_Com_14_Digitos()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsFalse(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Documento)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Documento_Possuir_Caracteres_Nao_Numericos()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "1234567890A",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Documento)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Endereco_Possuir_Menos_De_5_Caracteres()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Endereco)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Endereco_Possuir_Mais_De_250_Caracteres()
    {
        // Arrange
        string endereco = new('A', 251);

        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            endereco,
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Endereco)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Telefone_Possuir_Menos_De_10_Digitos()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "499999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Telefone)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Telefone_Possuir_Mais_De_11_Digitos()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "499999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.Telefone)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Horario_Abertura_For_Igual_Ao_Horario_Fechamento()
    {
        // Arrange
        TimeOnly horario = new(18, 0);

        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            horario,
            horario
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.HorarioFechamento)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Area_Atendimento_Possuir_Menos_De_2_Caracteres()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "A",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.AreaAtendimento)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Area_Atendimento_Possuir_Mais_De_150_Caracteres()
    {
        // Arrange
        string areaAtendimento = new('A', 151);

        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            areaAtendimento,
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.AreaAtendimento)));
    }

    [TestMethod]
    public void Deve_Apresentar_Erro_Quando_Taxa_Entrega_For_Negativa()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0),
            -1
        );

        // Act
        IReadOnlyList<ErroValidacao> erros =
            estabelecimento.Validar();

        // Assert
        Assert.IsTrue(erros.Any(e =>
            e.Campo == nameof(Estabelecimento.TaxaEntrega)));
    }

    [TestMethod]
    public void Deve_Ativar_Estabelecimento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        estabelecimento.Desativar();

        // Act
        estabelecimento.Ativar();

        // Assert
        Assert.IsTrue(estabelecimento.Ativo);
    }

    [TestMethod]
    public void Deve_Desativar_Estabelecimento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        estabelecimento.Desativar();

        // Assert
        Assert.IsFalse(estabelecimento.Ativo);
    }

    [TestMethod]
    public void Deve_Atualizar_Dados_Do_Estabelecimento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Antiga",
            "12345678000199",
            "Rua Antiga, 100",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0),
            5.00m
        );

        Estabelecimento estabelecimentoAtualizado = new(
            estabelecimento.Id,
            "Pizzaria Nova",
            "98.765.432/0001-00",
            "Avenida Nova, 500",
            "(48) 98888-7777",
            "Centro e bairros",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            10.50m
        );

        // Act
        estabelecimento.Atualizar(estabelecimentoAtualizado);

        // Assert
        Assert.AreEqual(
            "Pizzaria Nova",
            estabelecimento.NomeComercial
        );

        Assert.AreEqual(
            "98765432000100",
            estabelecimento.Documento
        );

        Assert.AreEqual(
            "Avenida Nova, 500",
            estabelecimento.Endereco
        );

        Assert.AreEqual(
            "48988887777",
            estabelecimento.Telefone
        );

        Assert.AreEqual(
            "Centro e bairros",
            estabelecimento.AreaAtendimento
        );

        Assert.AreEqual(
            new TimeOnly(10, 0),
            estabelecimento.HorarioAbertura
        );

        Assert.AreEqual(
            new TimeOnly(22, 0),
            estabelecimento.HorarioFechamento
        );

        Assert.AreEqual(
            10.50m,
            estabelecimento.TaxaEntrega
        );
    }

    [TestMethod]
    public void Deve_Manter_Id_Ao_Atualizar_Estabelecimento()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        Estabelecimento estabelecimento = new(
            id,
            "Pizzaria Antiga",
            "12345678000199",
            "Rua Antiga, 100",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        Estabelecimento estabelecimentoAtualizado = new(
            Guid.CreateVersion7(),
            "Pizzaria Nova",
            "98765432000100",
            "Avenida Nova, 500",
            "48988887777",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0)
        );

        // Act
        estabelecimento.Atualizar(estabelecimentoAtualizado);

        // Assert
        Assert.AreEqual(id, estabelecimento.Id);
    }

    [TestMethod]
    public void Deve_Estar_Disponivel_Dentro_Do_Horario()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        bool disponivel = estabelecimento.EstaDisponivel(
            new TimeOnly(20, 0)
        );

        // Assert
        Assert.IsTrue(disponivel);
    }

    [TestMethod]
    public void Deve_Estar_Indisponivel_Antes_Do_Horario_De_Abertura()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        bool disponivel = estabelecimento.EstaDisponivel(
            new TimeOnly(17, 59)
        );

        // Assert
        Assert.IsFalse(disponivel);
    }

    [TestMethod]
    public void Deve_Estar_Indisponivel_No_Horario_De_Fechamento()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        // Act
        bool disponivel = estabelecimento.EstaDisponivel(
            new TimeOnly(23, 0)
        );

        // Assert
        Assert.IsFalse(disponivel);
    }

    [TestMethod]
    public void Deve_Estar_Indisponivel_Quando_Estabelecimento_Estiver_Desativado()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(23, 0)
        );

        estabelecimento.Desativar();

        // Act
        bool disponivel = estabelecimento.EstaDisponivel(
            new TimeOnly(20, 0)
        );

        // Assert
        Assert.IsFalse(disponivel);
    }

    [TestMethod]
    public void Deve_Estar_Disponivel_Quando_Horario_Atravessar_Meia_Noite()
    {
        // Arrange
        Estabelecimento estabelecimento = new(
            Guid.CreateVersion7(),
            "Pizzaria Teste",
            "12345678000199",
            "Rua Teste, 123",
            "49999999999",
            "Centro",
            new TimeOnly(18, 0),
            new TimeOnly(2, 0)
        );

        // Act
        bool disponivelAntesDaMeiaNoite =
            estabelecimento.EstaDisponivel(
                new TimeOnly(20, 0)
            );

        bool disponivelDepoisDaMeiaNoite =
            estabelecimento.EstaDisponivel(
                new TimeOnly(1, 0)
            );

        bool indisponivelDepoisDoFechamento =
            estabelecimento.EstaDisponivel(
                new TimeOnly(3, 0)
            );

        // Assert
        Assert.IsTrue(disponivelAntesDaMeiaNoite);
        Assert.IsTrue(disponivelDepoisDaMeiaNoite);
        Assert.IsFalse(indisponivelDepoisDoFechamento);
    }
}