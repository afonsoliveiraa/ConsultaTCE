// Representa o formato retornado pela API para consulta de contratos.
export interface Contrato {
  tipoDocumento: string;
  codMunicipio: string;
  cpfGestor: string;
  numeroContrato: string;
  dataAssinatura: string | null;
  modalidade: string;
  vigenciaInicial: string | null;
  vigenciaFinal: string | null;
  referencia: string | null;
  objeto: string;
  valor: number;
  cpfFiscal: string;
  nomeFiscal: string;
}
