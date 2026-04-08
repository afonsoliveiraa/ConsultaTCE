namespace Domain.Dtos;

public record ContratoDTO(
    string NumeroContrato,
    string CodMunicipio,
    string CpfGestor,
    string Objeto,
    string CpfFiscal,
    string NomeFiscal
);