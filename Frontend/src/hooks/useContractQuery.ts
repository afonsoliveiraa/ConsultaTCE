import { useMemo, useState } from "preact/hooks";
import { buscarContratosPorNumero } from "../services/contratosApi";
import type { Contrato } from "../types/contrato";
import { contratoColumns, emptyFilters } from "../pages/contracts/contractQuery.constants";
import type {
  ContratoColumnDefinition,
  ContratoColumnId,
  ContratoFilters,
} from "../pages/contracts/contractQuery.types";
import { buildContratosCsv, filterContratos } from "../pages/contracts/contractQuery.utils";

// Encapsula o estado e as regras da consulta de contratos.
export function useContractQuery() {
  const [numeroContrato, setNumeroContrato] = useState("");
  const [contratos, setContratos] = useState<Contrato[]>([]);
  const [mensagemConsulta, setMensagemConsulta] = useState("");
  const [erroConsulta, setErroConsulta] = useState("");
  const [carregandoConsulta, setCarregandoConsulta] = useState(false);
  const [showColumnModal, setShowColumnModal] = useState(false);
  const [columns, setColumns] = useState<ContratoColumnDefinition[]>(contratoColumns);
  const [draggingColumnId, setDraggingColumnId] = useState<ContratoColumnId | null>(null);
  const [dropTargetColumnId, setDropTargetColumnId] = useState<ContratoColumnId | null>(null);
  const [quickSearch, setQuickSearch] = useState("");
  const [filters, setFilters] = useState<ContratoFilters>(emptyFilters);

  // Mantem apenas as colunas habilitadas no modal de configuracao.
  const visibleColumns = useMemo(
    () => columns.filter((column) => column.active !== false),
    [columns],
  );

  // Aplica busca rapida e filtros locais sobre a lista devolvida pela API.
  const filteredContratos = useMemo(
    () => filterContratos(contratos, filters, quickSearch),
    [contratos, filters, quickSearch],
  );

  // Atualiza a visibilidade das colunas diretamente do modal.
  const setColumnVisibility = (columnId: ContratoColumnId, checked: boolean) => {
    setColumns((current) =>
      current.map((column) =>
        column.id === columnId
          ? {
              ...column,
              active: checked,
            }
          : column,
      ),
    );
  };

  // Reordena as colunas pela mesma interacao de arrastar/soltar usada na grade.
  const handleColumnDrop = (targetId: ContratoColumnId) => {
    if (!draggingColumnId || draggingColumnId === targetId) {
      setDraggingColumnId(null);
      setDropTargetColumnId(null);
      return;
    }

    setColumns((current) => {
      const sourceIndex = current.findIndex((column) => column.id === draggingColumnId);
      const targetIndex = current.findIndex((column) => column.id === targetId);

      if (sourceIndex < 0 || targetIndex < 0) {
        return current;
      }

      const reordered = [...current];
      const [removed] = reordered.splice(sourceIndex, 1);
      reordered.splice(targetIndex, 0, removed);
      return reordered;
    });

    setDraggingColumnId(null);
    setDropTargetColumnId(null);
  };

  // Executa a consulta por numero do contrato e atualiza a grade de resultados.
  const handleBuscarContrato = async (event: Event) => {
    event.preventDefault();
    setMensagemConsulta("");
    setErroConsulta("");
    setContratos([]);

    const numeroNormalizado = numeroContrato.trim();

    if (!numeroNormalizado) {
      setErroConsulta("Informe o numero do contrato para consultar.");
      return;
    }

    setCarregandoConsulta(true);

    try {
      const resultado = await buscarContratosPorNumero(numeroNormalizado);
      setContratos(resultado);
      setMensagemConsulta(
        resultado.length === 1
          ? "1 contrato encontrado."
          : `${resultado.length} contratos encontrados.`,
      );
    } catch (error) {
      setErroConsulta(error instanceof Error ? error.message : "Falha ao consultar o contrato.");
    } finally {
      setCarregandoConsulta(false);
    }
  };

  // Exporta a grade filtrada para CSV usando as colunas visiveis na ordem atual.
  const handleExportCsv = () => {
    if (filteredContratos.length === 0) {
      return;
    }

    const csvContent = buildContratosCsv(filteredContratos, visibleColumns);
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");

    link.href = url;
    link.download = "consulta-contratos.csv";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  return {
    numeroContrato,
    contratos,
    filteredContratos,
    mensagemConsulta,
    erroConsulta,
    carregandoConsulta,
    showColumnModal,
    columns,
    visibleColumns,
    dropTargetColumnId,
    quickSearch,
    filters,
    setNumeroContrato,
    setQuickSearch,
    setFilters,
    setShowColumnModal,
    setDraggingColumnId,
    setDropTargetColumnId,
    setColumnVisibility,
    handleColumnDrop,
    handleBuscarContrato,
    handleExportCsv,
  };
}
