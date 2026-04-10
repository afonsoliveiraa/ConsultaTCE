using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InclusaoLicitacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "licitacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_documento = table.Column<string>(type: "text", nullable: false),
                    cod_municipio = table.Column<string>(type: "text", nullable: false),
                    data_autuacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    numero_processo = table.Column<string>(type: "text", nullable: false),
                    especie_processo = table.Column<string>(type: "text", nullable: false),
                    objeto = table.Column<string>(type: "text", nullable: false),
                    valor_estimado = table.Column<decimal>(type: "numeric", nullable: false),
                    cpf_parecer_juridico = table.Column<string>(type: "text", nullable: false),
                    nome_parecer_juridico = table.Column<string>(type: "text", nullable: false),
                    cpf_gestor_unidade = table.Column<string>(type: "text", nullable: false),
                    data_portaria_comissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sequencial_comissao = table.Column<string>(type: "text", nullable: false),
                    cpf_homologacao = table.Column<string>(type: "text", nullable: false),
                    nome_homologacao = table.Column<string>(type: "text", nullable: false),
                    data_homologacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hora_realizacao = table.Column<string>(type: "text", nullable: false),
                    data_realizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modalidade = table.Column<string>(type: "text", nullable: false),
                    criterio_julgamento = table.Column<string>(type: "text", nullable: false),
                    valor_limite_superior = table.Column<decimal>(type: "numeric", nullable: false),
                    justificativa_preco = table.Column<string>(type: "text", nullable: false),
                    motivo_escolha_fornecedor = table.Column<string>(type: "text", nullable: false),
                    fundamentacao_legal = table.Column<string>(type: "text", nullable: false),
                    orgao_gerenciador_ata = table.Column<string>(type: "text", nullable: false),
                    data_referencia = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cpf_cotacao_precos = table.Column<string>(type: "text", nullable: false),
                    nome_cotacao_precos = table.Column<string>(type: "text", nullable: false),
                    cpf_elaborador_termo = table.Column<string>(type: "text", nullable: false),
                    nome_elaborador_termo = table.Column<string>(type: "text", nullable: false),
                    forma_contratacao = table.Column<string>(type: "text", nullable: false),
                    tipo_disputa = table.Column<string>(type: "text", nullable: false),
                    url_plataforma = table.Column<string>(type: "text", nullable: false),
                    sistema_registro_preco = table.Column<string>(type: "text", nullable: false),
                    id_contratacao_pncp = table.Column<string>(type: "text", nullable: false),
                    id_ata_pncp = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_licitacoes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "licitacoes");
        }
    }
}
