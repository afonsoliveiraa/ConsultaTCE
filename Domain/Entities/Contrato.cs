using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("contratos")]
public class Contrato
{
    public int Id { get; set; }
    
    [Column("tipo_documento")]
    public string TipoDocumento { get; set; }

    [Column("cod_municipio")]
    public string CodMunicipio { get; set; }

    [Column("cpf_gestor")]
    public string CpfGestor { get; set; }

    [Column("numero_contrato")]
    public string NumeroContrato { get; set; }
    
    [Column("data_assinatura")]
    public DateTime? DataAssinatura { get; set; }

    [Column("tipo_objeto")] // Campo 6
    public string TipoObjeto { get; set; }

    [Column("modalidade")] // Campo 7
    public string Modalidade { get; set; }

    [Column("cpf_gestor_original")] // Campo 8
    public string CpfGestorOriginal { get; set; }

    [Column("numero_contrato_original")] // Campo 9
    public string NumeroContratoOrig { get; set; }

    [Column("data_contrato_original")] // Campo 10
    public DateTime? DataContratoOrig { get; set; }

    [Column("vigencia_inicial")] // Campo 11
    public DateTime? VigenciaInicial { get; set; }

    [Column("vigencia_final")] // Campo 12
    public DateTime? VigenciaFinal { get; set; }
    
    [Column("objeto_contrato")] // Campo 13
    public string Objeto { get; set; }

    [Column("valor")] // Campo 14
    public decimal Valor { get; set; }

    [Column("data_inicio_obra")] // Campo 15
    public DateTime? DataInicioObra { get; set; }

    [Column("tipo_obra_servico")] // Campo 16
    public string TipoObraServico { get; set; }

    [Column("numero_obra")] // Campo 17
    public string NumeroObra { get; set; }

    [Column("data_termino_obra")] // Campo 18
    public DateTime? DataTerminoObra { get; set; }

    [Column("referencia")] // Campo 19
    public DateTime? Referencia { get; set; }

    [Column("data_autuacao")] // Campo 20
    public DateTime? DataAutuacao { get; set; }

    [Column("numero_processo")] // Campo 21
    public string NumeroProcesso { get; set; }

    [Column("cpf_fiscal")] // Campo 22
    public string CpfFiscal { get; set; }

    [Column("nome_fiscal")] // Campo 23
    public string NomeFiscal { get; set; }

    [Column("id_contrato_pncp")] // Campo 24
    public string IdContratoPncp { get; set; }
}