import { type FunctionalComponent } from "preact";
import { MainLayout, UsersFiltersCard, UsersGridDemo } from "./components";

export const App: FunctionalComponent = () => {
  return (
    <MainLayout pageTitle="Inicio">
      <section class="user-view-page">
        <div class="user-view-topbar">
          <div class="breadcrumbs-demo">
            <a href="/">Usuarios</a>
            <span>&rsaquo;</span>
            <strong>Visualizar Usuario</strong>
          </div>

          <div class="stepper-inline">
            <div class="step-pill is-done"><span>✓</span><strong>Dados Basicos</strong></div>
            <div class="step-pill is-done"><span>✓</span><strong>Unidades Funcionais</strong></div>
            <div class="step-pill is-done"><span>✓</span><strong>Assuntos</strong></div>
          </div>
        </div>

        <UsersFiltersCard />
        <UsersGridDemo />
      </section>
    </MainLayout>
  );
};
