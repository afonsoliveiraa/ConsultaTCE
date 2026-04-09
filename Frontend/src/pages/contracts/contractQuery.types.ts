export type ContratoColumnId =
  | "tipoDocumento"
  | "codMunicipio"
  | "cpfGestor"
  | "numeroContrato"
  | "dataAssinatura"
  | "tipoObjeto"
  | "modalidade"
  | "cpfGestorOriginal"
  | "numeroContratoOrig"
  | "dataContratoOrig"
  | "vigenciaInicial"
  | "vigenciaFinal"
  | "dataInicioObra"
  | "tipoObraServico"
  | "numeroObra"
  | "dataTerminoObra"
  | "referencia"
  | "dataAutuacao"
  | "numeroProcesso"
  | "valor"
  | "objeto"
  | "cpfFiscal"
  | "nomeFiscal"
  | "idContratoPncp";

export interface ContratoColumnDefinition {
  id: ContratoColumnId;
  label: string;
  active?: boolean;
}

export interface ContratoFilters {
  tipoDocumento: string;
  numeroContrato: string;
  dataAssinatura: string;
  tipoObjeto: string;
  modalidade: string;
  codMunicipio: string;
  cpfGestor: string;
  cpfGestorOriginal: string;
  numeroContratoOrig: string;
  dataContratoOrig: string;
  vigenciaInicial: string;
  vigenciaFinal: string;
  dataInicioObra: string;
  tipoObraServico: string;
  numeroObra: string;
  dataTerminoObra: string;
  referencia: string;
  dataAutuacao: string;
  numeroProcesso: string;
  valor: string;
  objeto: string;
  cpfFiscal: string;
  nomeFiscal: string;
  idContratoPncp: string;
}
