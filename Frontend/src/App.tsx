import { type FunctionalComponent } from "preact";
import { AppShell, Panel, StatCard } from "./components";

const securityPort = import.meta.env.VITE_BACKEND_HTTPS_PORT ?? "7113";

export const App: FunctionalComponent = () => {
  return (
    <AppShell>
      <div class="stack">
        <Panel
          title="Ambiente base"
          subtitle={`Projeto limpo para comecar do zero, mantendo apenas as configuracoes principais. Porta segura do backend: ${securityPort}.`}
        >
          <div class="stats-grid">
            <StatCard label="Backend HTTPS" value={`https://localhost:${securityPort}`} tone="primary" />
            <StatCard label="Frontend HTTPS" value="https://localhost:3000" tone="success" />
            <StatCard label="CORS" value="Apenas frontend HTTPS" tone="info" />
            <StatCard label="Swagger" value="Disponivel em /swagger" tone="warning" />
          </div>
        </Panel>

        <Panel
          title="Estado atual"
          subtitle="A camada HTTP funcional foi removida para deixar o projeto como ambiente inicial configurado."
        >
          <div class="resource-summary">
            <strong>O que ficou ativo</strong>
            <p>HTTPS no backend, frontend em Preact, CORS configurado, Swagger no desenvolvimento e entrega de arquivos estaticos pelo host .NET.</p>
            <small>Adicione controllers, services e regras de negocio quando for iniciar o desenvolvimento funcional.</small>
          </div>
        </Panel>
      </div>
    </AppShell>
  );
};
