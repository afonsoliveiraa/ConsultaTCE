using Application.Interfaces;
using Domain.Entities;
using Domain.Models;

namespace Application.Services;

// Orquestra as regras de uso da feature sem conhecer detalhes de HTTP
// ou de parse da resposta remota.
public class TceAppService
{
    private static readonly HashSet<string> InternalRequiredParameters =
        new(["codigo_municipio", "quantidade", "deslocamento"], StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, string> CategoryNames =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Aquisicoes e contratos"] = "Compras e contratos",
            ["Orcamento municipal"] = "Orcamento",
            ["Informacoes basicas"] = "Informacoes gerais",
            ["Pessoal"] = "Servidores"
        };

    private static readonly Dictionary<string, string> EndpointDescriptions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["municipios"] = "Veja a lista de municipios disponiveis para consulta.",
            ["unidades_gestoras"] = "Consulte as unidades responsaveis pela administracao do municipio.",
            ["dados_orcamentos"] = "Acompanhe os principais dados do orcamento municipal.",
            ["contas_bancarias"] = "Veja as contas bancarias vinculadas ao municipio.",
            ["orgaos"] = "Consulte os orgaos que fazem parte da administracao municipal.",
            ["programas"] = "Veja os programas e acoes previstos pelo municipio.",
            ["licitacoes"] = "Acompanhe as licitacoes realizadas pelo municipio.",
            ["licitantes"] = "Veja quem participou das licitacoes do municipio.",
            ["contratados"] = "Consulte as pessoas e empresas contratadas pelo municipio.",
            ["contrato"] = "Consulte os contratos firmados pelo municipio.",
            ["notas_empenhos"] = "Veja as notas de empenho emitidas pelo municipio.",
            ["agentes_publicos"] = "Consulte informacoes sobre servidores e agentes publicos."
        };

    private static readonly Dictionary<string, string> ExplicitFieldLabels =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["exercicio_orcamento"] = "Ano de referencia",
            ["codigo_orgao"] = "Orgao",
            ["codigo_unidade"] = "Unidade",
            ["codigo_programa"] = "Programa",
            ["codigo_ingresso"] = "Tipo de ingresso",
            ["situacao_funcional"] = "Situacao funcional",
            ["numero_documento_negociante"] = "Documento do fornecedor",
            ["nome_negociante"] = "Nome do fornecedor",
            ["numero_licitacao"] = "Numero da licitacao",
            ["modalidade_licitacao"] = "Modalidade da licitacao",
            ["modalidade_processo_administrativo"] = "Modalidade do processo",
            ["tp_licitacao_li"] = "Tipo da licitacao",
            ["numero_contrato"] = "Numero do contrato",
            ["tipo_contrato"] = "Tipo de contrato",
            ["modalidade_contrato"] = "Modalidade do contrato",
            ["numero_empenho"] = "Numero do empenho",
            ["numero_banco"] = "Banco",
            ["numero_agencia"] = "Agencia",
            ["nome_servidor"] = "Nome do servidor",
            ["nome_programa"] = "Programa",
            ["nome_orgao"] = "Orgao",
            ["nome_unidade"] = "Unidade",
            ["nome_municipio"] = "Municipio",
            ["codigo_municipio"] = "Municipio",
            ["geoibgeId"] = "Codigo IBGE",
            ["geonamesId"] = "Codigo GeoNames"
        };

    private readonly ITceService _tceService;

    public TceAppService(ITceService tceService)
    {
        _tceService = tceService;
    }

    // Entrega o catalogo ja filtrado para a experiencia da tela.
    public async Task<IReadOnlyList<Endpoint>> GetCatalogAsync(CancellationToken cancellationToken)
    {
        var endpoints = await _tceService.GetEndpointsAsync(cancellationToken);

        return endpoints
            .Select(PresentEndpointForCatalog)
            .OrderBy(endpoint => endpoint.Category)
            .ThenBy(endpoint => endpoint.Key)
            .ToArray();
    }

    // Exponibiliza os municipios para o seletor principal da pagina.
    public Task<IReadOnlyList<MunicipalityOption>> GetMunicipalitiesAsync(CancellationToken cancellationToken)
        => _tceService.GetMunicipalitiesAsync(cancellationToken);

    // Valida a requisicao recebida e delega a consulta efetiva para a infraestrutura.
    public async Task<QueryResult> QueryAsync(QueryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EndpointKey))
        {
            throw new ArgumentException("O endpoint precisa ser informado.");
        }

        if (string.IsNullOrWhiteSpace(request.MunicipalityCode))
        {
            throw new ArgumentException("O municipio precisa ser informado.");
        }

        return await _tceService.QueryAsync(request, cancellationToken);
    }

    // Ajusta nome, descricao e campos exibidos sem alterar a integracao real com a API externa.
    private static Endpoint PresentEndpointForCatalog(Endpoint endpoint)
    {
        return new Endpoint
        {
            Key = endpoint.Key,
            Path = endpoint.Path,
            Category = BeautifyCategory(endpoint.Category),
            Description = BeautifyEndpointDescription(endpoint),
            RequiredFields = BuildVisibleRequiredFields(endpoint),
            OptionalFields = endpoint.OptionalFields
                .Select(field => BeautifyField(field))
                .OrderBy(field => field.Type == "date" ? 0 : 1)
                .ThenBy(field => field.Label)
                .ToArray()
        };
    }

    // Mantem visiveis apenas os campos que fazem sentido para o usuario final.
    private static IReadOnlyList<EndpointField> BuildVisibleRequiredFields(Endpoint endpoint)
    {
        return endpoint.RequiredFields
            .Where(field => !InternalRequiredParameters.Contains(field.Name))
            .Select(field => BeautifyField(field))
            .OrderBy(field => field.Type == "date" ? 0 : 1)
            .ThenBy(field => field.Label)
            .ToArray();
    }

    // Traduz as categorias para um agrupamento mais publico.
    private static string BeautifyCategory(string category)
    {
        return CategoryNames.TryGetValue(category.Trim(), out var friendlyCategory)
            ? friendlyCategory
            : category;
    }

    // Troca descricoes muito tecnicas por frases curtas e mais diretas.
    private static string BeautifyEndpointDescription(Endpoint endpoint)
    {
        return EndpointDescriptions.TryGetValue(endpoint.Key.Trim(), out var friendlyDescription)
            ? friendlyDescription
            : endpoint.Description;
    }

    // Ajusta rotulos e orientacoes dos campos mostrados no formulario.
    private static EndpointField BeautifyField(EndpointField field)
    {
        var label = BuildFriendlyFieldLabel(field.Name);
        var description = BuildFriendlyFieldDescription(field.Name, field.Type);

        return new EndpointField
        {
            Name = field.Name,
            Label = label,
            Description = description,
            Type = field.Type,
            Required = field.Required
        };
    }

    private static string BuildFriendlyFieldLabel(string fieldName)
    {
        if (ExplicitFieldLabels.TryGetValue(fieldName, out var explicitLabel))
        {
            return explicitLabel;
        }

        if (fieldName.Contains("data_", StringComparison.OrdinalIgnoreCase))
        {
            return fieldName.Trim().ToLowerInvariant() switch
            {
                "data_contrato" => "Data do contrato",
                "data_realizacao_autuacao_licitacao" => "Data da licitacao",
                "data_realizacao_licitacao" => "Data da licitacao",
                "data_referencia" => "Data de referencia",
                "data_referencia_empenho" => "Data de referencia",
                "data_referencia_agente_publico" => "Data de referencia",
                "data_envio_loa" => "Data de envio da LOA",
                "data_aprov_loa" => "Data de aprovacao da LOA",
                "data_public_loa" => "Data de publicacao da LOA",
                "data_emissao_empenho" => "Data de emissao do empenho",
                _ => HumanizeFieldName(fieldName)
            };
        }

        if (fieldName.StartsWith("valor_", StringComparison.OrdinalIgnoreCase))
        {
            return HumanizeFieldName(fieldName).Replace("Valor ", "Valor do ");
        }

        return HumanizeFieldName(fieldName);
    }

    private static string BuildFriendlyFieldDescription(string fieldName, string fieldType)
    {
        if (string.Equals(fieldType, "date", StringComparison.OrdinalIgnoreCase))
        {
            return "Você pode informar uma data exata ou um intervalo entre data inicial e data final.";
        }

        return fieldName.Trim().ToLowerInvariant() switch
        {
            "exercicio_orcamento" => "Informe o ano que deseja consultar.",
            "codigo_orgao" => "Selecione ou informe o orgao desejado.",
            "codigo_unidade" => "Selecione ou informe a unidade desejada.",
            "nome_negociante" => "Informe o nome da pessoa ou empresa que deseja localizar.",
            "numero_documento_negociante" => "Informe o CPF ou CNPJ da pessoa ou empresa.",
            "numero_contrato" => "Informe o numero do contrato, se quiser refinar a busca.",
            "numero_licitacao" => "Informe o numero da licitacao, se quiser refinar a busca.",
            "nome_servidor" => "Informe o nome do servidor para localizar registros relacionados.",
            _ => "Preencha essa informacao para refinar a consulta."
        };
    }

    private static string HumanizeFieldName(string fieldName)
    {
        var words = fieldName
            .Replace("geoibgeId", "codigo_ibge", StringComparison.OrdinalIgnoreCase)
            .Replace("geonamesId", "codigo_geonames", StringComparison.OrdinalIgnoreCase)
            .Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return string.Join(" ", words.Select(CapitalizeWord));
    }

    private static string CapitalizeWord(string word)
    {
        return word.ToLowerInvariant() switch
        {
            "cpf" => "CPF",
            "cnpj" => "CNPJ",
            "ibge" => "IBGE",
            "loa" => "LOA",
            _ => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()
        };
    }
}
