import api from "./api"; // Importa a instância configurada
import type { Contrato } from "../types/contrato";

export async function uploadContratos(arquivo: File): Promise<string> {
  const formData = new FormData();
  formData.append("arquivo", arquivo);

  // O Axios já lida com o erro e com a chave secreta automaticamente
  const response = await api.post("/Contratos/upload", formData);
  return response.data?.mensagem ?? "Sucesso";
}

export async function buscarContratosPorNumero(numeroContrato: string): Promise<Contrato[]> {
  const response = await api.get("/Contratos/buscar-por-contrato", {
    params: { numeroContrato } // O Axios monta a QueryString para você
  });
  return response.data;
}