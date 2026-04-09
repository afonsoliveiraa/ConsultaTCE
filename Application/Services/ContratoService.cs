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

    public async Task ImportarContratosAsync(Stream arquivoStream)
    {
        // 1. Orquestra a leitura (Transforma o binário em DTOs)
        var dtos = await _leitor.LerArquivoAsync(arquivoStream);

        // 2. Converte DTOs para Entidades de Domínio
        var entidades = dtos.Select(dto => new Contrato 
        {
            TipoDocumento = dto.TipoDocumento ?? "511", // Usa o do DTO ou o padrão 511
            CodMunicipio = dto.CodMunicipio,
            CpfGestor = dto.CpfGestor,
            NumeroContrato = dto.NumeroContrato,
            DataAssinatura = dto.DataAssinatura,
            Modalidade = dto.Modalidade,
            VigenciaInicial = dto.VigenciaInicial,
            VigenciaFinal = dto.VigenciaFinal,
            Referencia = dto.Referencia,
            Valor = dto.Valor,
            Objeto = dto.Objeto,
            CpfFiscal = dto.CpfFiscal,
            NomeFiscal = dto.NomeFiscal
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
            TipoDocumento:  n.TipoDocumento,
            CodMunicipio:   n.CodMunicipio,
            CpfGestor:      n.CpfGestor,
            NumeroContrato: n.NumeroContrato,
            DataAssinatura: n.DataAssinatura,
            Modalidade:     n.Modalidade,
            VigenciaInicial: n.VigenciaInicial,
            VigenciaFinal:   n.VigenciaFinal,
            Referencia: n.Referencia,
            Objeto:         n.Objeto,
            Valor:          n.Valor,
            CpfFiscal:      n.CpfFiscal,
            NomeFiscal:     n.NomeFiscal
        )).ToList();

        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new ContratoPagedResultDTO(items, page, pageSize, totalItems, totalPages);
    }
}
