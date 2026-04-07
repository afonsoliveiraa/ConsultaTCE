import { type FunctionalComponent } from "preact";
import { useEffect, useMemo, useState } from "preact/hooks";
import {
  AppShell,
  DataTable,
  Notice,
  Panel,
  PaginationControls,
  SelectField,
  StatCard,
  TextField,
} from "./components";

type ResourceDescriptor = {
  key: string;
  path: string;
  category?: string | null;
  description?: string | null;
  requiredQueryParameters: string[];
  optionalQueryParameters: string[];
  queryParameters: QueryParameterDescriptor[];
  requiresAuthentication: boolean;
};

type QueryParameterDescriptor = {
  name: string;
  required: boolean;
  description?: string | null;
  type?: string | null;
};

type ResourceCatalogResponse = {
  baseUrl: string;
  resources: ResourceDescriptor[];
};

type PaginatedEnvelope = {
  resource: string;
  sourceUrl: string;
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  items: Record<string, unknown>[];
  metadata: Record<string, unknown>;
  cachedAtUtc: string;
  expiresAtUtc: string;
};

type HealthResponse = {
  status: string;
  service: string;
  timestampUtc: string;
};

type QueryValues = Record<string, string>;

const initialQueryValues: QueryValues = {
  codigo_municipio: "",
  termo: "",
};

const securityPort = import.meta.env.VITE_BACKEND_HTTPS_PORT ?? "7113";

const buildRequestUrl = (
  resourceKey: string,
  page: number,
  pageSize: number,
  queryValues: QueryValues
) => {
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  });

  Object.entries(queryValues).forEach(([key, value]) => {
    if (value.trim()) {
      params.set(key, value.trim());
    }
  });

  return `/api/resources/${resourceKey}?${params.toString()}`;
};

export const App: FunctionalComponent = () => {
  const [health, setHealth] = useState<HealthResponse | null>(null);
  const [catalog, setCatalog] = useState<ResourceCatalogResponse | null>(null);
  const [selectedResource, setSelectedResource] = useState("");
  const [queryValues, setQueryValues] = useState<QueryValues>(initialQueryValues);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [result, setResult] = useState<PaginatedEnvelope | null>(null);
  const [loadingCatalog, setLoadingCatalog] = useState(true);
  const [loadingResult, setLoadingResult] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    const loadInitialState = async () => {
      try {
        const [healthResponse, catalogResponse] = await Promise.all([
          fetch("/api/health"),
          fetch("/api/resources"),
        ]);

        if (!healthResponse.ok || !catalogResponse.ok) {
          throw new Error("Falha ao carregar a API inicial.");
        }

        const nextHealth = (await healthResponse.json()) as HealthResponse;
        const nextCatalog = (await catalogResponse.json()) as ResourceCatalogResponse;

        setHealth(nextHealth);
        setCatalog(nextCatalog);
        setSelectedResource(nextCatalog.resources[0]?.key ?? "");
      } catch (error) {
        setErrorMessage(
          error instanceof Error ? error.message : "Erro inesperado ao iniciar a interface."
        );
      } finally {
        setLoadingCatalog(false);
      }
    };

    void loadInitialState();
  }, []);

  useEffect(() => {
    if (!selectedResource) {
      return;
    }

    const loadResource = async () => {
      setLoadingResult(true);
      setErrorMessage("");

      try {
        const response = await fetch(
          buildRequestUrl(selectedResource, page, pageSize, queryValues)
        );

        if (!response.ok) {
          throw new Error("Nao foi possivel carregar o recurso selecionado.");
        }

        const payload = (await response.json()) as PaginatedEnvelope;
        setResult(payload);
      } catch (error) {
        setResult(null);
        setErrorMessage(error instanceof Error ? error.message : "Erro ao consultar o backend.");
      } finally {
        setLoadingResult(false);
      }
    };

    void loadResource();
  }, [selectedResource, page, pageSize, queryValues]);

  const resourceOptions = useMemo(
    () =>
      (catalog?.resources ?? []).map((resource) => ({
        label: `${resource.key} | ${resource.category ?? "Sem categoria"}`,
        value: resource.key,
      })),
    [catalog]
  );

  const activeResource = useMemo(
    () => catalog?.resources.find((resource) => resource.key === selectedResource) ?? null,
    [catalog, selectedResource]
  );

  const columns = useMemo(() => {
    const firstRow = result?.items[0];
    return firstRow ? Object.keys(firstRow) : [];
  }, [result]);

  return (
    <AppShell>
      <div class="stack">
        <Panel
          title="Status da integracao"
          subtitle={`Resumo do servico e do catalogo devolvido pelo backend atual. Porta segura integrada: ${securityPort}.`}
        >
          <div class="stats-grid">
            <StatCard label="Servico" value={health?.service ?? "Carregando"} tone="primary" />
            <StatCard label="Saude" value={health?.status ?? "..."} tone="success" />
            <StatCard label="Recursos" value={String(catalog?.resources.length ?? 0)} tone="info" />
            <StatCard
              label="Ultima leitura"
              value={health ? new Date(health.timestampUtc).toLocaleString("pt-BR") : "..."}
              tone="warning"
            />
          </div>
        </Panel>

        <Panel
          title="Consulta"
          subtitle="Componentes essenciais reaproveitados: painel, campos, alertas, vazio e paginacao."
        >
          <div class="filters-grid">
            <SelectField
              label="Recurso"
              value={selectedResource}
              options={
                resourceOptions.length > 0
                  ? resourceOptions
                  : [{ label: loadingCatalog ? "Carregando..." : "Sem recursos", value: "" }]
              }
              onChange={(value) => {
                setSelectedResource(value);
                setPage(1);
              }}
            />
            <TextField
              label="Codigo do municipio"
              value={queryValues.codigo_municipio}
              placeholder="Ex.: 3550308"
              onInput={(value) => {
                setQueryValues((current) => ({ ...current, codigo_municipio: value }));
                setPage(1);
              }}
            />
            <TextField
              label="Termo"
              value={queryValues.termo}
              placeholder="Buscar por descricao"
              onInput={(value) => {
                setQueryValues((current) => ({ ...current, termo: value }));
                setPage(1);
              }}
            />
            <SelectField
              label="Itens por pagina"
              value={String(pageSize)}
              options={[
                { label: "10", value: "10" },
                { label: "25", value: "25" },
                { label: "50", value: "50" },
              ]}
              onChange={(value) => {
                setPageSize(Number(value));
                setPage(1);
              }}
            />
          </div>

          {activeResource ? (
            <div class="resource-summary">
              <strong>{activeResource.key}</strong>
              <p>{activeResource.description ?? "Sem descricao."}</p>
              <small>
                Parametros disponiveis:{" "}
                {activeResource.queryParameters.length > 0
                  ? activeResource.queryParameters.map((parameter) => parameter.name).join(", ")
                  : "nenhum"}
              </small>
            </div>
          ) : null}

          {errorMessage ? <Notice tone="error">{errorMessage}</Notice> : null}

          <DataTable columns={columns} rows={result?.items ?? []} loading={loadingResult} />

          <PaginationControls
            page={result?.page ?? page}
            totalPages={result?.totalPages ?? 1}
            loading={loadingResult}
            onPrevious={() => setPage((current) => Math.max(1, current - 1))}
            onNext={() => setPage((current) => current + 1)}
          />
        </Panel>
      </div>
    </AppShell>
  );
};
