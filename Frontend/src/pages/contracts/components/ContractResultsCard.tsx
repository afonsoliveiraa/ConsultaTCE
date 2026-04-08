import { type FunctionalComponent } from "preact";
import { ColumnsIcon, DownloadIcon, SearchIcon, SortIcon } from "../../../components/GridIcons";
import type { Contrato } from "../../../types/contrato";
import type { ContratoColumnDefinition, ContratoColumnId } from "../contractQuery.types";

interface ContractResultsCardProps {
  contratos: Contrato[];
  filteredContratos: Contrato[];
  quickSearch: string;
  visibleColumns: ContratoColumnDefinition[];
  dropTargetColumnId: ContratoColumnId | null;
  onQuickSearchChange: (value: string) => void;
  onShowColumnModal: (value: boolean) => void;
  onDragStart: (columnId: ContratoColumnId) => void;
  onDragOver: (event: DragEvent, columnId: ContratoColumnId) => void;
  onDragLeave: (columnId: ContratoColumnId) => void;
  onDrop: (columnId: ContratoColumnId) => void;
  onDragEnd: () => void;
  onExportCsv: () => void;
}

// Exibe a toolbar da grade e os resultados da consulta.
export const ContractResultsCard: FunctionalComponent<ContractResultsCardProps> = ({
  contratos,
  filteredContratos,
  quickSearch,
  visibleColumns,
  dropTargetColumnId,
  onQuickSearchChange,
  onShowColumnModal,
  onDragStart,
  onDragOver,
  onDragLeave,
  onDrop,
  onDragEnd,
  onExportCsv,
}) => (
  <article class="contracts-results contracts-results--consulta">
    <>
      <div class="grid-demo__toolbar contracts-results__toolbar">
        <div class="grid-demo__toolbar-buttons">
          <button class="grid-demo__text-button" type="button" onClick={() => onShowColumnModal(true)}>
            <span class="grid-demo__toolbar-icon contracts-results__action-icon" aria-hidden="true">
              <ColumnsIcon />
            </span>
            Colunas
          </button>

          <button class="grid-demo__text-button" type="button" onClick={onExportCsv}>
            <span class="grid-demo__toolbar-icon contracts-results__action-icon" aria-hidden="true">
              <DownloadIcon />
            </span>
            Exportar
          </button>
        </div>

        <div class="grid-demo__toolbar-actions">
          <label class="grid-demo__search grid-demo__search--compact">
            <div class="grid-demo__search-field">
              <input
                type="text"
                value={quickSearch}
                onInput={(event) => onQuickSearchChange(event.currentTarget.value)}
                placeholder="Busque em qualquer campo"
              />
              <span class="grid-demo__search-icon" aria-hidden="true">
                <SearchIcon />
              </span>
            </div>
          </label>
        </div>
      </div>

      <div class="contracts-table-wrap">
        <table class="contracts-table">
          <thead>
            <tr>
              {visibleColumns.map((column) => (
                <th
                  key={column.id}
                  class={`grid-demo__head-cell${dropTargetColumnId === column.id ? " is-drop-target" : ""}`}
                  draggable
                  onDragStart={() => onDragStart(column.id)}
                  onDragOver={(event) => onDragOver(event, column.id)}
                  onDragLeave={() => onDragLeave(column.id)}
                  onDrop={() => onDrop(column.id)}
                  onDragEnd={onDragEnd}
                >
                  <button class="grid-demo__sort-button grid-demo__sort-button--active" type="button">
                    <span>{column.label}</span>
                    <SortIcon active />
                  </button>
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {filteredContratos.length > 0 ? (
              filteredContratos.map((contrato) => (
                <tr key={`${contrato.numeroContrato}-${contrato.cpfFiscal}`}>
                  {visibleColumns.map((column) => (
                    <td key={column.id}>{contrato[column.id]}</td>
                  ))}
                </tr>
              ))
            ) : (
              <tr>
                <td class="contracts-table__empty-cell" colSpan={visibleColumns.length}>
                  {contratos.length === 0
                    ? "Pesquise um numero de contrato para carregar os dados na grade."
                    : "Nenhum contrato corresponde aos filtros aplicados."}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </>
  </article>
);
