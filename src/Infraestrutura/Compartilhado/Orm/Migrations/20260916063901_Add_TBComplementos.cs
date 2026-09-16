using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_TBComplementos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complementos_TBProdutos_ProdutoId",
                table: "Complementos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complementos",
                table: "Complementos");

            migrationBuilder.RenameTable(
                name: "Complementos",
                newName: "TBComplementos");

            migrationBuilder.RenameIndex(
                name: "IX_Complementos_ProdutoId",
                table: "TBComplementos",
                newName: "IX_TBComplementos_ProdutoId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoAdicional",
                table: "TBComplementos",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "TBComplementos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TBComplementos",
                table: "TBComplementos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBComplementos_TBProdutos_ProdutoId",
                table: "TBComplementos",
                column: "ProdutoId",
                principalTable: "TBProdutos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBComplementos_TBProdutos_ProdutoId",
                table: "TBComplementos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TBComplementos",
                table: "TBComplementos");

            migrationBuilder.RenameTable(
                name: "TBComplementos",
                newName: "Complementos");

            migrationBuilder.RenameIndex(
                name: "IX_TBComplementos_ProdutoId",
                table: "Complementos",
                newName: "IX_Complementos_ProdutoId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecoAdicional",
                table: "Complementos",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Complementos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complementos",
                table: "Complementos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Complementos_TBProdutos_ProdutoId",
                table: "Complementos",
                column: "ProdutoId",
                principalTable: "TBProdutos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
