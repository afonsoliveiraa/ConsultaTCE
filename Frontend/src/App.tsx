import { type FunctionalComponent } from "preact";
import { MainLayout } from "./components";
import { ContractQueryPage, NotFoundPage, UploadHistoryPage } from "./pages";
import { resolveAppRoute } from "./routes/appRoutes";

export const App: FunctionalComponent = () => {
  // Usa o modulo de rotas para manter a resolucao das paginas em um unico lugar.
  const currentPage = resolveAppRoute(window.location.pathname);

  return (
    <MainLayout pageTitle={currentPage.title}>
      <section class={`contracts-page${currentPage.key === "consulta" ? " contracts-page--consulta" : ""}`}>
        {currentPage.key === "upload" ? <UploadHistoryPage /> : null}
        {currentPage.key === "consulta" ? <ContractQueryPage /> : null}
        {currentPage.key === "not-found" ? <NotFoundPage /> : null}
      </section>
    </MainLayout>
  );
};
