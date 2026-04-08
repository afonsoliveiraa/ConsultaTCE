using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarTabelaContratosCompleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_assinatura",
                table: "contratos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modalidade",
                table: "contratos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "valor",
                table: "contratos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "vigencia_final",
                table: "contratos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "vigencia_inicial",
                table: "contratos",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_assinatura",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "modalidade",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "valor",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "vigencia_final",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "vigencia_inicial",
                table: "contratos");
        }
    }
}
