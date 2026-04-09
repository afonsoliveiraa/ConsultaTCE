import type { ContratoColumnDefinition, ContratoFilters } from "./contractQuery.types";

// Define as colunas disponiveis na grade e sua ordem inicial.
export const contratoColumns: ContratoColumnDefinition[] = [
  { id: "tipoDocumento", label: "Tipo Documento", active: true },
  { id: "numeroContrato", label: "Numero", active: true },
  { id: "dataAssinatura", label: "Data Assinatura", active: true },
  { id: "modalidade", label: "Modalidade", active: true },
  { id: "codMunicipio", label: "Municipio", active: true },
  { id: "cpfGestor", label: "CPF Gestor", active: true },
  { id: "vigenciaInicial", label: "Vigencia Inicial", active: true },
  { id: "vigenciaFinal", label: "Vigencia Final", active: true },
  { id: "referencia", label: "Referencia", active: true },
  { id: "valor", label: "Valor", active: true },
  { id: "objeto", label: "Objeto", active: true },
  { id: "cpfFiscal", label: "CPF Fiscal", active: true },
  { id: "nomeFiscal", label: "Nome Fiscal", active: true },
];

// Mantem o formato padrao dos filtros locais da grade.
export const emptyFilters: ContratoFilters = {
  tipoDocumento: "",
  numeroContrato: "",
  dataAssinatura: "",
  modalidade: "",
  codMunicipio: "",
  cpfGestor: "",
  vigenciaInicial: "",
  vigenciaFinal: "",
  referencia: "",
  valor: "",
  objeto: "",
  cpfFiscal: "",
  nomeFiscal: "",
};
