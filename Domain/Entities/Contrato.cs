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
    public DateTime? DataAssinatura { get; set; } // Tipo DateTime

    [Column("modalidade")]
    public string Modalidade { get; set; }
    
    [Column("vigencia_inicial")]
    public DateTime? VigenciaInicial { get; set; }

    [Column("vigencia_final")]
    public DateTime? VigenciaFinal { get; set; }
    
    [Column("referencia")]
    public DateTime? Referencia { get; set; }
    
    [Column("valor")]
    public decimal Valor { get; set; }

    [Column("objeto_contrato")]
    public string Objeto { get; set; }

    [Column("cpf_fiscal")]
    public string CpfFiscal { get; set; }

    [Column("nome_fiscal")]
    public string NomeFiscal { get; set; }
}