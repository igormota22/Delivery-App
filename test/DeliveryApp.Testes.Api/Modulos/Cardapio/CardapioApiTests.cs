
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DeliveryApp.Testes.Api.Compartilhado;
using DeliveryApp.WebApi.Modulos.Cardapio;
using DeliveryApp.WebApi.Modulos.Estabelecimentos;

namespace DeliveryApp.Testes.Api.Modulos.Cardapio;

[TestClass]
[DoNotParallelize]
public class CardapioApiTests : ApiTestBase
{
    [TestMethod]
    public async Task DeveObter_CardapioSemToken()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        Client.DefaultRequestHeaders.Authorization = null;

        HttpResponseMessage response = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/cardapio"
        );

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        CardapioResponse? resultado =
            await response.Content.ReadFromJsonAsync<CardapioResponse>();

        Assert.IsNotNull(resultado);
        Assert.AreEqual(estabelecimentoId, resultado.EstabelecimentoId);
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoListarCategoriasSemToken()
    {
        Guid estabelecimentoId = Guid.NewGuid();

        HttpResponseMessage response = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/categorias"
        );

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveCadastrar_Categoria()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        CadastrarCategoriaRequest request =
            new("Lanches");

        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/categorias",
            request
        );

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        CategoriaResponse? resultado =
            await response.Content.ReadFromJsonAsync<CategoriaResponse>();

        Assert.IsNotNull(resultado);

        RegistrarCategoriaCriada(resultado.Id);

        Assert.AreNotEqual(Guid.Empty, resultado.Id);
        Assert.AreEqual("Lanches", resultado.Nome);
    }

    [TestMethod]
    public async Task DeveListar_Categorias()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        CategoriaResponse categoria = await CadastrarCategoria(
            estabelecimentoId,
            "Lanches"
        );

        HttpResponseMessage response = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/categorias"
        );

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        List<CategoriaResponse>? resultado =
            await response.Content.ReadFromJsonAsync<List<CategoriaResponse>>();

        Assert.IsNotNull(resultado);

        CategoriaResponse? categoriaEncontrada =
            resultado.SingleOrDefault(x => x.Id == categoria.Id);

        Assert.IsNotNull(categoriaEncontrada);
        Assert.AreEqual("Lanches", categoriaEncontrada.Nome);
    }

    [TestMethod]
    public async Task DeveEditar_Categoria()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        CategoriaResponse categoria = await CadastrarCategoria(
            estabelecimentoId,
            "Lanches"
        );

        CadastrarCategoriaRequest request =
            new("Hambúrgueres");

        HttpResponseMessage response = await Client.PutAsJsonAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/categorias/{categoria.Id}",
            request
        );

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

        HttpResponseMessage consulta = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/categorias"
        );

        Assert.AreEqual(HttpStatusCode.OK, consulta.StatusCode);

        List<CategoriaResponse>? categorias =
            await consulta.Content.ReadFromJsonAsync<List<CategoriaResponse>>();

        Assert.IsNotNull(categorias);

        CategoriaResponse? categoriaEditada =
            categorias.SingleOrDefault(x => x.Id == categoria.Id);

        Assert.IsNotNull(categoriaEditada);
        Assert.AreEqual("Hambúrgueres", categoriaEditada.Nome);
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoEditarCategoriaSemToken()
    {
        Guid estabelecimentoId = Guid.NewGuid();
        Guid categoriaId = Guid.NewGuid();

        CadastrarCategoriaRequest request =
            new("Hambúrgueres");

        HttpResponseMessage response = await Client.PutAsJsonAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/categorias/{categoriaId}",
            request
        );

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveCadastrar_Produto()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        CategoriaResponse categoria = await CadastrarCategoria(
            estabelecimentoId,
            "Lanches"
        );

        CadastrarProdutoRequest request = new(
            categoria.Id,
            "X-Bacon",
            "Hambúrguer com bacon e queijo",
            25.90m,
            []
        );

        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos",
            request
        );

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        CadastroProdutoResponse? resultado =
            await response.Content.ReadFromJsonAsync<CadastroProdutoResponse>();

        Assert.IsNotNull(resultado);

        RegistrarProdutoCriado(resultado.Id);

        Assert.AreNotEqual(Guid.Empty, resultado.Id);
    }

    [TestMethod]
    public async Task DeveCadastrar_ProdutoComComplementos()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        CategoriaResponse categoria = await CadastrarCategoria(
            estabelecimentoId,
            "Lanches"
        );

        CadastrarProdutoRequest request = new(
            categoria.Id,
            "X-Bacon",
            "Hambúrguer com bacon e queijo",
            25.90m,
            [
                new ComplementoRequest("Bacon Extra", 5.00m),
                new ComplementoRequest("Queijo Extra", 3.50m)
            ]
        );

        HttpResponseMessage cadastro = await Client.PostAsJsonAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos",
            request
        );

        Assert.AreEqual(HttpStatusCode.Created, cadastro.StatusCode);

        CadastroProdutoResponse? resultado =
            await cadastro.Content.ReadFromJsonAsync<CadastroProdutoResponse>();

        Assert.IsNotNull(resultado);

        RegistrarProdutoCriado(resultado.Id);

        HttpResponseMessage consulta = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos"
        );

        Assert.AreEqual(HttpStatusCode.OK, consulta.StatusCode);

        List<ProdutoResponse>? produtos =
            await consulta.Content.ReadFromJsonAsync<List<ProdutoResponse>>();

        Assert.IsNotNull(produtos);

        ProdutoResponse? produto =
            produtos.SingleOrDefault(x => x.Id == resultado.Id);

        Assert.IsNotNull(produto);
        Assert.AreEqual("X-Bacon", produto.Nome);
        Assert.AreEqual(2, produto.Complementos.Count);

        Assert.IsTrue(
            produto.Complementos.Any(x =>
                x.Nome == "Bacon Extra" &&
                x.PrecoAdicional == 5.00m)
        );

        Assert.IsTrue(
            produto.Complementos.Any(x =>
                x.Nome == "Queijo Extra" &&
                x.PrecoAdicional == 3.50m)
        );
    }

    [TestMethod]
    public async Task DeveListar_Produtos()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        CategoriaResponse categoria = await CadastrarCategoria(
            estabelecimentoId,
            "Lanches"
        );

        CadastroProdutoResponse produto = await CadastrarProduto(
            estabelecimentoId,
            categoria.Id,
            "X-Bacon",
            "Hambúrguer com bacon",
            25.90m
        );

        HttpResponseMessage response = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos"
        );

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        List<ProdutoResponse>? resultado =
            await response.Content.ReadFromJsonAsync<List<ProdutoResponse>>();

        Assert.IsNotNull(resultado);

        ProdutoResponse? produtoEncontrado =
            resultado.SingleOrDefault(x => x.Id == produto.Id);

        Assert.IsNotNull(produtoEncontrado);
        Assert.AreEqual("X-Bacon", produtoEncontrado.Nome);
        Assert.AreEqual("Hambúrguer com bacon", produtoEncontrado.Descricao);
        Assert.AreEqual(25.90m, produtoEncontrado.Preco);
        Assert.IsTrue(produtoEncontrado.Ativo);
        Assert.AreEqual(categoria.Id, produtoEncontrado.CategoriaId);
        Assert.AreEqual("Lanches", produtoEncontrado.CategoriaNome);
        Assert.AreEqual(0, produtoEncontrado.Complementos.Count);
    }

    [TestMethod]
    public async Task DeveEditar_Produto()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        CategoriaResponse categoria = await CadastrarCategoria(
            estabelecimentoId,
            "Lanches"
        );

        CadastroProdutoResponse produto = await CadastrarProduto(
            estabelecimentoId,
            categoria.Id,
            "X-Bacon",
            "Hambúrguer com bacon",
            25.90m
        );

        CadastrarProdutoRequest request = new(
            categoria.Id,
            "X-Bacon Especial",
            "Hambúrguer artesanal com bacon",
            32.90m,
            [
                new ComplementoRequest("Bacon Extra", 5.00m)
            ]
        );

        HttpResponseMessage response = await Client.PutAsJsonAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos/{produto.Id}",
            request
        );

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

        HttpResponseMessage consulta = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos"
        );

        Assert.AreEqual(HttpStatusCode.OK, consulta.StatusCode);

        List<ProdutoResponse>? produtos =
            await consulta.Content.ReadFromJsonAsync<List<ProdutoResponse>>();

        Assert.IsNotNull(produtos);

        ProdutoResponse? produtoEditado =
            produtos.SingleOrDefault(x => x.Id == produto.Id);

        Assert.IsNotNull(produtoEditado);
        Assert.AreEqual("X-Bacon Especial", produtoEditado.Nome);
        Assert.AreEqual(
            "Hambúrguer artesanal com bacon",
            produtoEditado.Descricao
        );
        Assert.AreEqual(32.90m, produtoEditado.Preco);
        Assert.AreEqual(1, produtoEditado.Complementos.Count);
        Assert.AreEqual("Bacon Extra", produtoEditado.Complementos[0].Nome);
    }

    [TestMethod]
    public async Task DeveAtivarEDesativar_Produto()
    {
        Guid estabelecimentoId = await CriarEstabelecimentoAutenticado();

        CategoriaResponse categoria = await CadastrarCategoria(
            estabelecimentoId,
            "Lanches"
        );

        CadastroProdutoResponse produto = await CadastrarProduto(
            estabelecimentoId,
            categoria.Id,
            "X-Bacon",
            "Hambúrguer com bacon",
            25.90m
        );

        HttpResponseMessage desativar = await Client.PatchAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos/{produto.Id}/desativar",
            null
        );

        Assert.AreEqual(HttpStatusCode.NoContent, desativar.StatusCode);

        HttpResponseMessage consultaDesativado = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos"
        );

        List<ProdutoResponse>? produtosDesativados =
            await consultaDesativado.Content
                .ReadFromJsonAsync<List<ProdutoResponse>>();

        Assert.IsNotNull(produtosDesativados);

        ProdutoResponse? produtoDesativado =
            produtosDesativados.SingleOrDefault(x => x.Id == produto.Id);

        Assert.IsNotNull(produtoDesativado);
        Assert.IsFalse(produtoDesativado.Ativo);

        HttpResponseMessage ativar = await Client.PatchAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos/{produto.Id}/ativar",
            null
        );

        Assert.AreEqual(HttpStatusCode.NoContent, ativar.StatusCode);

        HttpResponseMessage consultaAtivado = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos"
        );

        List<ProdutoResponse>? produtosAtivados =
            await consultaAtivado.Content
                .ReadFromJsonAsync<List<ProdutoResponse>>();

        Assert.IsNotNull(produtosAtivados);

        ProdutoResponse? produtoAtivado =
            produtosAtivados.SingleOrDefault(x => x.Id == produto.Id);

        Assert.IsNotNull(produtoAtivado);
        Assert.IsTrue(produtoAtivado.Ativo);
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoListarProdutosSemToken()
    {
        Guid estabelecimentoId = Guid.NewGuid();

        HttpResponseMessage response = await Client.GetAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos"
        );

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoCadastrarProdutoSemToken()
    {
        Guid estabelecimentoId = Guid.NewGuid();

        CadastrarProdutoRequest request = new(
            Guid.NewGuid(),
            "X-Bacon",
            "Hambúrguer com bacon",
            25.90m,
            []
        );

        HttpResponseMessage response = await Client.PostAsJsonAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos",
            request
        );

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoEditarProdutoSemToken()
    {
        Guid estabelecimentoId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        CadastrarProdutoRequest request = new(
            Guid.NewGuid(),
            "X-Bacon",
            "Hambúrguer com bacon",
            25.90m,
            []
        );

        HttpResponseMessage response = await Client.PutAsJsonAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos/{produtoId}",
            request
        );

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar_UnauthorizedAoAlterarAtivoProdutoSemToken()
    {
        Guid estabelecimentoId = Guid.NewGuid();
        Guid produtoId = Guid.NewGuid();

        HttpResponseMessage response = await Client.PatchAsync(
            $"/api/estabelecimentos/{estabelecimentoId}/produtos/{produtoId}/ativar",
            null
        );

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<Guid> CriarEstabelecimentoAutenticado()
    {
        string documento = GerarDocumento();
        string email = GerarEmail();
        string senha = "Senha@123";

        CadastrarEstabelecimentoRequest cadastro =
            CriarCadastroRequest(documento, email, senha);

        HttpResponseMessage cadastroResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/cadastro",
                cadastro
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            cadastroResponse.StatusCode
        );

        CadastrarEstabelecimentoResponse? estabelecimento =
            await cadastroResponse.Content
                .ReadFromJsonAsync<CadastrarEstabelecimentoResponse>();

        Assert.IsNotNull(estabelecimento);

        RegistrarEstabelecimentoCriado(estabelecimento.Id);

        HttpResponseMessage loginResponse =
            await Client.PostAsJsonAsync(
                "/api/estabelecimentos/login",
                new AutenticarEstabelecimentoRequest(
                    email,
                    senha
                )
            );

        Assert.AreEqual(
            HttpStatusCode.OK,
            loginResponse.StatusCode
        );

        AutenticarEstabelecimentoResponse? login =
            await loginResponse.Content
                .ReadFromJsonAsync<AutenticarEstabelecimentoResponse>();

        Assert.IsNotNull(login);
        Assert.IsFalse(string.IsNullOrWhiteSpace(login.AccessToken));

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken
            );

        return estabelecimento.Id;
    }

    private async Task<CategoriaResponse> CadastrarCategoria(
        Guid estabelecimentoId,
        string nome
    )
    {
        CadastrarCategoriaRequest request =
            new(nome);

        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                $"/api/estabelecimentos/{estabelecimentoId}/categorias",
                request
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            response.StatusCode
        );

        CategoriaResponse? resultado =
            await response.Content
                .ReadFromJsonAsync<CategoriaResponse>();

        Assert.IsNotNull(resultado);

        RegistrarCategoriaCriada(resultado.Id);

        return resultado;
    }

    private async Task<CadastroProdutoResponse> CadastrarProduto(
        Guid estabelecimentoId,
        Guid categoriaId,
        string nome,
        string descricao,
        decimal preco
    )
    {
        CadastrarProdutoRequest request = new(
            categoriaId,
            nome,
            descricao,
            preco,
            []
        );

        HttpResponseMessage response =
            await Client.PostAsJsonAsync(
                $"/api/estabelecimentos/{estabelecimentoId}/produtos",
                request
            );

        Assert.AreEqual(
            HttpStatusCode.Created,
            response.StatusCode
        );

        CadastroProdutoResponse? resultado =
            await response.Content
                .ReadFromJsonAsync<CadastroProdutoResponse>();

        Assert.IsNotNull(resultado);

        RegistrarProdutoCriado(resultado.Id);

        return resultado;
    }

    private static CadastrarEstabelecimentoRequest CriarCadastroRequest(
        string documento,
        string email,
        string senha
    )
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

    private sealed record CadastroProdutoResponse(Guid Id);
}
