using Domain.Dtos;

namespace Domain.Interfaces;

public interface ILeitorLI
{
    Task<IEnumerable<LicitacaoDTO>> LerArquivoAsync(Stream arquivo);

}