using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("contratos")]
public class Contrato
{
    public int Id { get; set; }
    
    [Column("tipo_documento")]
    public string TipoDocumento { get; set; } // Campo 1

    [Column("cod_municipio")]
    public string CodMunicipio { get; set; } // Campo 2

    [Column("cpf_gestor")]
    public string CpfGestor { get; set; } // Campo 3 (Gestor que celebrou)

    [Column("numero_contrato")]
    public string NumeroContrato { get; set; } // Campo 4

    [Column("objeto_contrato")]
    public string Objeto { get; set; } // Campo 13

    [Column("cpf_fiscal")]
    public string CpfFiscal { get; set; } // Campo 22

    [Column("nome_fiscal")]
    public string NomeFiscal { get; set; } // Campo 23
}