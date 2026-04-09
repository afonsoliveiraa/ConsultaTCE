using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarTabelaContratosTodosCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cpf_gestor_original",
                table: "contratos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_autuacao",
                table: "contratos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_contrato_original",
                table: "contratos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inicio_obra",
                table: "contratos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_termino_obra",
                table: "contratos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "id_contrato_pncp",
                table: "contratos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "numero_contrato_original",
                table: "contratos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "numero_obra",
                table: "contratos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "numero_processo",
                table: "contratos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tipo_objeto",
                table: "contratos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tipo_obra_servico",
                table: "contratos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cpf_gestor_original",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "data_autuacao",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "data_contrato_original",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "data_inicio_obra",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "data_termino_obra",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "id_contrato_pncp",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "numero_contrato_original",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "numero_obra",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "numero_processo",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "tipo_objeto",
                table: "contratos");

            migrationBuilder.DropColumn(
                name: "tipo_obra_servico",
                table: "contratos");
        }
    }
}
