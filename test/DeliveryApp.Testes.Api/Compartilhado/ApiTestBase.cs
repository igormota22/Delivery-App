
using DeliveryApp.Infraestrutura.Compartilhado.Orm;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace DeliveryApp.Testes.Api.Compartilhado;

public abstract class ApiTestBase
{
    protected static WebApplicationFactory<Program> Factory { get; } = new();

    protected HttpClient Client { get; private set; } = null!;

    private readonly List<Guid> clientesCriados = [];
    private readonly List<Guid> estabelecimentosCriados = [];

    [TestInitialize]
    public void Inicializar()
    {
        Client = Factory.CreateClient();
    }

    protected void RegistrarClienteCriado(Guid clienteId)
    {
        clientesCriados.Add(clienteId);
    }

    protected void RegistrarEstabelecimentoCriado(Guid estabelecimentoId)
    {
        estabelecimentosCriados.Add(estabelecimentoId);
    }

    [TestCleanup]
    public async Task Finalizar()
    {
        Console.WriteLine("=== CLEANUP INICIADO ===");

        Client.Dispose();

        using IServiceScope scope = Factory.Services.CreateScope();

        DeliveryAppDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<DeliveryAppDbContext>();

        UserManager<IdentityUser<Guid>> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

        foreach (Guid clienteId in clientesCriados)
        {
            Console.WriteLine($"Removendo cliente: {clienteId}");

            await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                DELETE FROM "TBClientes"
                WHERE "Id" = {clienteId};
                """);

            IdentityUser<Guid>? usuario =
                await userManager.FindByIdAsync(clienteId.ToString());

            if (usuario is not null)
            {
                IdentityResult resultado =
                    await userManager.DeleteAsync(usuario);

                Console.WriteLine(
                    $"Usuário removido: {resultado.Succeeded}"
                );
            }
        }

        foreach (Guid estabelecimentoId in estabelecimentosCriados)
        {
            Console.WriteLine(
                $"Removendo estabelecimento: {estabelecimentoId}"
            );

            await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                DELETE FROM "TBEstabelecimentos"
                WHERE "Id" = {estabelecimentoId};
                """);

            IdentityUser<Guid>? usuario =
                await userManager.FindByIdAsync(
                    estabelecimentoId.ToString()
                );

            if (usuario is not null)
            {
                IdentityResult resultado =
                    await userManager.DeleteAsync(usuario);

                Console.WriteLine(
                    $"Usuário removido: {resultado.Succeeded}"
                );
            }
        }

        Console.WriteLine("=== CLEANUP FINALIZADO ===");
    }
}
