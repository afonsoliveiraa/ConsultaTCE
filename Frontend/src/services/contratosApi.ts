import type { Contrato } from "../types/contrato";

// Endpoint base exposto pelo controller de contratos no backend.
const contratosEndpoint = "/api/Contratos";

// Centraliza a leitura de erros HTTP para manter as mensagens consistentes na UI.
async function parseError(response: Response): Promise<string> {
  const contentType = response.headers.get("content-type") ?? "";

  if (contentType.includes("application/json")) {
    const body = await response.json();

    if (typeof body === "string") {
      return body;
    }

    if (body?.mensagem) {
      return body.mensagem as string;
    }

    if (body?.title) {
      return body.title as string;
    }
  }

  const text = await response.text();
  return text || "Nao foi possivel concluir a requisicao.";
}

// Envia o arquivo selecionado para o endpoint de importacao.
export async function uploadContratos(arquivo: File): Promise<string> {
  const formData = new FormData();
  formData.append("arquivo", arquivo);

  const response = await fetch(`${contratosEndpoint}/upload`, {
    method: "POST",
    body: formData,
  });

  if (!response.ok) {
    throw new Error(await parseError(response));
  }

  const body = await response.json();
  return body?.mensagem ?? "Contratos processados com sucesso.";
}

// Consulta contratos persistidos no banco a partir do numero informado.
export async function buscarContratosPorNumero(numeroContrato: string): Promise<Contrato[]> {
  const query = new URLSearchParams({ numeroContrato });
  const response = await fetch(`${contratosEndpoint}/buscar-por-contrato?${query.toString()}`);

  if (!response.ok) {
    throw new Error(await parseError(response));
  }

  return (await response.json()) as Contrato[];
}
