using CacaoTrigo.Data;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CacaoTrigo.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosYZonas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Sentencias idempotentes: funcionan aunque la base ya tenga parte de los cambios.
            foreach (var sql in EsquemaBaseDatos.Sentencias)
            {
                migrationBuilder.Sql(sql);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_AspNetUsers_UsuarioId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_AspNetUsers_UsuarioId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Mesas_MesaId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_UsuarioId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_MesaId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Zona",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Pedidos");

            // Las reservas sin mesa no caben en la columna obligatoria anterior.
            migrationBuilder.Sql("DELETE FROM Reservas WHERE MesaId IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "MesaId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_MesaId",
                table: "Reservas",
                column: "MesaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Mesas_MesaId",
                table: "Reservas",
                column: "MesaId",
                principalTable: "Mesas",
                principalColumn: "MesaId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
