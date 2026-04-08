import { type FunctionalComponent } from "preact";

const steps = ["Dados Basicos", "Unidades Funcionais", "Assuntos"] as const;

// Exibe o breadcrumb da pagina e os blocos visuais de contexto.
export const ContractsTopbar: FunctionalComponent = () => (
  <div class="contracts-topbar">
    <div class="contracts-breadcrumbs">
      <span>Processos</span>
      <span>&rsaquo;</span>
      <strong>Contratos</strong>
    </div>

    <div class="contracts-steps" aria-label="Etapas">
      {steps.map((step) => (
        <div key={step} class="contracts-step">
          <span aria-hidden="true">&#10003;</span>
          <strong>{step}</strong>
        </div>
      ))}
    </div>
  </div>
);
