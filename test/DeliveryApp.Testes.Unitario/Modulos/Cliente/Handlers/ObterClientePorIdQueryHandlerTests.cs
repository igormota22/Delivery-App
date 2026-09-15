using DeliveryApp.Aplicacao.Modulos.Clientes;
using DeliveryApp.Aplicacao.Modulos.Clientes.DTOs;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Clientes;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Clientes;

[TestClass]
public class ObterClientePorIdQueryHandlerTests
{
    private Mock<IRepositorioCliente> repositorioClienteMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;
    private ObterClientePorIdQueryHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioClienteMock = new Mock<IRepositorioCliente>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        handler = new ObterClientePorIdQueryHandler(
            repositorioClienteMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ClienteQuandoUsuarioForAutorizado()
    {
        // Arrange
        Guid clienteId = Guid.NewGuid();

        ObterClientePorIdQuery query = new(clienteId);

        Cliente cliente = new(
            clienteId,
            "Igor Mello",
            "123.456.789-01"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(clienteId);

        provedorDeUsuarioMock
            .SetupGet(x => x.Email)
            .Returns("igor@email.com");

        repositorioClienteMock
            .Setup(x => x.SelecionarPorIdAsync(
                clienteId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(cliente);

        // Act
        Result<ClienteDto> resultado = await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        ClienteDto clienteDto = resultado.Value;

        Assert.AreEqual(clienteId, clienteDto.Id);
        Assert.AreEqual("Igor Mello", clienteDto.Nome);
        Assert.AreEqual("12345678901", clienteDto.Cpf);
        Assert.AreEqual("igor@email.com", clienteDto.Email);

        repositorioClienteMock.Verify(
            x => x.SelecionarPorIdAsync(
                clienteId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoUsuarioNaoForAutorizado()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();
        Guid outroClienteId = Guid.NewGuid();

        ObterClientePorIdQuery query = new(outroClienteId);

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(usuarioId);

        // Act
        Result<ClienteDto> resultado = await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioClienteMock.Verify(
            x => x.SelecionarPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoClienteNaoForEncontrado()
    {
        // Arrange
        Guid clienteId = Guid.NewGuid();

        ObterClientePorIdQuery query = new(clienteId);

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(clienteId);

        repositorioClienteMock
            .Setup(x => x.SelecionarPorIdAsync(
                clienteId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((Cliente?)null);

        // Act
        Result<ClienteDto> resultado = await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioClienteMock.Verify(
            x => x.SelecionarPorIdAsync(
                clienteId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}