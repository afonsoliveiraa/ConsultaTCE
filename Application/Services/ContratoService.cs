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

    public async Task<IEnumerable<ContratoDTO>> BuscarPorNumeroContratoAsync(string numeroContrato)
    {
        // 1. Busca as entidades no banco via repositório
        var notasEntidades = await _repository.BuscarPorContratoAsync(numeroContrato);

        // 2. Transforma as entidades de volta para o seu DTO existente
        return notasEntidades.Select(n => new ContratoDTO(
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
    }
}