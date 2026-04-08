// Representa o formato retornado pela API para consulta de contratos.
export interface Contrato {
  numeroContrato: string;
  codMunicipio: string;
  cpfGestor: string;
  objeto: string;
  cpfFiscal: string;
  nomeFiscal: string;
}
