using DeliveryApp.Aplicacao.Compartilhado;
using DeliveryApp.Aplicacao.Modulos.Clientes;
using DeliveryApp.Dominio.Compartilhado;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Clientes;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Clientes;

[TestClass]
public class CadastrarClienteCommandHandlerTests
{
    private Mock<IRepositorioCliente> repositorioClienteMock = null!;
    private Mock<IGerenciadorDeIdentidade> gerenciadorDeIdentidadeMock = null!;
    private CadastrarClienteCommandHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioClienteMock = new Mock<IRepositorioCliente>();
        gerenciadorDeIdentidadeMock = new Mock<IGerenciadorDeIdentidade>();

        handler = new CadastrarClienteCommandHandler(
            repositorioClienteMock.Object,
            gerenciadorDeIdentidadeMock.Object
        );
    }

    [TestMethod]
    public async Task DeveCadastrar_ClienteComSucesso()
    {
        // Arrange
        CadastrarClienteCommand command = new(
            "Igor Mello",
            "123.456.789-01",
            "igor@email.com",
            "Senha123!"
        );

        repositorioClienteMock
            .Setup(x => x.ExisteRegistroComCpfAsync(
                "12345678901",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        gerenciadorDeIdentidadeMock
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ))
            .ReturnsAsync(
                new UsuarioDto(
                    It.IsAny<Guid>(),
                    command.Email
                )
            );

        repositorioClienteMock
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(Task.CompletedTask);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreNotEqual(Guid.Empty, resultado.Value);

        repositorioClienteMock.Verify(
            x => x.ExisteRegistroComCpfAsync(
                "12345678901",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        gerenciadorDeIdentidadeMock.Verify(
            x => x.CadastrarAsync(
                resultado.Value,
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ),
            Times.Once
        );

        repositorioClienteMock.Verify(
            x => x.CadastrarAsync(
                It.Is<Cliente>(cliente =>
                    cliente.Id == resultado.Value &&
                    cliente.Nome == "Igor Mello" &&
                    cliente.Cpf == "12345678901"
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoClienteForInvalido()
    {
        // Arrange
        CadastrarClienteCommand command = new(
            "I",
            "123",
            "igor@email.com",
            "Senha123!"
        );

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioClienteMock.Verify(
            x => x.ExisteRegistroComCpfAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        gerenciadorDeIdentidadeMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TipoUsuario>()
            ),
            Times.Never
        );

        repositorioClienteMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoCpfEstiverDuplicado()
    {
        // Arrange
        CadastrarClienteCommand command = new(
            "Igor Mello",
            "123.456.789-01",
            "igor@email.com",
            "Senha123!"
        );

        repositorioClienteMock
            .Setup(x => x.ExisteRegistroComCpfAsync(
                "12345678901",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(true);

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        gerenciadorDeIdentidadeMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<TipoUsuario>()
            ),
            Times.Never
        );

        repositorioClienteMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoIdentityRetornarErroDeValidacao()
    {
        // Arrange
        CadastrarClienteCommand command = new(
            "Igor Mello",
            "123.456.789-01",
            "igor@email.com",
            "senha"
        );

        repositorioClienteMock
            .Setup(x => x.ExisteRegistroComCpfAsync(
                "12345678901",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        var excecao = new ValidacaoDeIdentidadeException(
         "Campo",
         "Valor inválido."
 );

        gerenciadorDeIdentidadeMock
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ))
            .ThrowsAsync(excecao);
        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioClienteMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoIdentityRetornarConflito()
    {
        // Arrange
        CadastrarClienteCommand command = new(
            "Igor Mello",
            "123.456.789-01",
            "igor@email.com",
            "Senha123!"
        );

        repositorioClienteMock
            .Setup(x => x.ExisteRegistroComCpfAsync(
                "12345678901",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        gerenciadorDeIdentidadeMock
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ))
            .ThrowsAsync(
                new ConflitoDeIdentidadeException(
                    "Já existe um usuário cadastrado com este e-mail."
                )
            );

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioClienteMock.Verify(
            x => x.CadastrarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveExcluirUsuario_QuandoOcorrerConflitoDePersistencia()
    {
        // Arrange
        CadastrarClienteCommand command = new(
            "Igor Mello",
            "123.456.789-01",
            "igor@email.com",
            "Senha123!"
        );

        repositorioClienteMock
            .Setup(x => x.ExisteRegistroComCpfAsync(
                "12345678901",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(false);

        gerenciadorDeIdentidadeMock
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Guid>(),
                command.Email,
                command.Senha,
                TipoUsuario.Cliente
            ))
            .ReturnsAsync(
                new UsuarioDto(
                    Guid.NewGuid(),
                    command.Email
                )
            );

        repositorioClienteMock
            .Setup(x => x.CadastrarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(
                new ConflitoDePersistenciaException("Já existe um cliente cadastrado com este email ou CPF.", new Exception())
            );

        // Act
        Result<Guid> resultado = await handler.Handle(command);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        gerenciadorDeIdentidadeMock.Verify(
            x => x.ExcluirAsync(
                It.IsAny<Guid>()
            ),
            Times.Once
        );
    }
}