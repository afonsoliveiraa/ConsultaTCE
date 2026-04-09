using System.Globalization;
using System.Text;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace Infrastructure.FileProcessors;

public class LeitorCO : ILeitorCO
{
    public async Task<IEnumerable<ContratoDTO>> LerArquivoAsync(Stream stream)
    {
        var lista = new List<ContratoDTO>();
        using var parser = new TextFieldParser(stream, Encoding.GetEncoding("iso-8859-1"));

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        int? anoLayoutDetectado = null;

        while (!parser.EndOfData)
        {
            var colunas = parser.ReadFields();

            if (colunas == null || colunas.Length < 2)
            {
                continue;
            }

            // A Referencia continua sendo a fonte oficial do periodo do registro.
            // Se um dia quiser voltar ao modelo anterior, aqui e o ponto para recolocar a validacao por exercicio selecionado.
            var anoReferencia = ExtrairAnoReferencia(colunas);

            // Mantemos um unico layout por arquivo para evitar mistura silenciosa de estruturas diferentes.
            anoLayoutDetectado ??= anoReferencia;
            if (AnoParaLayout(anoLayoutDetectado.Value) != AnoParaLayout(anoReferencia))
            {
                throw new Exception(
                    $"O arquivo contem referencias de layouts diferentes ({anoLayoutDetectado} e {anoReferencia}). Separe os arquivos por periodo antes de importar.");
            }

            var dto = MapearPorAno(colunas, anoReferencia);
            if (!string.IsNullOrWhiteSpace(dto.NumeroContrato))
            {
                lista.Add(dto);
            }
        }

        return await Task.FromResult(lista);
    }

    private int ExtrairAnoReferencia(string[] colunas)
    {
        var indiceReferencia = ObterIndiceReferencia(colunas.Length);
        if (indiceReferencia >= colunas.Length)
        {
            throw new Exception("Nao foi possivel localizar a coluna de referencia no arquivo enviado.");
        }

        var referenciaBruta = colunas[indiceReferencia]?.Trim();
        if (string.IsNullOrWhiteSpace(referenciaBruta) || referenciaBruta.Length < 4)
        {
            throw new Exception("Nao foi possivel identificar o ano de referencia no arquivo enviado.");
        }

        if (!int.TryParse(referenciaBruta[..4], out var anoReferencia))
        {
            throw new Exception("A referencia do arquivo esta em um formato invalido.");
        }

        return anoReferencia;
    }

    private int ObterIndiceReferencia(int quantidadeColunas)
    {
        return quantidadeColunas switch
        {
            18 => 17,
            >= 19 => 18,
            _ => throw new Exception("O layout do arquivo de contratos nao foi reconhecido.")
        };
    }

    private string AnoParaLayout(int anoReferencia)
    {
        return anoReferencia switch
        {
            <= 2018 => "2017-2018",
            <= 2023 => "2019-2023",
            _ => "2024+"
        };
    }

    private ContratoDTO MapearPorAno(string[] colunas, int anoReferencia)
    {
        return anoReferencia switch
        {
            <= 2018 => MapLayout2017_2018(colunas),
            <= 2023 => MapLayout2019_2023(colunas),
            _ => MapLayout2024_2025(colunas)
        };
    }

    private ContratoDTO MapLayout2017_2018(string[] colunas)
    {
        return new ContratoDTO(
            TipoDocumento: colunas[0].Trim(),
            CodMunicipio: colunas[1].Trim(),
            CpfGestor: colunas[2].Trim(),
            NumeroContrato: colunas[3].Trim(),
            DataAssinatura: ParseDataTce(colunas[4]),
            TipoObjeto: colunas[5].Trim(),
            Modalidade: colunas[6].Trim(),
            CpfGestorOriginal: "",
            NumeroContratoOrig: colunas[7].Trim(),
            DataContratoOrig: ParseDataTce(colunas[8]),
            VigenciaInicial: ParseDataTce(colunas[9]),
            VigenciaFinal: ParseDataTce(colunas[10]),
            Objeto: colunas[11].Trim(),
            Valor: ParseDecimalTce(colunas[12]),
            DataInicioObra: ParseDataTce(colunas[13]),
            TipoObraServico: colunas[14].Trim(),
            NumeroObra: colunas[15].Trim(),
            DataTerminoObra: ParseDataTce(colunas[16]),
            Referencia: ParseReferenciaTce(colunas[17]),
            DataAutuacao: null,
            NumeroProcesso: "",
            CpfFiscal: "",
            NomeFiscal: "",
            IdContratoPncp: ""
        );
    }

    private ContratoDTO MapLayout2019_2023(string[] colunas)
    {
        return new ContratoDTO(
            TipoDocumento: colunas[0].Trim(),
            CodMunicipio: colunas[1].Trim(),
            CpfGestor: colunas[2].Trim(),
            NumeroContrato: colunas[3].Trim(),
            DataAssinatura: ParseDataTce(colunas[4]),
            TipoObjeto: colunas[5].Trim(),
            Modalidade: colunas[6].Trim(),
            CpfGestorOriginal: colunas[7].Trim(),
            NumeroContratoOrig: colunas[8].Trim(),
            DataContratoOrig: ParseDataTce(colunas[9]),
            VigenciaInicial: ParseDataTce(colunas[10]),
            VigenciaFinal: ParseDataTce(colunas[11]),
            Objeto: colunas[12].Trim(),
            Valor: ParseDecimalTce(colunas[13]),
            DataInicioObra: ParseDataTce(colunas[14]),
            TipoObraServico: colunas[15].Trim(),
            NumeroObra: colunas[16].Trim(),
            DataTerminoObra: ParseDataTce(colunas[17]),
            Referencia: ParseReferenciaTce(colunas[18]),
            DataAutuacao: null,
            NumeroProcesso: "",
            CpfFiscal: "",
            NomeFiscal: "",
            IdContratoPncp: ""
        );
    }

    private ContratoDTO MapLayout2024_2025(string[] colunas)
    {
        return new ContratoDTO(
            TipoDocumento: colunas[0].Trim(),
            CodMunicipio: colunas[1].Trim(),
            CpfGestor: colunas[2].Trim(),
            NumeroContrato: colunas[3].Trim(),
            DataAssinatura: ParseDataTce(colunas[4]),
            TipoObjeto: colunas[5].Trim(),
            Modalidade: colunas[6].Trim(),
            CpfGestorOriginal: colunas[7].Trim(),
            NumeroContratoOrig: colunas[8].Trim(),
            DataContratoOrig: ParseDataTce(colunas[9]),
            VigenciaInicial: ParseDataTce(colunas[10]),
            VigenciaFinal: ParseDataTce(colunas[11]),
            Objeto: colunas[12].Trim(),
            Valor: ParseDecimalTce(colunas[13]),
            DataInicioObra: ParseDataTce(colunas[14]),
            TipoObraServico: colunas[15].Trim(),
            NumeroObra: colunas[16].Trim(),
            DataTerminoObra: ParseDataTce(colunas[17]),
            Referencia: ParseReferenciaTce(colunas[18]),
            DataAutuacao: ParseDataTce(colunas[19]),
            NumeroProcesso: colunas[20].Trim(),
            CpfFiscal: colunas[21].Trim(),
            NomeFiscal: colunas[22].Trim(),
            IdContratoPncp: colunas[23].Trim()
        );
    }

    private DateTime? ParseDataTce(string dataStr)
    {
        if (string.IsNullOrWhiteSpace(dataStr) || dataStr == "0")
        {
            return null;
        }

        if (DateTime.TryParseExact(dataStr.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
        {
            return DateTime.SpecifyKind(data, DateTimeKind.Utc);
        }

        return null;
    }

    private DateTime? ParseReferenciaTce(string dataStr)
    {
        if (string.IsNullOrWhiteSpace(dataStr) || dataStr == "0")
        {
            return null;
        }

        if (DateTime.TryParseExact(dataStr.Trim(), "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
        {
            return new DateTime(data.Year, data.Month, 1, 12, 0, 0, DateTimeKind.Utc);
        }

        return null;
    }

    private decimal ParseDecimalTce(string valorStr)
    {
        if (string.IsNullOrWhiteSpace(valorStr))
        {
            return 0;
        }

        decimal.TryParse(valorStr.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var valor);
        return valor;
    }
}
