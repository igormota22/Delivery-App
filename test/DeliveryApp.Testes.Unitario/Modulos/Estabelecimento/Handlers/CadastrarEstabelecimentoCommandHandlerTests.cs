
using DeliveryApp.Aplicacao.Modulos.Estabelecimentos;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Estabelecimentos;

[TestClass]
public class CadastrarEstabelecimentoCommandHandlerTests
{
    private Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidade = null!;
    private Mock<IRepositorioEstabelecimento> repositorioEstabelecimento = null!;
    private CadastrarEstabelecimentoCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        gerenciadorDeIdentidade = new Mock<IGerenciadorDeIdentidade>();
        repositorioEstabelecimento = new Mock<IRepositorioEstabelecimento>();

        handler = new CadastrarEstabelecimentoCommandHandler(
            gerenciadorDeIdentidade.Object,
            repositorioEstabelecimento.Object
        );
    }

    [TestMethod]
    public async Task Deve_CadastrarEstabelecimento_ComDadosValidos()
    {
        // Arrange
        string nomeComercial = "Restaurante do Igor";
        string documento = "12345678000199";
        string endereco = "Rua das Flores, 100";
        string telefone = "49999999999";
        string areaAtendimento = "Centro";
        TimeOnly horarioAbertura = new(10, 0);
        TimeOnly horarioFechamento = new(22, 0);
        string email = "estabelecimento@email.com";
        string senha = "Senha@123";
        decimal taxaEntrega = 5.00m;

        gerenciadorDeIdentidade
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha,
                TipoUsuario.Estabelecimento))
            .ReturnsAsync(new UsuarioDto(
                It.IsAny<Guid>(),
                email
            ));

        repositorioEstabelecimento
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CadastrarEstabelecimentoCommand(
            nomeComercial,
            documento,
            endereco,
            telefone,
            areaAtendimento,
            horarioAbertura,
            horarioFechamento,
            email,
            senha,
            taxaEntrega
        );

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreNotEqual(Guid.Empty, resultado.Value);

        gerenciadorDeIdentidade.Verify(
            x => x.CadastrarAsync(
                resultado.Value,
                email,
                senha,
                TipoUsuario.Estabelecimento),
            Times.Once
        );

        repositorioEstabelecimento.Verify(
            x => x.CadastrarAsync(
                It.Is<Estabelecimento>(e =>
                    e.Id == resultado.Value &&
                    e.NomeComercial == nomeComercial &&
                    e.Documento == documento &&
                    e.Endereco == endereco &&
                    e.Telefone == telefone &&
                    e.AreaAtendimento == areaAtendimento &&
                    e.HorarioAbertura == horarioAbertura &&
                    e.HorarioFechamento == horarioFechamento &&
                    e.TaxaEntrega == taxaEntrega),
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoDadosDoEstabelecimentoForemInvalidos()
    {
        // Arrange
        var command = new CadastrarEstabelecimentoCommand(
            "",
            "12345678000199",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            "estabelecimento@email.com",
            "Senha@123",
            5.00m
        );

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        gerenciadorDeIdentidade.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TipoUsuario>()),
            Times.Never
        );

        repositorioEstabelecimento.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoValidacaoDeIdentidadeFalhar()
    {
        // Arrange
        string email = "estabelecimento@email.com";
        string senha = "Senha@123";

        gerenciadorDeIdentidade
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha,
                TipoUsuario.Estabelecimento))
            .ThrowsAsync(
                new ValidacaoDeIdentidadeException(
                    "Email",
                    "E-mail inválido."
                ));

        var command = new CadastrarEstabelecimentoCommand(
            "Restaurante do Igor",
            "12345678000199",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            email,
            senha,
            5.00m
        );

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Deve_RetornarErro_QuandoIdentidadeJaExistir()
    {
        // Arrange
        string email = "estabelecimento@email.com";
        string senha = "Senha@123";

        gerenciadorDeIdentidade
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha,
                TipoUsuario.Estabelecimento))
            .ThrowsAsync(
                new ConflitoDeIdentidadeException(
                    "Já existe um usuário com esse e-mail."
                ));

        var command = new CadastrarEstabelecimentoCommand(
            "Restaurante do Igor",
            "12345678000199",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            email,
            senha,
            5.00m
        );

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        repositorioEstabelecimento.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task Deve_ExcluirIdentidade_QuandoCadastroDoEstabelecimentoForDuplicado()
    {
        // Arrange
        string email = "estabelecimento@email.com";
        string senha = "Senha@123";

        gerenciadorDeIdentidade
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                email,
                senha,
                TipoUsuario.Estabelecimento))
            .ReturnsAsync(new UsuarioDto(
                It.IsAny<Guid>(),
                email
            ));

        repositorioEstabelecimento
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Estabelecimento>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new ConflitoDePersistenciaException("Ja existe um estabelecimento com esse Email e senha", new Exception()));

        var command = new CadastrarEstabelecimentoCommand(
            "Restaurante do Igor",
            "12345678000199",
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            email,
            senha,
            5.00m
        );

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        gerenciadorDeIdentidade.Verify(
            x => x.ExcluirAsync(It.IsAny<Guid>()),
            Times.Once
        );
    }
}
