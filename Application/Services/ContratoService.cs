using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ContratoService
{
    private readonly ILeitorCO _leitor;
    private readonly IContratoRepository _repository;

    public ContratoService(ILeitorCO leitor, IContratoRepository repository)
    {
        _leitor = leitor;
        _repository = repository;
    }

    public async Task ImportarContratosAsync(Stream arquivoStream, string exerc)
    {
        // 1. Orquestra a leitura (Transforma o binário em DTOs)
        var dtos = await _leitor.LerArquivoAsync(arquivoStream, exerc);

        // 2. Converte DTOs para Entidades de Domínio
        var entidades = dtos.Select(dto => new Contrato 
        {
            TipoDocumento = dto.TipoDocumento ?? "511",
            CodMunicipio = dto.CodMunicipio,
            CpfGestor = dto.CpfGestor,
            NumeroContrato = dto.NumeroContrato,
            DataAssinatura = dto.DataAssinatura,
            TipoObjeto = dto.TipoObjeto,              // Novo
            Modalidade = dto.Modalidade,
            CpfGestorOriginal = dto.CpfGestorOriginal, // Novo
            NumeroContratoOrig = dto.NumeroContratoOrig,// Novo
            DataContratoOrig = dto.DataContratoOrig,   // Novo
            VigenciaInicial = dto.VigenciaInicial,
            VigenciaFinal = dto.VigenciaFinal,
            Objeto = dto.Objeto,
            Valor = dto.Valor,
            DataInicioObra = dto.DataInicioObra,       // Novo
            TipoObraServico = dto.TipoObraServico,     // Novo
            NumeroObra = dto.NumeroObra,               // Novo
            DataTerminoObra = dto.DataTerminoObra,     // Novo
            Referencia = dto.Referencia,
            DataAutuacao = dto.DataAutuacao,           // Novo
            NumeroProcesso = dto.NumeroProcesso,       // Novo
            CpfFiscal = dto.CpfFiscal,
            NomeFiscal = dto.NomeFiscal,
            IdContratoPncp = dto.IdContratoPncp        // Novo
        }).ToList();

        if (entidades.Any())
        {
            // 3. Orquestra a persistência no Banco de Dados
            await _repository.AdicionarVariosAsync(entidades);
        }
    }

    public async Task<ContratoPagedResultDTO> BuscarPorNumeroContratoAsync(
        string? numeroContrato,
        int page,
        int pageSize)
    {
        // 1. Busca as entidades no banco via repositório já com paginação.
        var (notasEntidades, totalItems) = await _repository.BuscarPorContratoAsync(numeroContrato, page, pageSize);

        // 2. Transforma as entidades da página atual no DTO da API.
        var items = notasEntidades.Select(n => new ContratoDTO(
            TipoDocumento:      n.TipoDocumento,
            CodMunicipio:       n.CodMunicipio,
            CpfGestor:          n.CpfGestor,
            NumeroContrato:     n.NumeroContrato,
            DataAssinatura:     n.DataAssinatura,
            TipoObjeto:         n.TipoObjeto,       // Novo
            Modalidade:         n.Modalidade,
            CpfGestorOriginal:  n.CpfGestorOriginal,// Novo
            NumeroContratoOrig: n.NumeroContratoOrig,// Novo
            DataContratoOrig:   n.DataContratoOrig, // Novo
            VigenciaInicial:    n.VigenciaInicial,
            VigenciaFinal:      n.VigenciaFinal,
            Objeto:             n.Objeto,
            Valor:              n.Valor,
            DataInicioObra:     n.DataInicioObra,   // Novo
            TipoObraServico:    n.TipoObraServico,  // Novo
            NumeroObra:         n.NumeroObra,       // Novo
            DataTerminoObra:    n.DataTerminoObra,  // Novo
            Referencia:         n.Referencia,
            DataAutuacao:       n.DataAutuacao,     // Novo
            NumeroProcesso:     n.NumeroProcesso,   // Novo
            CpfFiscal:          n.CpfFiscal,
            NomeFiscal:         n.NomeFiscal,
            IdContratoPncp:     n.IdContratoPncp    // Novo
        )).ToList();

        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new ContratoPagedResultDTO(items, page, pageSize, totalItems, totalPages);
    }
}
