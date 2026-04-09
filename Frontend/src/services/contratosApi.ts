import api from "./api";
import type { ContratoPagedResult } from "../types/contrato";

export async function uploadContratos(arquivo: File): Promise<string> {
  const formData = new FormData();
  formData.append("arquivo", arquivo);

  // O backend agora detecta o periodo pela Referencia do arquivo.
  // Se quiser voltar ao fluxo antigo, e so recolocar o campo exerc aqui.
  const response = await api.post("/Contratos/upload", formData);
  return response.data?.mensagem ?? "Sucesso";
}

export async function buscarContratosPorNumero(
  numeroContrato: string,
  page = 1,
  pageSize = 20,
): Promise<ContratoPagedResult> {
  const response = await api.get("/Contratos/buscar-por-contrato", {
    params: { numeroContrato, page, pageSize },
  });

  return response.data;
}
