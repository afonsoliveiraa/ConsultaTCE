export type ContratoColumnId =
  | "tipoDocumento"
  | "numeroContrato"
  | "dataAssinatura"
  | "modalidade"
  | "codMunicipio"
  | "cpfGestor"
  | "vigenciaInicial"
  | "vigenciaFinal"
  | "referencia"
  | "valor"
  | "objeto"
  | "cpfFiscal"
  | "nomeFiscal";

export interface ContratoColumnDefinition {
  id: ContratoColumnId;
  label: string;
  active?: boolean;
}

export interface ContratoFilters {
  tipoDocumento: string;
  numeroContrato: string;
  dataAssinatura: string;
  modalidade: string;
  codMunicipio: string;
  cpfGestor: string;
  vigenciaInicial: string;
  vigenciaFinal: string;
  referencia: string;
  valor: string;
  objeto: string;
  cpfFiscal: string;
  nomeFiscal: string;
}
