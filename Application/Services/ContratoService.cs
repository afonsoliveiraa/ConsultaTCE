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
            // CAMPOS OBRIGATÓRIOS QUE ESTAVAM FALTANDO:
            TipoDocumento = "511", // Valor fixo do manual SIM
            CodMunicipio = dto.CodMunicipio,
            Objeto = dto.Objeto,
        
            // CAMPOS QUE VOCÊ JÁ TINHA:
            CpfGestor = dto.CpfGestor,
            NumeroContrato = dto.NumeroContrato,

            // CAMPOS DO FISCAL:
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
            NumeroContrato: n.NumeroContrato,
            CodMunicipio:   n.CodMunicipio,
            CpfGestor:      n.CpfGestor,
            Objeto:         n.Objeto,
            CpfFiscal:      n.CpfFiscal,
            NomeFiscal:     n.NomeFiscal
        )).ToList();
    }
}