import type { Contrato } from "../../types/contrato";
import type { ContratoColumnDefinition, ContratoColumnId, ContratoFilters } from "./contractQuery.types";

const dateColumns = new Set<ContratoColumnId>([
  "dataAssinatura",
  "dataContratoOrig",
  "vigenciaInicial",
  "vigenciaFinal",
  "dataInicioObra",
  "dataTerminoObra",
  "referencia",
  "dataAutuacao",
]);

// Escapa valores para gerar um CSV valido mesmo com aspas, virgulas ou quebras de linha.
function escapeCsvValue(value: string) {
  const normalizedValue = value.replace(/"/g, "\"\"");
  return `"${normalizedValue}"`;
}

function normalizeContratoValue(value: string | number | null) {
  return value == null ? "" : String(value);
}

function formatBrazilianDate(value: string | null) {
  if (!value) {
    return "";
  }

  const isoDateMatch = /^(\d{4})-(\d{2})-(\d{2})/.exec(value);
  if (isoDateMatch) {
    const [, year, month, day] = isoDateMatch;
    return `${day}/${month}/${year}`;
  }

  const parsedDate = new Date(value);
  if (Number.isNaN(parsedDate.getTime())) {
    return value;
  }

  return new Intl.DateTimeFormat("pt-BR").format(parsedDate);
}

export function formatContratoValue(contrato: Contrato, field: ContratoColumnId) {
  const value = contrato[field];

  if (dateColumns.has(field)) {
    return formatBrazilianDate(value as string | null);
  }

  return normalizeContratoValue(value);
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
      (Object.keys(contrato) as ContratoColumnId[]).some((field) =>
        formatContratoValue(contrato, field).toLowerCase().includes(quickSearchTerm),
      );

    if (!matchesQuickSearch) {
      return false;
    }

    return (Object.entries(filters) as [ContratoColumnId, string][]).every(([field, value]) => {
      const filterTerm = value.trim().toLowerCase();

      if (!filterTerm) {
        return true;
      }

      return formatContratoValue(contrato, field).toLowerCase().includes(filterTerm);
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
      .map((column) => escapeCsvValue(formatContratoValue(contrato, column.id)))
      .join(";"),
  );

  return [header, ...rows].join("\n");
}
