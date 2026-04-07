import { type FunctionalComponent } from "preact";

export const EmptyState: FunctionalComponent<{
  title?: string;
  description?: string;
}> = ({ title = "Nenhum registro encontrado.", description }) => (
  <div class="empty-state">
    <div class="empty-state__icon" aria-hidden="true">
      []
    </div>
    <strong>{title}</strong>
    {description ? <p>{description}</p> : null}
  </div>
);
