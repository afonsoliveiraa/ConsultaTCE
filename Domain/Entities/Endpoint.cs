namespace Domain.Entities;

// Representa um endpoint do catalogo publico do TCE-CE que pode ser exibido
// no front-end e consumido pela camada de aplicacao.
public class Endpoint
{
    // Chave interna usada pelo sistema para identificar o endpoint.
    public string Key { get; init; } = string.Empty;

    // Caminho efetivo consumido na API publica do TCE-CE.
    public string Path { get; init; } = string.Empty;

    // Agrupamento logico apenas para organizar a lista no front-end.
    public string Category { get; init; } = "Outros";

    // Texto exibido para orientar o usuario sobre o que o endpoint retorna.
    public string Description { get; init; } = string.Empty;

    // Parametros obrigatorios que o usuario realmente precisa preencher.
    public IReadOnlyList<EndpointField> RequiredFields { get; init; } = [];

    // Parametros opcionais mantidos no catalogo para evolucoes futuras.
    public IReadOnlyList<EndpointField> OptionalFields { get; init; } = [];
}

// Modela um campo de consulta do endpoint.
public class EndpointField
{
    // Nome tecnico enviado na query string da API externa.
    public string Name { get; init; } = string.Empty;

    // Rotulo amigavel para renderizacao no formulario.
    public string Label { get; init; } = string.Empty;

    // Explicacao complementar exibida abaixo do campo quando disponivel.
    public string Description { get; init; } = string.Empty;

    // Tipo inferido para o front-end escolher o input adequado.
    public string Type { get; init; } = "text";

    // Indica se o campo precisa ser preenchido para a consulta acontecer.
    public bool Required { get; init; }
}
