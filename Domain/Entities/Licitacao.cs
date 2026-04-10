using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("licitacoes")]
public class Licitacao
{
    public int Id { get; set; }

    [Column("tipo_documento")]
    public string TipoDocumento { get; set; } // Campo 1

    [Column("cod_municipio")]
    public string CodMunicipio { get; set; } // Campo 2

    [Column("data_autuacao")]
    public DateTime? DataAutuacao { get; set; } // Campo 3

    [Column("numero_processo")]
    public string NumeroProcesso { get; set; } // Campo 4 (Chave importante)

    [Column("especie_processo")]
    public string EspecieProcesso { get; set; } // Campo 5

    [Column("objeto")]
    public string Objeto { get; set; } // Campo 6

    [Column("valor_estimado")]
    public decimal ValorEstimado { get; set; } // Campo 7

    [Column("cpf_parecer_juridico")]
    public string CpfParecerJuridico { get; set; } // Campo 8

    [Column("nome_parecer_juridico")]
    public string NomeParecerJuridico { get; set; } // Campo 9

    [Column("cpf_gestor_unidade")]
    public string CpfGestorUnidade { get; set; } // Campo 10

    [Column("data_portaria_comissao")]
    public DateTime? DataPortariaComissao { get; set; } // Campo 11

    [Column("sequencial_comissao")]
    public string SequencialComissao { get; set; } // Campo 12

    [Column("cpf_homologacao")]
    public string CpfHomologacao { get; set; } // Campo 13

    [Column("nome_homologacao")]
    public string NomeHomologacao { get; set; } // Campo 14

    [Column("data_homologacao")]
    public DateTime? DataHomologacao { get; set; } // Campo 15

    [Column("hora_realizacao")]
    public string HoraRealizacao { get; set; } // Campo 16

    [Column("data_realizacao")]
    public DateTime? DataRealizacao { get; set; } // Campo 17

    [Column("modalidade")]
    public string Modalidade { get; set; } // Campo 18

    [Column("criterio_julgamento")]
    public string CriterioJulgamento { get; set; } // Campo 19

    [Column("valor_limite_superior")]
    public decimal ValorLimiteSuperior { get; set; } // Campo 20

    [Column("justificativa_preco")]
    public string JustificativaPreco { get; set; } // Campo 21

    [Column("motivo_escolha_fornecedor")]
    public string MotivoEscolhaFornecedor { get; set; } // Campo 22

    [Column("fundamentacao_legal")]
    public string FundamentacaoLegal { get; set; } // Campo 23

    [Column("orgao_gerenciador_ata")]
    public string OrgaoGerenciadorAta { get; set; } // Campo 24

    [Column("data_referencia")]
    public DateTime? DataReferencia { get; set; } // Campo 25 (yyyyMM)

    [Column("cpf_cotacao_precos")]
    public string CpfCotacaoPrecos { get; set; } // Campo 26

    [Column("nome_cotacao_precos")]
    public string NomeCotacaoPrecos { get; set; } // Campo 27

    [Column("cpf_elaborador_termo")]
    public string CpfElaboradorTermo { get; set; } // Campo 28

    [Column("nome_elaborador_termo")]
    public string NomeElaboradorTermo { get; set; } // Campo 29

    [Column("forma_contratacao")]
    public string FormaContratacao { get; set; } // Campo 30

    [Column("tipo_disputa")]
    public string TipoDisputa { get; set; } // Campo 31

    [Column("url_plataforma")]
    public string UrlPlataforma { get; set; } // Campo 32

    [Column("sistema_registro_preco")]
    public string SistemaRegistroPreco { get; set; } // Campo 33

    [Column("id_contratacao_pncp")]
    public string IdContratacaoPncp { get; set; } // Campo 34

    [Column("id_ata_pncp")]
    public string IdAtaPncp { get; set; } // Campo 35
}