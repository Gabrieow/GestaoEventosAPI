using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoEventosAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioToOrganizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Organizadores",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Organizadores_UsuarioId",
                table: "Organizadores",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizadores_Usuarios_UsuarioId",
                table: "Organizadores",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizadores_Usuarios_UsuarioId",
                table: "Organizadores");

            migrationBuilder.DropIndex(
                name: "IX_Organizadores_UsuarioId",
                table: "Organizadores");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Organizadores");
        }
    }
}
