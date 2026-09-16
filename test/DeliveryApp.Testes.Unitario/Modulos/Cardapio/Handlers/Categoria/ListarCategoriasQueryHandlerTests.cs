using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Aplicacao.Modulos.Cardapio.Util;
using DeliveryApp.Dominio.Compartilhado.Auth;
using DeliveryApp.Dominio.Modulos.Cardapio;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio;

[TestClass]
public class ListarCategoriasQueryHandlerTests
{
    private Mock<IRepositorioCategoria> repositorioCategoriaMock = null!;
    private Mock<IProvedorDeUsuario> provedorDeUsuarioMock = null!;
    private ListarCategoriasQueryHandler handler = null!;

    [TestInitialize]
    public void Inicializar()
    {
        repositorioCategoriaMock = new Mock<IRepositorioCategoria>();
        provedorDeUsuarioMock = new Mock<IProvedorDeUsuario>();

        handler = new ListarCategoriasQueryHandler(
            repositorioCategoriaMock.Object,
            provedorDeUsuarioMock.Object
        );
    }

    [TestMethod]
    public async Task DeveRetornar_CategoriasQuandoUsuarioForAutorizado()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        ListarCategoriasQuery query = new(estabelecimentoId);

        Categoria categoria1 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Lanches"
        );

        Categoria categoria2 = new(
            Guid.NewGuid(),
            estabelecimentoId,
            "Bebidas"
        );

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarTodosAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync([categoria1, categoria2]);

        // Act
        Result<IReadOnlyList<CategoriaDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        IReadOnlyList<CategoriaDto> categorias = resultado.Value;

        Assert.HasCount(2, categorias);

        Assert.AreEqual(categoria1.Id, categorias[0].Id);
        Assert.AreEqual(categoria1.Nome, categorias[0].Nome);

        Assert.AreEqual(categoria2.Id, categorias[1].Id);
        Assert.AreEqual(categoria2.Nome, categorias[1].Nome);

        repositorioCategoriaMock.Verify(
            x => x.SelecionarTodosAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task DeveRetornar_ListaVaziaQuandoNaoHouverCategorias()
    {
        // Arrange
        Guid estabelecimentoId = Guid.NewGuid();

        ListarCategoriasQuery query = new(estabelecimentoId);

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(estabelecimentoId);

        repositorioCategoriaMock
            .Setup(x => x.SelecionarTodosAsync(
                estabelecimentoId,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync([]);

        // Act
        Result<IReadOnlyList<CategoriaDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        IReadOnlyList<CategoriaDto> categorias = resultado.Value;

        Assert.HasCount(0, categorias);
    }

    [TestMethod]
    public async Task DeveRetornar_ErroQuandoUsuarioNaoForAutorizado()
    {
        // Arrange
        Guid usuarioId = Guid.NewGuid();
        Guid estabelecimentoId = Guid.NewGuid();

        ListarCategoriasQuery query = new(estabelecimentoId);

        provedorDeUsuarioMock
            .SetupGet(x => x.Id)
            .Returns(usuarioId);

        // Act
        Result<IReadOnlyList<CategoriaDto>> resultado =
            await handler.Handle(query);

        // Assert
        Assert.IsTrue(resultado.IsFailed);
        Assert.IsNotEmpty(resultado.Errors);

        repositorioCategoriaMock.Verify(
            x => x.SelecionarTodosAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }

}
