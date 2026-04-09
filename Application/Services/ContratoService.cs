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

    public async Task<int> ImportarContratosAsync(Stream arquivoStream)
    {
        // O leitor agora detecta o ano pela Referencia do proprio arquivo.
        // Se precisar voltar ao modelo antigo, basta reintroduzir o parametro exerc aqui.
        var dtos = await _leitor.LerArquivoAsync(arquivoStream);

        var entidades = dtos.Select(dto => new Contrato
        {
            TipoDocumento = dto.TipoDocumento ?? "511",
            CodMunicipio = dto.CodMunicipio,
            CpfGestor = dto.CpfGestor,
            NumeroContrato = dto.NumeroContrato,
            DataAssinatura = dto.DataAssinatura,
            TipoObjeto = dto.TipoObjeto,
            Modalidade = dto.Modalidade,
            CpfGestorOriginal = dto.CpfGestorOriginal,
            NumeroContratoOrig = dto.NumeroContratoOrig,
            DataContratoOrig = dto.DataContratoOrig,
            VigenciaInicial = dto.VigenciaInicial,
            VigenciaFinal = dto.VigenciaFinal,
            Objeto = dto.Objeto,
            Valor = dto.Valor,
            DataInicioObra = dto.DataInicioObra,
            TipoObraServico = dto.TipoObraServico,
            NumeroObra = dto.NumeroObra,
            DataTerminoObra = dto.DataTerminoObra,
            Referencia = dto.Referencia,
            DataAutuacao = dto.DataAutuacao,
            NumeroProcesso = dto.NumeroProcesso,
            CpfFiscal = dto.CpfFiscal,
            NomeFiscal = dto.NomeFiscal,
            IdContratoPncp = dto.IdContratoPncp
        }).ToList();

        if (entidades.Any())
        {
            await _repository.AdicionarVariosAsync(entidades);
        }

        return entidades.Count;
    }

    public async Task<ContratoPagedResultDTO> BuscarPorNumeroContratoAsync(
        string? numeroContrato,
        int page,
        int pageSize)
    {
        var (notasEntidades, totalItems) = await _repository.BuscarPorContratoAsync(numeroContrato, page, pageSize);

        var items = notasEntidades.Select(n => new ContratoDTO(
            TipoDocumento: n.TipoDocumento,
            CodMunicipio: n.CodMunicipio,
            CpfGestor: n.CpfGestor,
            NumeroContrato: n.NumeroContrato,
            DataAssinatura: n.DataAssinatura,
            TipoObjeto: n.TipoObjeto,
            Modalidade: n.Modalidade,
            CpfGestorOriginal: n.CpfGestorOriginal,
            NumeroContratoOrig: n.NumeroContratoOrig,
            DataContratoOrig: n.DataContratoOrig,
            VigenciaInicial: n.VigenciaInicial,
            VigenciaFinal: n.VigenciaFinal,
            Objeto: n.Objeto,
            Valor: n.Valor,
            DataInicioObra: n.DataInicioObra,
            TipoObraServico: n.TipoObraServico,
            NumeroObra: n.NumeroObra,
            DataTerminoObra: n.DataTerminoObra,
            Referencia: n.Referencia,
            DataAutuacao: n.DataAutuacao,
            NumeroProcesso: n.NumeroProcesso,
            CpfFiscal: n.CpfFiscal,
            NomeFiscal: n.NomeFiscal,
            IdContratoPncp: n.IdContratoPncp
        )).ToList();

        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new ContratoPagedResultDTO(items, page, pageSize, totalItems, totalPages);
    }
}
