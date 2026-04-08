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

            using var parser = new TextFieldParser(stream, Encoding.GetEncoding("iso-8859-1"));
            
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true; 

            while (!parser.EndOfData)
            {
                var colunas = parser.ReadFields();

                // Se a linha for nula, apenas pula para a próxima
                if (colunas == null) continue;

                // Como o layout é fixo, acessamos as posições do manual diretamente:
                // Se o número do contrato (Campo 4 / Índice 3) existir, processamos a linha
                if (!string.IsNullOrWhiteSpace(colunas[3]))
                {
                    lista.Add(new ContratoDTO(
                        NumeroContrato: colunas[3].Trim(),  // Campo 4
                        CodMunicipio:   colunas[1].Trim(),  // Campo 2
                        CpfGestor:      colunas[2].Trim(),  // Campo 3
                        Objeto:         colunas[12].Trim(), // Campo 13
                        CpfFiscal:      colunas[21].Trim(), // Campo 22
                        NomeFiscal:     colunas[22].Trim()  // Campo 23
                    ));
                }
            }

            return await Task.FromResult(lista);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro no processamento S&S: {e.Message}");
            throw;
        }
    }
}