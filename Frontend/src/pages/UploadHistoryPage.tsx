import { type FunctionalComponent } from "preact";
import { useState } from "preact/hooks";
import { uploadContratos } from "../services/contratosApi";

// Pagina dedicada apenas ao fluxo de importacao de arquivos.
export const UploadHistoryPage: FunctionalComponent = () => {
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [mensagemUpload, setMensagemUpload] = useState("");
  const [erroUpload, setErroUpload] = useState("");
  const [carregandoUpload, setCarregandoUpload] = useState(false);

  // Processa o envio do arquivo para o backend e limpa o input ao concluir.
  const handleUpload = async (event: Event) => {
    event.preventDefault();
    setMensagemUpload("");
    setErroUpload("");

    if (!arquivo) {
      setErroUpload("Selecione um arquivo antes de enviar.");
      return;
    }

    setCarregandoUpload(true);

    try {
      const mensagem = await uploadContratos(arquivo);
      setMensagemUpload(mensagem);
      setArquivo(null);

      // Limpa o input nativo para permitir reenviar o mesmo arquivo se necessario.
      const input = document.getElementById("arquivo-contratos") as HTMLInputElement | null;
      if (input) {
        input.value = "";
      }
    } catch (error) {
      setErroUpload(error instanceof Error ? error.message : "Falha ao enviar o arquivo.");
    } finally {
      setCarregandoUpload(false);
    }
  };

  return (
    <div class="contracts-upload-shell">
      <div class="contracts-grid contracts-grid--single">
        <article class="contracts-card contracts-card--upload-compact">
          <div class="contracts-card__header">
            <div>
              <h2>Importar arquivo</h2>
            </div>
          </div>

          <form class="contracts-form" onSubmit={handleUpload}>
            <label class="contracts-field" for="arquivo-contratos">
              <span>Arquivo de contratos</span>
              <input
                id="arquivo-contratos"
                type="file"
                onChange={(event) => setArquivo(event.currentTarget.files?.[0] ?? null)}
              />
            </label>

            <button class="contracts-button" type="submit" disabled={carregandoUpload}>
              {carregandoUpload ? "Enviando arquivo..." : "Enviar arquivo"}
            </button>
          </form>

          {arquivo ? (
            <p class="contracts-feedback contracts-feedback--neutral">
              Arquivo selecionado: <strong>{arquivo.name}</strong>
            </p>
          ) : null}

          {mensagemUpload ? (
            <p class="contracts-feedback contracts-feedback--success">{mensagemUpload}</p>
          ) : null}

          {erroUpload ? (
            <p class="contracts-feedback contracts-feedback--error">{erroUpload}</p>
          ) : null}
        </article>
      </div>
    </div>
  );
};
