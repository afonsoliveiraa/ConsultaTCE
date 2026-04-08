import type { ContratoColumnDefinition, ContratoFilters } from "./contractQuery.types";

// Define as colunas disponiveis na grade e sua ordem inicial.
export const contratoColumns: ContratoColumnDefinition[] = [
  { id: "numeroContrato", label: "Numero", active: true },
  { id: "codMunicipio", label: "Municipio", active: true },
  { id: "cpfGestor", label: "CPF Gestor", active: true },
  { id: "objeto", label: "Objeto", active: true },
  { id: "cpfFiscal", label: "CPF Fiscal", active: true },
  { id: "nomeFiscal", label: "Nome Fiscal", active: true },
];

// Mantem o formato padrao dos filtros locais da grade.
export const emptyFilters: ContratoFilters = {
  numeroContrato: "",
  codMunicipio: "",
  cpfGestor: "",
  objeto: "",
  cpfFiscal: "",
  nomeFiscal: "",
};
