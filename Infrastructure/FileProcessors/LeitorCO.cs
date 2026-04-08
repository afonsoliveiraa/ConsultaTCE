using System.Text;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace Infrastructure.FileProcessors;

public class LeitorCO : ILeitorCO
{
    public async Task<IEnumerable<ContratoDTO>> LerArquivoAsync(Stream stream)
    {
        try
        {
            var lista = new List<ContratoDTO>();
            // Usamos o encoding iso-8859-1 para suportar os acentos dos arquivos do TCE
            using var parser = new TextFieldParser(stream, Encoding.GetEncoding("iso-8859-1"));
            
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true; 

            while (!parser.EndOfData)
            {
                var colunas = parser.ReadFields();
                if (colunas == null) continue;

                // Validamos o campo 4 (Número do Contrato)
                if (!string.IsNullOrWhiteSpace(colunas[3]))
                {
                    lista.Add(new ContratoDTO(
                        TipoDocumento:  colunas[0].Trim(),
                        CodMunicipio:   colunas[1].Trim(),
                        CpfGestor:      colunas[2].Trim(),
                        NumeroContrato: colunas[3].Trim(),
                        DataAssinatura: ParseDataTce(colunas[4]),
                        Modalidade:     colunas[6].Trim(),
                        VigenciaInicial: ParseDataTce(colunas[10]),
                        VigenciaFinal:   ParseDataTce(colunas[11]),
                        Objeto:         colunas[12].Trim(),
                        Valor:          ParseDecimalTce(colunas[13]),
                        CpfFiscal:      colunas[21].Trim(),
                        Referencia: ParseReferenciaTce(colunas[18]),
                        NomeFiscal:     colunas[22].Trim()
                    ));
                }
            }
            return await Task.FromResult(lista);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro no LeitorCO: {e.Message}");
            throw;
        }
    }

    private DateTime? ParseDataTce(string dataStr)
    {
        if (string.IsNullOrWhiteSpace(dataStr) || dataStr == "0") return null;
        
        if (DateTime.TryParseExact(dataStr.Trim(), "yyyyMMdd", 
            System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out var data))
        {
            // SOLUÇÃO PARA O ERRO DO POSTGRES:
            // O Npgsql exige que DateTimes enviados para colunas 'with time zone' sejam explicitamente UTC.
            return DateTime.SpecifyKind(data, DateTimeKind.Utc);
        }
        return null;
    }

    private DateTime? ParseReferenciaTce(string dataStr)
    {
        if (string.IsNullOrWhiteSpace(dataStr) || dataStr == "0") return null;
    
        if (DateTime.TryParseExact(dataStr.Trim(), "yyyyMM", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out var data))
        {
            // Em vez de SpecifyKind UTC, vamos garantir que a hora seja Meio-Dia (12:00).
            // Assim, mesmo que o fuso mude 3 horas para frente ou para trás, o DIA continua o mesmo.
            var dataMeioDia = new DateTime(data.Year, data.Month, data.Day, 12, 0, 0, DateTimeKind.Utc);
            return dataMeioDia;
        }
        return null;
    }
    
    private decimal ParseDecimalTce(string valorStr)
    {
        if (string.IsNullOrWhiteSpace(valorStr)) return 0;
        
        // InvariantCulture garante que o ponto decimal (.) seja processado corretamente
        decimal.TryParse(valorStr.Trim(), 
            System.Globalization.NumberStyles.Any, 
            System.Globalization.CultureInfo.InvariantCulture, out var valor);
            
        return valor;
    }
}