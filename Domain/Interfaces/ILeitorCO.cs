using Domain.Dtos;

namespace Domain.Interfaces;

public interface ILeitorCO
{
    Task<IEnumerable<ContratoDTO>> LerArquivoAsync(Stream arquivoStream);

}