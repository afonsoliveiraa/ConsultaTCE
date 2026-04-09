using System.Text;
using System.Globalization;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace Infrastructure.FileProcessors;

public class LeitorCO : ILeitorCO
{
    public async Task<IEnumerable<ContratoDTO>> LerArquivoAsync(Stream stream, string exerc)
    {
        try
        {
            var lista = new List<ContratoDTO>();
            using var parser = new TextFieldParser(stream, Encoding.GetEncoding("iso-8859-1"));
            
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(","); 
            parser.HasFieldsEnclosedInQuotes = true;

            if (!int.TryParse(exerc, out int anoExercicio))
                throw new ArgumentException("O exercício fornecido é inválido.");

            while (!parser.EndOfData)
            {
                var colunas = parser.ReadFields();
                
                // Ignora linhas vazias ou incompletas que não sejam erro de layout
                if (colunas == null || colunas.Length < 2) continue;

                // --- VALIDAÇÃO DE SEGURANÇA (EXERCÍCIO) ---
                // Verifica a posição da Referência: 18 colunas (index 17) ou 19+ colunas (index 18)
                string refBruta = (colunas.Length == 18) ? colunas[17] : colunas[18];

                if (string.IsNullOrWhiteSpace(refBruta) || refBruta.Length < 4)
                {
                    throw new Exception("Não foi possível identificar o ano de referência no arquivo enviado.");
                }

                // Validação solicitada: Comparar o ano da referência com o exercício selecionado
                string anoArquivo = refBruta.Substring(0, 4);
                if (anoArquivo != exerc)
                {
                    // Lança a exceção que o front-end deve exibir
                    throw new Exception($"Bloqueio de segurança: O arquivo contém dados de {anoArquivo}, mas você selecionou o exercício {exerc}.");
                }

                ContratoDTO dto;

                // Seleção de Layout baseada no ano selecionado (já validado com o arquivo)
                if (anoExercicio <= 2018)
                    dto = MapLayout2017_2018(colunas);
                else if (anoExercicio >= 2019 && anoExercicio <= 2023)
                    dto = MapLayout2019_2023(colunas);
                else
                    dto = MapLayout2024_2025(colunas);

                if (!string.IsNullOrWhiteSpace(dto.NumeroContrato))
                {
                    lista.Add(dto);
                }
            }

            return await Task.FromResult(lista);
        }
        catch (Exception)
        {
            // Faz o throw para que o Controller capture a mensagem exata
            throw; 
        }
    }

    private ContratoDTO MapLayout2017_2018(string[] colunas)
    {
        return new ContratoDTO(
            TipoDocumento:      colunas[0].Trim(),
            CodMunicipio:       colunas[1].Trim(),
            CpfGestor:          colunas[2].Trim(),
            NumeroContrato:     colunas[3].Trim(),
            DataAssinatura:     ParseDataTce(colunas[4]),
            TipoObjeto:         colunas[5].Trim(),
            Modalidade:         colunas[6].Trim(),
            CpfGestorOriginal:  "",
            NumeroContratoOrig: colunas[7].Trim(),
            DataContratoOrig:   ParseDataTce(colunas[8]),
            VigenciaInicial:    ParseDataTce(colunas[9]),
            VigenciaFinal:      ParseDataTce(colunas[10]),
            Objeto:             colunas[11].Trim(),
            Valor:              ParseDecimalTce(colunas[12]),
            DataInicioObra:     ParseDataTce(colunas[13]),
            TipoObraServico:    colunas[14].Trim(),
            NumeroObra:         colunas[15].Trim(),
            DataTerminoObra:    ParseDataTce(colunas[16]),
            Referencia:         ParseReferenciaTce(colunas[17]),
            DataAutuacao:       null,
            NumeroProcesso:     "",
            CpfFiscal:          "",
            NomeFiscal:         "",
            IdContratoPncp:     ""
        );
    }

    private ContratoDTO MapLayout2019_2023(string[] colunas)
    {
        return new ContratoDTO(
            TipoDocumento:      colunas[0].Trim(),
            CodMunicipio:       colunas[1].Trim(),
            CpfGestor:          colunas[2].Trim(),
            NumeroContrato:     colunas[3].Trim(),
            DataAssinatura:     ParseDataTce(colunas[4]),
            TipoObjeto:         colunas[5].Trim(),
            Modalidade:         colunas[6].Trim(),
            CpfGestorOriginal:  colunas[7].Trim(),
            NumeroContratoOrig: colunas[8].Trim(),
            DataContratoOrig:   ParseDataTce(colunas[9]),
            VigenciaInicial:    ParseDataTce(colunas[10]),
            VigenciaFinal:      ParseDataTce(colunas[11]),
            Objeto:             colunas[12].Trim(),
            Valor:              ParseDecimalTce(colunas[13]),
            DataInicioObra:     ParseDataTce(colunas[14]),
            TipoObraServico:    colunas[15].Trim(),
            NumeroObra:         colunas[16].Trim(),
            DataTerminoObra:    ParseDataTce(colunas[17]),
            Referencia:         ParseReferenciaTce(colunas[18]),
            DataAutuacao:       null,
            NumeroProcesso:     "",
            CpfFiscal:          "",
            NomeFiscal:         "",
            IdContratoPncp:     ""
        );
    }

    private ContratoDTO MapLayout2024_2025(string[] colunas)
    {
        return new ContratoDTO(
            TipoDocumento:      colunas[0].Trim(),
            CodMunicipio:       colunas[1].Trim(),
            CpfGestor:          colunas[2].Trim(),
            NumeroContrato:     colunas[3].Trim(),
            DataAssinatura:     ParseDataTce(colunas[4]),
            TipoObjeto:         colunas[5].Trim(),
            Modalidade:         colunas[6].Trim(),
            CpfGestorOriginal:  colunas[7].Trim(),
            NumeroContratoOrig: colunas[8].Trim(),
            DataContratoOrig:   ParseDataTce(colunas[9]),
            VigenciaInicial:    ParseDataTce(colunas[10]),
            VigenciaFinal:      ParseDataTce(colunas[11]),
            Objeto:             colunas[12].Trim(),
            Valor:              ParseDecimalTce(colunas[13]),
            DataInicioObra:     ParseDataTce(colunas[14]),
            TipoObraServico:    colunas[15].Trim(),
            NumeroObra:         colunas[16].Trim(),
            DataTerminoObra:    ParseDataTce(colunas[17]),
            Referencia:         ParseReferenciaTce(colunas[18]),
            DataAutuacao:       ParseDataTce(colunas[19]),
            NumeroProcesso:     colunas[20].Trim(),
            CpfFiscal:          colunas[21].Trim(),
            NomeFiscal:         colunas[22].Trim(),
            IdContratoPncp:     colunas[23].Trim()
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