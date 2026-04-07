import { type FunctionalComponent } from "preact";
import { EmptyState } from "./EmptyState";

const formatValue = (value: unknown) => {
  if (value === null || value === undefined || value === "") {
    return "—";
  }

  if (typeof value === "boolean") {
    return value ? "Sim" : "Não";
  }

  return String(value);
};

export const DataTable: FunctionalComponent<{
  columns: string[];
  rows: Record<string, unknown>[];
  loading: boolean;
}> = ({ columns, rows, loading }) => (
  <div class="table-shell">
    <table>
      <thead>
        <tr>
          {columns.map((column) => (
            <th key={column}>{column}</th>
          ))}
        </tr>
      </thead>
      <tbody>
        {loading ? (
          <tr>
            <td colSpan={Math.max(columns.length, 1)} class="table-feedback">
              Carregando dados...
            </td>
          </tr>
        ) : rows.length === 0 ? (
          <tr>
            <td colSpan={Math.max(columns.length, 1)} class="table-feedback">
              <EmptyState
                title="Nenhum registro encontrado."
                description="Ajuste os filtros ou selecione outro recurso para consultar."
              />
            </td>
          </tr>
        ) : (
          rows.map((row, rowIndex) => (
            <tr key={String(row.id ?? rowIndex)}>
              {columns.map((column) => (
                <td key={`${rowIndex}-${column}`}>{formatValue(row[column])}</td>
              ))}
            </tr>
          ))
        )}
      </tbody>
    </table>
  </div>
);
