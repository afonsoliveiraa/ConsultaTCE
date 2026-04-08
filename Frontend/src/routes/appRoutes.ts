export type AppRouteKey = "upload" | "consulta" | "not-found";

export interface AppRoute {
  key: AppRouteKey;
  title: string;
}

// Centraliza a resolucao das rotas conhecidas do frontend.
export function resolveAppRoute(pathname: string): AppRoute {
  if (pathname === "/" || pathname === "/upload-de-historico") {
    return {
      key: "upload",
      title: "Upload de historico",
    };
  }

  if (pathname === "/consulta-de-contrato") {
    return {
      key: "consulta",
      title: "Consulta de contrato",
    };
  }

  return {
    key: "not-found",
    title: "Pagina nao encontrada",
  };
}
