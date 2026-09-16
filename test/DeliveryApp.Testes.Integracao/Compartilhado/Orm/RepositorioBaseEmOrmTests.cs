using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Dominio.Modulos.Clientes;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using DeliveryApp.Infraestrutura.Compartilhado.Orm;
using DeliveryApp.Infraestrutura.Modulos.Cardapio;
using DeliveryApp.Infraestrutura.Modulos.Clientes;
using DeliveryApp.Infraestrutura.Modulos.Estabelecimentos;
using DeliveryApp.Testes.Integracao.Compartilhado.Identity;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrmTests
{
    protected DeliveryAppDbContext dbContext = null!;

    // =========================================================
    // REPOSITÓRIOS
    // =========================================================

    protected RepositorioClienteEmOrm repositorioCliente = null!;
    protected RepositorioEstabelecimentoEmOrm repositorioEstabelecimento = null!;
    protected RepositorioCategoriaEmOrm repositorioCategoria = null!;
    protected RepositorioProdutoEmOrm repositorioProduto = null!;




    // =========================================================
    // INICIALIZAÇÃO
    // =========================================================
    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext(Guid.NewGuid());

        repositorioCliente = new RepositorioClienteEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Cliente>(
            cliente => repositorioCliente
                .CadastrarAsync(cliente)
                .GetAwaiter()
                .GetResult()
        );

        BuilderSetup.SetCreatePersistenceMethod<IList<Cliente>>(
            clientes =>
            {
                foreach (Cliente cliente in clientes)
                {
                    repositorioCliente
                        .CadastrarAsync(cliente)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        );

        repositorioEstabelecimento = new RepositorioEstabelecimentoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Estabelecimento>(
            estabelecimento => repositorioEstabelecimento
                .CadastrarAsync(estabelecimento)
                .GetAwaiter()
                .GetResult()
        );

        BuilderSetup.SetCreatePersistenceMethod<IList<Estabelecimento>>(
            estabelecimentos =>
            {
                foreach (Estabelecimento estabelecimento in estabelecimentos)
                {
                    repositorioEstabelecimento
                        .CadastrarAsync(estabelecimento)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        );

        repositorioCategoria = new RepositorioCategoriaEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Categoria>(
            categoria => repositorioCategoria
                .CadastrarAsync(categoria)
                .GetAwaiter()
                .GetResult()
        );

        BuilderSetup.SetCreatePersistenceMethod<IList<Categoria>>(
            categorias =>
            {
                foreach (Categoria categoria in categorias)
                {
                    repositorioCategoria
                        .CadastrarAsync(categoria)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        );

        repositorioProduto = new RepositorioProdutoEmOrm(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Produto>(
            produto => repositorioProduto
                .CadastrarAsync(produto)
                .GetAwaiter()
                .GetResult()
        );

        BuilderSetup.SetCreatePersistenceMethod<IList<Produto>>(
            produtos =>
            {
                foreach (Produto produto in produtos)
                {
                    repositorioProduto
                        .CadastrarAsync(produto)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        );
    }

    // =========================================================
    // LIMPEZA
    // =========================================================

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }


    // =========================================================
    // CRIAÇÃO DO CONTEXTO
    // =========================================================

    private static DeliveryAppDbContext CriarDbContext(
        Guid userId
    )
    {
        DbContextOptions<DeliveryAppDbContext> options =
            new DbContextOptionsBuilder<DeliveryAppDbContext>()
                .UseInMemoryDatabase(
                    $"integracao-{Guid.NewGuid():N}"
                )
                .Options;

        return new DeliveryAppDbContext(
            options,
            new ProvedorDeUsuarioFake(userId)
        );
    }
}