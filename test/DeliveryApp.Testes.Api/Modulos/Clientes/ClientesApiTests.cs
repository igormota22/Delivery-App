using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DeliveryApp.Testes.Api.Compartilhado;
using DeliveryApp.WebApi.Modulos.Clientes;

namespace DeliveryApp.Testes.Api.Modulos.Clientes;

[TestClass]
[DoNotParallelize]
public class ClientesApiTests : ApiTestBase
{
    [TestMethod]
    public async Task DeveCadastrar_Cliente()
    {
        string cpf = GerarCpf();
        string email = GerarEmail();

        var request = new CadastrarClienteRequest(
            "Igor Mello",
            cpf,
            email,
            "Senha@123"
        );

        HttpResponseMessage response = await Client.PostAsJsonAsync(
            "/api/clientes/cadastro",
            request
        );

        string corpo = await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        Assert.AreEqual(
            HttpStatusCode.Created,
            response.StatusCode
        );

        CadastrarClienteResponse? resultado =
            await response.Content
                .ReadFromJsonAsync<CadastrarClienteResponse>();

        Assert.IsNotNull(resultado);

        Console.WriteLine($"CLIENTE CRIADO: {resultado.Id}");

        // Registra imediatamente para o cleanup.
        RegistrarClienteCriado(resultado.Id);

        Assert.AreNotEqual(
            Guid.Empty,
            resultado.Id
        );

        Assert.AreEqual(
            request.Nome,
            resultado.Nome
        );
    }

    [TestMethod]
    public async Task DeveAutenticar_Cliente()
    {
        string cpf = GerarCpf();
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = new CadastrarClienteRequest(
            "Igor Mello",
            cpf,
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/api/clientes/cadastro",
                cadastroRequest
            );

        string cadastroCorpo =
            await cadastroResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"CADASTRO: {(int)cadastroResponse.StatusCode} - {cadastroResponse.StatusCode}"
        );

        Console.WriteLine($"CADASTRO CORPO: {cadastroCorpo}");

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarClienteResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarClienteResponse>();

        Assert.IsNotNull(cadastro);

        // Registra antes de continuar o teste.
        RegistrarClienteCriado(cadastro.Id);

        var loginRequest = new AutenticarClienteRequest(
            cadastroRequest.Email,
            cadastroRequest.Senha
        );

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/api/clientes/login",
                loginRequest
            );

        string loginCorpo =
            await loginResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"LOGIN: {(int)loginResponse.StatusCode} - {loginResponse.StatusCode}"
        );

        Console.WriteLine($"LOGIN CORPO: {loginCorpo}");

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarClienteResponse? resultado =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarClienteResponse>();

        Assert.IsNotNull(resultado);

        Assert.AreEqual(
            cadastro.Id,
            resultado.ClienteId
        );

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(resultado.AccessToken)
        );

        Assert.IsTrue(
            resultado.DataExpiracaoEmUtc > DateTime.UtcNow
        );
    }

    [TestMethod]
    public async Task DeveObter_ClienteAutenticado()
    {
        string cpf = GerarCpf();
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = new CadastrarClienteRequest(
            "Igor Mello",
            cpf,
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/api/clientes/cadastro",
                cadastroRequest
            );

        string cadastroCorpo =
            await cadastroResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"CADASTRO: {(int)cadastroResponse.StatusCode} - {cadastroResponse.StatusCode}"
        );

        Console.WriteLine($"CADASTRO CORPO: {cadastroCorpo}");

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarClienteResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarClienteResponse>();

        Assert.IsNotNull(cadastro);

        // Registra imediatamente para o cleanup.
        RegistrarClienteCriado(cadastro.Id);

        var loginRequest = new AutenticarClienteRequest(
            cadastroRequest.Email,
            cadastroRequest.Senha
        );

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/api/clientes/login",
                loginRequest
            );

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarClienteResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarClienteResponse>();

        Assert.IsNotNull(login);

        Assert.AreEqual(
            cadastro.Id,
            login.ClienteId
        );

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(login.AccessToken)
        );

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );

        HttpResponseMessage response =
            await Client.GetAsync(
                $"/api/clientes/{cadastro.Id}"
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"GET: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"GET CORPO: {corpo}");

        Assert.AreEqual(
            HttpStatusCode.OK,
            response.StatusCode
        );

        ClienteResponse? cliente =
            await response.Content
                .ReadFromJsonAsync<ClienteResponse>();

        Assert.IsNotNull(cliente);

        Assert.AreEqual(
            cadastro.Id,
            cliente.Id
        );

        Assert.AreEqual(
            cadastroRequest.Nome,
            cliente.Nome
        );

        Assert.AreEqual(
            cadastroRequest.Cpf,
            cliente.Cpf
        );

        Assert.AreEqual(
            cadastroRequest.Email,
            cliente.Email
        );
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoObterClienteSemToken()
    {
        Guid clienteId = Guid.NewGuid();

        HttpResponseMessage response =
            await Client.GetAsync(
                $"/api/clientes/{clienteId}"
            );

        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveRejeitar_LoginComCredenciaisInvalidas()
    {
        var request = new AutenticarClienteRequest(
            GerarEmail(),
            "Senha@123"
        );

        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/api/clientes/login",
                request
            );

        Assert.AreNotEqual(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }

    private static string GerarEmail()
    {
        return $"igor{Guid.NewGuid():N}@email.com";
    }

    private static string GerarCpf()
    {
        long numero = Random.Shared.NextInt64(
            10000000000,
            99999999999
        );

        return numero.ToString();
    }
}