using DeliveryApp.Aplicacao.Modulos.Cardapio;
using DeliveryApp.Aplicacao.Modulos.Cardapio.Util;
using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using FluentResults;
using Moq;

namespace DeliveryApp.Testes.Unidade.Modulos.Cardapio
{
    [TestClass]
    public class ObterCardapioQueryHandlerTests
    {
        private Mock<IRepositorioEstabelecimento> repositorioEstabelecimentoMock = null!;
        private Mock<IRepositorioProduto> repositorioProdutoMock = null!;
        private ObterCardapioQueryHandler handler = null!;

        [TestInitialize]
        public void Inicializar()
        {
            repositorioEstabelecimentoMock = new Mock<IRepositorioEstabelecimento>();
            repositorioProdutoMock = new Mock<IRepositorioProduto>();

            handler = new ObterCardapioQueryHandler(
                repositorioEstabelecimentoMock.Object,
                repositorioProdutoMock.Object
            );
        }

        [TestMethod]
        public async Task DeveRetornar_CardapioQuandoEstabelecimentoEstiverAtivo()
        {
            // Arrange
            Guid estabelecimentoId = Guid.NewGuid();
            Guid categoriaId = Guid.NewGuid();

            ObterCardapioQuery query = new(estabelecimentoId);

            Estabelecimento estabelecimento = new(
                estabelecimentoId,
                "Pizzaria",
                "01366999202",
                "Rua Finado Da Silva",
                "(49) 99999-9999",
                "Lages",
                new TimeOnly(19),
                new TimeOnly(23),
                0
            );

            Categoria categoria = new(
                categoriaId,
                estabelecimentoId,
                "Lanches"
            );

            Produto produto = new(
                Guid.NewGuid(),
                estabelecimentoId,
                categoriaId,
                "X-Burger",
                "Hambúrguer artesanal",
                25.90m
            );

            typeof(Produto)
                .GetProperty(nameof(Produto.Categoria))!
                .SetValue(produto, categoria);

            repositorioEstabelecimentoMock
                .Setup(x => x.SelecionarPorIdAsync(
                    estabelecimentoId,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(estabelecimento);

            repositorioProdutoMock
                .Setup(x => x.SelecionarCardapioAsync(
                    estabelecimentoId,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync([produto]);

            // Act
            Result<CardapioDto> resultado = await handler.Handle(query);

            // Assert
            Assert.IsTrue(resultado.IsSuccess);

            CardapioDto cardapioDto = resultado.Value;

            Assert.AreEqual(estabelecimentoId, cardapioDto.EstabelecimentoId);
            Assert.HasCount(1, cardapioDto.Categorias);

            Assert.AreEqual(
                categoriaId,
                cardapioDto.Categorias[0].Id
            );

            Assert.AreEqual(
                "Lanches",
                cardapioDto.Categorias[0].Nome
            );

            Assert.HasCount(
                1,
                cardapioDto.Categorias[0].Produtos
            );

            repositorioEstabelecimentoMock.Verify(
                x => x.SelecionarPorIdAsync(
                    estabelecimentoId,
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );

            repositorioProdutoMock.Verify(
                x => x.SelecionarCardapioAsync(
                    estabelecimentoId,
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task DeveRetornar_ErroQuandoEstabelecimentoNaoForEncontrado()
        {
            // Arrange
            Guid estabelecimentoId = Guid.NewGuid();

            ObterCardapioQuery query = new(estabelecimentoId);

            repositorioEstabelecimentoMock
                .Setup(x => x.SelecionarPorIdAsync(
                    estabelecimentoId,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync((Estabelecimento?)null);

            // Act
            Result<CardapioDto> resultado = await handler.Handle(query);

            // Assert
            Assert.IsTrue(resultado.IsFailed);
            Assert.IsNotEmpty(resultado.Errors);

            repositorioProdutoMock.Verify(
                x => x.SelecionarCardapioAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [TestMethod]
        public async Task DeveRetornar_ErroQuandoEstabelecimentoEstiverInativo()
        {
            // Arrange
            Guid estabelecimentoId = Guid.NewGuid();

            ObterCardapioQuery query = new(estabelecimentoId);

            Estabelecimento estabelecimento = new(
                estabelecimentoId,
                "Pizzaria",
                "01366999202",
                "Rua Finado Da Silva",
                "(49) 99999-9999",
                "Lages",
                new TimeOnly(19),
                new TimeOnly(23),
                0
            );

            estabelecimento.Desativar();

            repositorioEstabelecimentoMock
                .Setup(x => x.SelecionarPorIdAsync(
                    estabelecimentoId,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(estabelecimento);

            // Act
            Result<CardapioDto> resultado = await handler.Handle(query);

            // Assert
            Assert.IsTrue(resultado.IsFailed);
            Assert.IsNotEmpty(resultado.Errors);

            repositorioProdutoMock.Verify(
                x => x.SelecionarCardapioAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Never
            );
        }

        [TestMethod]
        public async Task DeveAgrupar_ProdutosPorCategoria()
        {
            // Arrange
            Guid estabelecimentoId = Guid.NewGuid();

            ObterCardapioQuery query = new(estabelecimentoId);

            Estabelecimento estabelecimento = new(
                estabelecimentoId,
                "Pizzaria",
                "01366999202",
                "Rua Finado Da Silva",
                "(49) 99999-9999",
                "Lages",
                new TimeOnly(19),
                new TimeOnly(23),
                0
            );

            Categoria categoriaLanches = new(
                Guid.NewGuid(),
                estabelecimentoId,
                "Lanches"
            );

            Categoria categoriaBebidas = new(
                Guid.NewGuid(),
                estabelecimentoId,
                "Bebidas"
            );

            Produto xBurger = new(
                Guid.NewGuid(),
                estabelecimentoId,
                categoriaLanches.Id,
                "X-Burger",
                "Hambúrguer artesanal",
                25.90m
            );

            Produto xSalada = new(
                Guid.NewGuid(),
                estabelecimentoId,
                categoriaLanches.Id,
                "X-Salada",
                "Hambúrguer com salada",
                27.90m
            );

            Produto cocaCola = new(
                Guid.NewGuid(),
                estabelecimentoId,
                categoriaBebidas.Id,
                "Coca-Cola",
                "Refrigerante",
                8.00m
            );

            typeof(Produto)
                .GetProperty(nameof(Produto.Categoria))!
                .SetValue(xBurger, categoriaLanches);

            typeof(Produto)
                .GetProperty(nameof(Produto.Categoria))!
                .SetValue(xSalada, categoriaLanches);

            typeof(Produto)
                .GetProperty(nameof(Produto.Categoria))!
                .SetValue(cocaCola, categoriaBebidas);

            repositorioEstabelecimentoMock
                .Setup(x => x.SelecionarPorIdAsync(
                    estabelecimentoId,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(estabelecimento);

            repositorioProdutoMock
                .Setup(x => x.SelecionarCardapioAsync(
                    estabelecimentoId,
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync([
                    xBurger,
                xSalada,
                cocaCola
                ]);

            // Act
            Result<CardapioDto> resultado = await handler.Handle(query);

            // Assert
            Assert.IsTrue(resultado.IsSuccess);

            CardapioDto cardapioDto = resultado.Value;

            Assert.HasCount(2, cardapioDto.Categorias);

            CategoriaDoCardapioDto categoriaBebidasDto =
                cardapioDto.Categorias
                    .Single(categoria => categoria.Id == categoriaBebidas.Id);

            CategoriaDoCardapioDto categoriaLanchesDto =
                cardapioDto.Categorias
                    .Single(categoria => categoria.Id == categoriaLanches.Id);

            Assert.AreEqual(
                "Bebidas",
                categoriaBebidasDto.Nome
            );

            Assert.AreEqual(
                "Lanches",
                categoriaLanchesDto.Nome
            );

            Assert.HasCount(
                1,
                categoriaBebidasDto.Produtos
            );

            Assert.HasCount(
                2,
                categoriaLanchesDto.Produtos
            );
        }
    }
}