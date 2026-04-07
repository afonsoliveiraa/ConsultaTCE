import { type FunctionalComponent } from "preact";
import { Button } from "./Button";

export const PaginationControls: FunctionalComponent<{
  page: number;
  totalPages: number;
  loading?: boolean;
  onPrevious: () => void;
  onNext: () => void;
}> = ({ page, totalPages, loading, onPrevious, onNext }) => (
  <div class="pagination-bar">
    <Button variant="ghost" disabled={page <= 1 || loading} onClick={onPrevious}>
      Anterior
    </Button>
    <span>
      Pagina {page} de {Math.max(totalPages, 1)}
    </span>
    <Button variant="primary" disabled={loading || page >= totalPages} onClick={onNext}>
      Proxima
    </Button>
  </div>
);
