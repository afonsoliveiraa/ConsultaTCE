import api from "./api";
import type { ContratoPagedResult } from "../types/contrato";

export async function uploadContratos(arquivo: File): Promise<string> {
  const formData = new FormData();
  formData.append("arquivo", arquivo);

  // O Axios já lida com o erro e com a chave secreta automaticamente.
  const response = await api.post("/Contratos/upload", formData);
  return response.data?.mensagem ?? "Sucesso";
}

export async function buscarContratosPorNumero(
  numeroContrato: string,
  page = 1,
  pageSize = 50,
): Promise<ContratoPagedResult> {
  const response = await api.get("/Contratos/buscar-por-contrato", {
    params: { numeroContrato, page, pageSize },
  });

  return response.data;
}
