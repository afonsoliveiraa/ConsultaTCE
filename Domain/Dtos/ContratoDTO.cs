namespace Domain.Dtos;

public record ContratoDTO(
    string TipoDocumento,        // 1
    string CodMunicipio,         // 2
    string CpfGestor,            // 3
    string NumeroContrato,       // 4
    DateTime? DataAssinatura,    // 5
    string TipoObjeto,           // 6
    string Modalidade,           // 7
    string CpfGestorOriginal,    // 8
    string NumeroContratoOrig,   // 9
    DateTime? DataContratoOrig,  // 10
    DateTime? VigenciaInicial,   // 11
    DateTime? VigenciaFinal,     // 12
    string Objeto,               // 13
    decimal Valor,               // 14
    DateTime? DataInicioObra,    // 15
    string TipoObraServico,      // 16
    string NumeroObra,           // 17
    DateTime? DataTerminoObra,   // 18
    DateTime? Referencia,        // 19 (AnoMes)
    DateTime? DataAutuacao,      // 20
    string NumeroProcesso,       // 21
    string CpfFiscal,            // 22
    string NomeFiscal,           // 23
    string IdContratoPncp        // 24
);