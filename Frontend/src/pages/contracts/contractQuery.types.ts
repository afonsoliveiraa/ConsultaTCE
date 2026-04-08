export type ContratoColumnId =
  | "numeroContrato"
  | "codMunicipio"
  | "cpfGestor"
  | "objeto"
  | "cpfFiscal"
  | "nomeFiscal";

export interface ContratoColumnDefinition {
  id: ContratoColumnId;
  label: string;
  active?: boolean;
}

export interface ContratoFilters {
  numeroContrato: string;
  codMunicipio: string;
  cpfGestor: string;
  objeto: string;
  cpfFiscal: string;
  nomeFiscal: string;
}
