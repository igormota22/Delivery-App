using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_TBProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complementos_Produtos_ProdutoId",
                table: "Complementos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_TBCategorias_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos");

            migrationBuilder.RenameTable(
                name: "Produtos",
                newName: "TBProdutos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "TBProdutos",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "TBProdutos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "TBProdutos",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TBProdutos",
                table: "TBProdutos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TBProdutos_CategoriaId_EstabelecimentoId",
                table: "TBProdutos",
                columns: new[] { "CategoriaId", "EstabelecimentoId" });

            migrationBuilder.CreateIndex(
                name: "IX_TBProdutos_EstabelecimentoId",
                table: "TBProdutos",
                column: "EstabelecimentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complementos_TBProdutos_ProdutoId",
                table: "Complementos",
                column: "ProdutoId",
                principalTable: "TBProdutos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TBProdutos_TBCategorias_CategoriaId_EstabelecimentoId",
                table: "TBProdutos",
                columns: new[] { "CategoriaId", "EstabelecimentoId" },
                principalTable: "TBCategorias",
                principalColumns: new[] { "Id", "EstabelecimentoId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TBProdutos_TBEstabelecimentos_EstabelecimentoId",
                table: "TBProdutos",
                column: "EstabelecimentoId",
                principalTable: "TBEstabelecimentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complementos_TBProdutos_ProdutoId",
                table: "Complementos");

            migrationBuilder.DropForeignKey(
                name: "FK_TBProdutos_TBCategorias_CategoriaId_EstabelecimentoId",
                table: "TBProdutos");

            migrationBuilder.DropForeignKey(
                name: "FK_TBProdutos_TBEstabelecimentos_EstabelecimentoId",
                table: "TBProdutos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TBProdutos",
                table: "TBProdutos");

            migrationBuilder.DropIndex(
                name: "IX_TBProdutos_CategoriaId_EstabelecimentoId",
                table: "TBProdutos");

            migrationBuilder.DropIndex(
                name: "IX_TBProdutos_EstabelecimentoId",
                table: "TBProdutos");

            migrationBuilder.RenameTable(
                name: "TBProdutos",
                newName: "Produtos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Produtos",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Produtos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Produtos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complementos_Produtos_ProdutoId",
                table: "Complementos",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_TBCategorias_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "TBCategorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
