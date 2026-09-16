
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DeliveryApp.Testes.Api.Compartilhado;
using DeliveryApp.WebApi.Modulos.Estabelecimentos;

namespace DeliveryApp.Testes.Api.Modulos.Estabelecimentos;

[TestClass]
[DoNotParallelize]
public class EstabelecimentosApiTests : ApiTestBase
{
    [TestMethod]
    public async Task DeveCadastrar_Estabelecimento()
    {
        string documento = GerarDocumento();
        string email = GerarEmail();
        string senha = "Senha@123";

        var request = CriarCadastroRequest(
            documento,
            email,
            senha
        );

        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/cadastro",
                request
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"STATUS: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine($"CORPO: {corpo}");

        Assert.AreEqual(
            HttpStatusCode.Created,
            response.StatusCode
        );

        CadastrarEstabelecimentoResponse? resultado =
            await response.Content
                .ReadFromJsonAsync<CadastrarEstabelecimentoResponse>();

        Assert.IsNotNull(resultado);

        Console.WriteLine(
            $"ESTABELECIMENTO CRIADO: {resultado.Id}"
        );

        // Registra imediatamente para o cleanup.
        RegistrarEstabelecimentoCriado(resultado.Id);

        Assert.AreNotEqual(
            Guid.Empty,
            resultado.Id
        );

        Assert.AreEqual(
            request.NomeComercial,
            resultado.NomeComercial
        );
    }

    [TestMethod]
    public async Task DeveAutenticar_Estabelecimento()
    {
        string documento = GerarDocumento();
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = CriarCadastroRequest(
            documento,
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/cadastro",
                cadastroRequest
            );

        string cadastroCorpo =
            await cadastroResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"CADASTRO: {(int)cadastroResponse.StatusCode} - {cadastroResponse.StatusCode}"
        );

        Console.WriteLine(
            $"CADASTRO CORPO: {cadastroCorpo}"
        );

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarEstabelecimentoResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarEstabelecimentoResponse>();

        Assert.IsNotNull(cadastro);

        // Registra antes de continuar o teste.
        RegistrarEstabelecimentoCriado(cadastro.Id);

        var loginRequest = new AutenticarEstabelecimentoRequest(
            cadastroRequest.Email,
            cadastroRequest.Senha
        );

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/login",
                loginRequest
            );

        string loginCorpo =
            await loginResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"LOGIN: {(int)loginResponse.StatusCode} - {loginResponse.StatusCode}"
        );

        Console.WriteLine(
            $"LOGIN CORPO: {loginCorpo}"
        );

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarEstabelecimentoResponse? resultado =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarEstabelecimentoResponse>();

        Assert.IsNotNull(resultado);

        Assert.AreEqual(
            cadastro.Id,
            resultado.EstabelecimentoId
        );

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(resultado.AccessToken)
        );

        Assert.IsTrue(
            resultado.DataExpiracaoEmUtc > DateTime.UtcNow
        );
    }

    [TestMethod]
    public async Task DeveObter_EstabelecimentoAutenticado()
    {
        string documento = GerarDocumento();
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = CriarCadastroRequest(
            documento,
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/cadastro",
                cadastroRequest
            );

        string cadastroCorpo =
            await cadastroResponse.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"CADASTRO: {(int)cadastroResponse.StatusCode} - {cadastroResponse.StatusCode}"
        );

        Console.WriteLine(
            $"CADASTRO CORPO: {cadastroCorpo}"
        );

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarEstabelecimentoResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarEstabelecimentoResponse>();

        Assert.IsNotNull(cadastro);

        RegistrarEstabelecimentoCriado(cadastro.Id);

        var loginRequest = new AutenticarEstabelecimentoRequest(
            cadastroRequest.Email,
            cadastroRequest.Senha
        );

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/login",
                loginRequest
            );

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarEstabelecimentoResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarEstabelecimentoResponse>();

        Assert.IsNotNull(login);

        Assert.AreEqual(
            cadastro.Id,
            login.EstabelecimentoId
        );

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );

        HttpResponseMessage response =
            await Client.GetAsync(
                $"/api/estabelecimentos/{cadastro.Id}"
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"GET: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine(
            $"GET CORPO: {corpo}"
        );

        Assert.AreEqual(
            HttpStatusCode.OK,
            response.StatusCode
        );

        EstabelecimentoResponse? estabelecimento =
            await response.Content
                .ReadFromJsonAsync<EstabelecimentoResponse>();

        Assert.IsNotNull(estabelecimento);

        Assert.AreEqual(
            cadastro.Id,
            estabelecimento.Id
        );

        Assert.AreEqual(
            cadastroRequest.NomeComercial,
            estabelecimento.NomeComercial
        );

        Assert.AreEqual(
            cadastroRequest.Documento,
            estabelecimento.Documento
        );

        Assert.AreEqual(
            cadastroRequest.Endereco,
            estabelecimento.Endereco
        );

        Assert.AreEqual(
            cadastroRequest.Telefone,
            estabelecimento.Telefone
        );

        Assert.AreEqual(
            cadastroRequest.AreaAtendimento,
            estabelecimento.AreaAtendimento
        );

        Assert.AreEqual(
            cadastroRequest.HorarioAbertura,
            estabelecimento.HorarioAbertura
        );

        Assert.AreEqual(
            cadastroRequest.HorarioFechamento,
            estabelecimento.HorarioFechamento
        );

        Assert.AreEqual(
            cadastroRequest.TaxaEntrega,
            estabelecimento.TaxaEntrega
        );
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoObterEstabelecimentoSemToken()
    {
        Guid estabelecimentoId = Guid.NewGuid();

        HttpResponseMessage response =
            await Client.GetAsync(
                $"/api/estabelecimentos/{estabelecimentoId}"
            );

        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveListar_EstabelecimentosDisponiveis()
    {
        string documento = GerarDocumento();
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = CriarCadastroRequest(
            documento,
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/cadastro",
                cadastroRequest
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarEstabelecimentoResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarEstabelecimentoResponse>();

        Assert.IsNotNull(cadastro);

        RegistrarEstabelecimentoCriado(cadastro.Id);

        var loginRequest = new AutenticarEstabelecimentoRequest(
            email,
            senha
        );

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/login",
                loginRequest
            );

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarEstabelecimentoResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarEstabelecimentoResponse>();

        Assert.IsNotNull(login);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );

        HttpResponseMessage response =
            await Client.GetAsync(
                "/api/estabelecimentos/disponiveis"
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"DISPONIVEIS: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine(
            $"DISPONIVEIS CORPO: {corpo}"
        );

        Assert.AreEqual(
            HttpStatusCode.OK,
            response.StatusCode
        );

        List<EstabelecimentoResponse>? resultado =
            await response.Content
                .ReadFromJsonAsync<List<EstabelecimentoResponse>>();

        Assert.IsNotNull(resultado);

        Assert.IsTrue(
            resultado.Any(x => x.Id == cadastro.Id)
        );
    }

    [TestMethod]
    public async Task DeveEditar_EstabelecimentoAutenticado()
    {
        string documento = GerarDocumento();
        string email = GerarEmail();
        string senha = "Senha@123";

        var cadastroRequest = CriarCadastroRequest(
            documento,
            email,
            senha
        );

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/cadastro",
                cadastroRequest
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarEstabelecimentoResponse? cadastro =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarEstabelecimentoResponse>();

        Assert.IsNotNull(cadastro);

        RegistrarEstabelecimentoCriado(cadastro.Id);

        var loginRequest = new AutenticarEstabelecimentoRequest(
            email,
            senha
        );

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/login",
                loginRequest
            );

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarEstabelecimentoResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarEstabelecimentoResponse>();

        Assert.IsNotNull(login);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );

        var request = new EditarEstabelecimentoRequest(
            "Restaurante Atualizado",
            documento,
            "Rua Nova, 200",
            "48988888888",
            "Centro e Regiao",
            new TimeOnly(9, 0),
            new TimeOnly(23, 0)
        );

        HttpResponseMessage response =
            await Client.PutAsJsonAsync(
                $"/api/estabelecimentos/{cadastro.Id}",
                request
            );

        string corpo =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(
            $"EDITAR: {(int)response.StatusCode} - {response.StatusCode}"
        );

        Console.WriteLine(
            $"EDITAR CORPO: {corpo}"
        );

        Assert.AreEqual(
            HttpStatusCode.NoContent,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoEditarEstabelecimentoSemToken()
    {
        Guid estabelecimentoId = Guid.NewGuid();

        var request = new EditarEstabelecimentoRequest(
            "Restaurante Atualizado",
            GerarDocumento(),
            "Rua Nova, 200",
            "48988888888",
            "Centro",
            new TimeOnly(9, 0),
            new TimeOnly(23, 0)
        );

        HttpResponseMessage response =
            await Client.PutAsJsonAsync(
                $"/api/estabelecimentos/{estabelecimentoId}",
                request
            );

        Assert.AreEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveRejeitar_LoginComCredenciaisInvalidas()
    {
        var request = new AutenticarEstabelecimentoRequest(
            GerarEmail(),
            "SenhaInvalida"
        );

        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/login",
                request
            );

        Assert.AreNotEqual(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }

    private static CadastrarEstabelecimentoRequest CriarCadastroRequest(
        string documento,
        string email,
        string senha)
    {
        return new CadastrarEstabelecimentoRequest(
            "Restaurante do Igor",
            documento,
            "Rua das Flores, 100",
            "49999999999",
            "Centro",
            new TimeOnly(10, 0),
            new TimeOnly(22, 0),
            email,
            senha,
            5.00m
        );
    }

    private static string GerarEmail()
    {
        return $"igor{Guid.NewGuid():N}@email.com";
    }

    private static string GerarDocumento()
    {
        long numero = Random.Shared.NextInt64(
            10000000000,
            99999999999
        );

        return numero.ToString();
    }
}
