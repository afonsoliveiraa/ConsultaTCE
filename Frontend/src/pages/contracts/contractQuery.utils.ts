import type { Contrato } from "../../types/contrato";
import type { ContratoColumnDefinition, ContratoColumnId, ContratoFilters } from "./contractQuery.types";

// Escapa valores para gerar um CSV valido mesmo com aspas, virgulas ou quebras de linha.
function escapeCsvValue(value: string) {
  const normalizedValue = value.replace(/"/g, "\"\"");
  return `"${normalizedValue}"`;
}

// Aplica a busca rapida e os filtros locais sobre a lista retornada pela API.
export function filterContratos(
  contratos: Contrato[],
  filters: ContratoFilters,
  quickSearch: string,
) {
  const quickSearchTerm = quickSearch.trim().toLowerCase();

  return contratos.filter((contrato) => {
    const matchesQuickSearch =
      quickSearchTerm.length === 0 ||
      Object.values(contrato).some((value) =>
        String(value).toLowerCase().includes(quickSearchTerm),
      );

    if (!matchesQuickSearch) {
      return false;
    }

    return (Object.entries(filters) as [ContratoColumnId, string][]).every(([field, value]) => {
      const filterTerm = value.trim().toLowerCase();

      if (!filterTerm) {
        return true;
      }

      return String(contrato[field]).toLowerCase().includes(filterTerm);
    });
  });
}

// Monta o conteudo CSV com base nas colunas visiveis e na ordem atual da grade.
export function buildContratosCsv(
  contratos: Contrato[],
  visibleColumns: ContratoColumnDefinition[],
) {
  const header = visibleColumns.map((column) => escapeCsvValue(column.label)).join(";");
  const rows = contratos.map((contrato) =>
    visibleColumns
      .map((column) => escapeCsvValue(String(contrato[column.id] ?? "")))
      .join(";"),
  );

  return [header, ...rows].join("\n");
}
