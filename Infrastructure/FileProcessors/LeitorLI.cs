using System.Globalization;
using System.Text;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace Infrastructure.FileProcessors;

public class LeitorLI : ILeitorLI
{
    public async Task<IEnumerable<LicitacaoDTO>> LerArquivoAsync(Stream stream)
    {
        var lista = new List<LicitacaoDTO>();
        using var parser = new TextFieldParser(stream, Encoding.GetEncoding("iso-8859-1"));

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        int? anoLayoutDetectado = null;

        while (!parser.EndOfData)
        {
            var colunas = parser.ReadFields();

            if (colunas == null || colunas.Length < 2) continue;

            var anoReferencia = ExtrairAnoReferencia(colunas);

            // Validação de consistência do arquivo (Evita misturar 2023 com 2026 no mesmo arquivo)
            anoLayoutDetectado ??= anoReferencia;
            if (AnoParaLayout(anoLayoutDetectado.Value) != AnoParaLayout(anoReferencia))
            {
                throw new Exception(
                    $"O arquivo contém referências de layouts incompatíveis ({anoLayoutDetectado} e {anoReferencia}).");
            }

            var dto = MapearPorAno(colunas, anoReferencia);
            if (!string.IsNullOrWhiteSpace(dto.NumeroProcesso))
            {
                lista.Add(dto);
            }
        }

        return await Task.FromResult(lista);
    }

    private int ExtrairAnoReferencia(string[] colunas)
    {
        // No LI (501), a referência está sempre no Campo 25 (índice 24)
        if (colunas.Length < 25)
        {
            throw new Exception("Layout do arquivo de licitações inválido (campos insuficientes).");
        }

        var referenciaBruta = colunas[24]?.Trim(); 
        if (string.IsNullOrWhiteSpace(referenciaBruta) || referenciaBruta.Length < 4)
        {
            throw new Exception("Não foi possível identificar o ano de referência no Campo 25.");
        }

        if (!int.TryParse(referenciaBruta[..4], out var anoReferencia))
        {
            throw new Exception("Ano de referência inválido no arquivo.");
        }

        return anoReferencia;
    }

    private string AnoParaLayout(int anoReferencia)
    {
        // 2017-2023: 25 campos
        // 2024-2026: 35 campos
        return anoReferencia <= 2023 ? "LEGADO" : "MODERNO_PNCP";
    }

    private LicitacaoDTO MapearPorAno(string[] colunas, int anoReferencia)
    {
        // Encaminha para o mapeamento correto baseado no ano de referência
        return anoReferencia <= 2023 
            ? MapLayoutLegado(colunas) 
            : MapLayout2024_2026(colunas);
    }

    private LicitacaoDTO MapLayoutLegado(string[] col)
    {
        return new LicitacaoDTO(
            TipoDocumento:           col[0].Trim(),
            CodMunicipio:            col[1].Trim(),
            DataAutuacao:            ParseDataTce(col[2]),
            NumeroProcesso:          col[3].Trim(),
            EspecieProcesso:         col[4].Trim(),
            Objeto:                  col[5].Trim(),
            ValorEstimado:           ParseDecimalTce(col[6]),
            CpfParecerJuridico:      col[7].Trim(),
            NomeParecerJuridico:     col[8].Trim(),
            CpfGestorUnidade:        col[9].Trim(),
            DataPortariaComissao:    ParseDataTce(col[10]),
            SequencialComissao:      col[11].Trim(),
            CpfHomologacao:          col[12].Trim(),
            NomeHomologacao:         col[13].Trim(),
            DataHomologacao:         ParseDataTce(col[14]),
            HoraRealizacao:          col[15].Trim(),
            DataRealizacao:          ParseDataTce(col[16]),
            Modalidade:              col[17].Trim(),
            CriterioJulgamento:      col[18].Trim(),
            ValorLimiteSuperior:     ParseDecimalTce(col[19]),
            JustificativaPreco:      col[20].Trim(),
            MotivoEscolhaFornecedor: col[21].Trim(),
            FundamentacaoLegal:      col[22].Trim(),
            OrgaoGerenciadorAta:     col[23].Trim(),
            DataReferencia:          ParseReferenciaTce(col[24]),
            // Campos inexistentes no layout de 25 campos preenchidos com vazio
            CpfCotacaoPrecos: "", NomeCotacaoPrecos: "", CpfElaboradorTermo: "",
            NomeElaboradorTermo: "", FormaContratacao: "", TipoDisputa: "",
            UrlPlataforma: "", SistemaRegistroPreco: "", IdContratacaoPncp: "", IdAtaPncp: ""
        );
    }

    private LicitacaoDTO MapLayout2024_2026(string[] col)
    {
        // Suporta 2024, 2025 e 2026 conforme manuais enviados
        return new LicitacaoDTO(
            TipoDocumento:           col[0].Trim(),
            CodMunicipio:            col[1].Trim(),
            DataAutuacao:            ParseDataTce(col[2]),
            NumeroProcesso:          col[3].Trim(),
            EspecieProcesso:         col[4].Trim(),
            Objeto:                  col[5].Trim(),
            ValorEstimado:           ParseDecimalTce(col[6]),
            CpfParecerJuridico:      col[7].Trim(),
            NomeParecerJuridico:     col[8].Trim(),
            CpfGestorUnidade:        col[9].Trim(),
            DataPortariaComissao:    ParseDataTce(col[10]),
            SequencialComissao:      col[11].Trim(),
            CpfHomologacao:          col[12].Trim(),
            NomeHomologacao:         col[13].Trim(),
            DataHomologacao:         ParseDataTce(col[14]),
            HoraRealizacao:          col[15].Trim(),
            DataRealizacao:          ParseDataTce(col[16]),
            Modalidade:              col[17].Trim(),
            CriterioJulgamento:      col[18].Trim(),
            ValorLimiteSuperior:     ParseDecimalTce(col[19]),
            JustificativaPreco:      col[20].Trim(),
            MotivoEscolhaFornecedor: col[21].Trim(),
            FundamentacaoLegal:      col[22].Trim(),
            OrgaoGerenciadorAta:     col[23].Trim(),
            DataReferencia:          ParseReferenciaTce(col[24]),
            CpfCotacaoPrecos:        col[25].Trim(),
            NomeCotacaoPrecos:       col[26].Trim(),
            CpfElaboradorTermo:      col[27].Trim(),
            NomeElaboradorTermo:     col[28].Trim(),
            FormaContratacao:        col[29].Trim(),
            TipoDisputa:             col[30].Trim(),
            UrlPlataforma:           col[31].Trim(),
            SistemaRegistroPreco:    col[32].Trim(),
            IdContratacaoPncp:       col[33].Trim(),
            IdAtaPncp:               col[34].Trim()
        );
    }

    private DateTime? ParseDataTce(string dataStr)
    {
        if (string.IsNullOrWhiteSpace(dataStr) || dataStr == "0") return null;
        if (DateTime.TryParseExact(dataStr.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
            return DateTime.SpecifyKind(data, DateTimeKind.Utc);
        return null;
    }

    private DateTime? ParseReferenciaTce(string dataStr)
    {
        if (string.IsNullOrWhiteSpace(dataStr) || dataStr == "0") return null;
        if (DateTime.TryParseExact(dataStr.Trim(), "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
            return new DateTime(data.Year, data.Month, 1, 12, 0, 0, DateTimeKind.Utc);
        return null;
    }

    private decimal ParseDecimalTce(string valorStr)
    {
        if (string.IsNullOrWhiteSpace(valorStr)) return 0;
        decimal.TryParse(valorStr.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var valor);
        return valor;
    }
}