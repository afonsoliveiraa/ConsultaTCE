namespace Domain.Dtos;

public record ContratoDTO(
    string TipoDocumento,
    string CodMunicipio,
    string CpfGestor,
    string NumeroContrato,
    DateTime? DataAssinatura,
    string Modalidade,
    DateTime? VigenciaInicial,
    DateTime? VigenciaFinal,
    DateTime? Referencia,
    string Objeto,
    decimal Valor,
    string CpfFiscal,
    string NomeFiscal
);