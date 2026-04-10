using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class LicitacaoService
{
    private readonly ILeitorLI _leitor;
    private readonly ILicitacaoRepository _repository;

    public LicitacaoService(ILeitorLI leitor, ILicitacaoRepository repository)
    {
        _leitor = leitor;
        _repository = repository;
    }

    public async Task<int> ImportarLicitacoesAsync(Stream arquivoStream)
    {
        // O leitor processa o arquivo (identificando se é legado ou moderno automaticamente)
        var dtos = await _leitor.LerArquivoAsync(arquivoStream);

        var entidades = dtos.Select(dto => new Licitacao
        {
            // Mapeamento dos campos baseado no layout 501
            TipoDocumento = dto.TipoDocumento ?? "501",
            CodMunicipio = dto.CodMunicipio,
            DataAutuacao = dto.DataAutuacao,
            NumeroProcesso = dto.NumeroProcesso,
            EspecieProcesso = dto.EspecieProcesso,
            Objeto = dto.Objeto,
            ValorEstimado = dto.ValorEstimado,
            CpfParecerJuridico = dto.CpfParecerJuridico,
            NomeParecerJuridico = dto.NomeParecerJuridico,
            CpfGestorUnidade = dto.CpfGestorUnidade,
            DataPortariaComissao = dto.DataPortariaComissao,
            SequencialComissao = dto.SequencialComissao,
            CpfHomologacao = dto.CpfHomologacao,
            NomeHomologacao = dto.NomeHomologacao,
            DataHomologacao = dto.DataHomologacao,
            HoraRealizacao = dto.HoraRealizacao,
            DataRealizacao = dto.DataRealizacao,
            Modalidade = dto.Modalidade,
            CriterioJulgamento = dto.CriterioJulgamento,
            ValorLimiteSuperior = dto.ValorLimiteSuperior,
            JustificativaPreco = dto.JustificativaPreco,
            MotivoEscolhaFornecedor = dto.MotivoEscolhaFornecedor,
            FundamentacaoLegal = dto.FundamentacaoLegal,
            OrgaoGerenciadorAta = dto.OrgaoGerenciadorAta,
            DataReferencia = dto.DataReferencia,
            
            // Campos específicos de 2024 a 2026
            CpfCotacaoPrecos = dto.CpfCotacaoPrecos,
            NomeCotacaoPrecos = dto.NomeCotacaoPrecos,
            CpfElaboradorTermo = dto.CpfElaboradorTermo,
            NomeElaboradorTermo = dto.NomeElaboradorTermo,
            FormaContratacao = dto.FormaContratacao,
            TipoDisputa = dto.TipoDisputa,
            UrlPlataforma = dto.UrlPlataforma,
            SistemaRegistroPreco = dto.SistemaRegistroPreco,
            IdContratacaoPncp = dto.IdContratacaoPncp,
            IdAtaPncp = dto.IdAtaPncp
        }).ToList();

        if (entidades.Any())
        {
            await _repository.AdicionarVariosAsync(entidades);
        }

        return entidades.Count;
    }

    public async Task<LicitacaoPagedResultDTO> BuscarPorNumeroProcessoAsync(
        string? numeroProcesso,
        int page,
        int pageSize)
    {
        var (entidades, totalItems) = await _repository.BuscarPorProcessoAsync(numeroProcesso, page, pageSize);

        var items = entidades.Select(n => new LicitacaoDTO(
            TipoDocumento: n.TipoDocumento,
            CodMunicipio: n.CodMunicipio,
            DataAutuacao: n.DataAutuacao,
            NumeroProcesso: n.NumeroProcesso,
            EspecieProcesso: n.EspecieProcesso,
            Objeto: n.Objeto,
            ValorEstimado: n.ValorEstimado,
            CpfParecerJuridico: n.CpfParecerJuridico,
            NomeParecerJuridico: n.NomeParecerJuridico,
            CpfGestorUnidade: n.CpfGestorUnidade,
            DataPortariaComissao: n.DataPortariaComissao,
            SequencialComissao: n.SequencialComissao,
            CpfHomologacao: n.CpfHomologacao,
            NomeHomologacao: n.NomeHomologacao,
            DataHomologacao: n.DataHomologacao,
            HoraRealizacao: n.HoraRealizacao,
            DataRealizacao: n.DataRealizacao,
            Modalidade: n.Modalidade,
            CriterioJulgamento: n.CriterioJulgamento,
            ValorLimiteSuperior: n.ValorLimiteSuperior,
            JustificativaPreco: n.JustificativaPreco,
            MotivoEscolhaFornecedor: n.MotivoEscolhaFornecedor,
            FundamentacaoLegal: n.FundamentacaoLegal,
            OrgaoGerenciadorAta: n.OrgaoGerenciadorAta,
            DataReferencia: n.DataReferencia,
            CpfCotacaoPrecos: n.CpfCotacaoPrecos,
            NomeCotacaoPrecos: n.NomeCotacaoPrecos,
            CpfElaboradorTermo: n.CpfElaboradorTermo,
            NomeElaboradorTermo: n.NomeElaboradorTermo,
            FormaContratacao: n.FormaContratacao,
            TipoDisputa: n.TipoDisputa,
            UrlPlataforma: n.UrlPlataforma,
            SistemaRegistroPreco: n.SistemaRegistroPreco,
            IdContratacaoPncp: n.IdContratacaoPncp,
            IdAtaPncp: n.IdAtaPncp
        )).ToList();

        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new LicitacaoPagedResultDTO(items, page, pageSize, totalItems, totalPages);
    }
}