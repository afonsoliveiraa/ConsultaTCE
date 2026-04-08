import { type FunctionalComponent } from "preact";
import { useRef } from "preact/hooks";
import { useState } from "preact/hooks";
import { uploadContratos } from "../services/contratosApi";

// Pagina dedicada apenas ao fluxo de importacao de arquivos.
export const UploadHistoryPage: FunctionalComponent = () => {
  const inputArquivoRef = useRef<HTMLInputElement>(null);
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [mensagemUpload, setMensagemUpload] = useState("");
  const [erroUpload, setErroUpload] = useState("");
  const [carregandoUpload, setCarregandoUpload] = useState(false);
  const [arrastandoArquivo, setArrastandoArquivo] = useState(false);

  // Atualiza o arquivo visivel na tela antes de iniciar o envio automatico.
  const atualizarArquivoSelecionado = (proximoArquivo: File | null) => {
    setArquivo(proximoArquivo);
    setErroUpload("");
  };

  // Envia o arquivo assim que ele entrar na tela por clique ou arraste.
  const enviarArquivoSelecionado = async (proximoArquivo: File | null) => {
    setMensagemUpload("");
    setErroUpload("");
    atualizarArquivoSelecionado(proximoArquivo);

    if (!proximoArquivo) {
      return;
    }

    setCarregandoUpload(true);

    try {
      const mensagem = await uploadContratos(proximoArquivo);
      setMensagemUpload(mensagem);
    } catch (error) {
      setErroUpload(error instanceof Error ? error.message : "Falha ao enviar o arquivo.");
    } finally {
      setCarregandoUpload(false);
    }
  };

  // Libera o drop do navegador e destaca a area de upload enquanto o arquivo estiver sobre ela.
  const handleDragOver = (event: DragEvent) => {
    event.preventDefault();
    setArrastandoArquivo(true);
  };

  // Remove o destaque quando o arquivo sai da area de upload sem soltar.
  const handleDragLeave = (event: DragEvent) => {
    const elementoAtual = event.currentTarget as HTMLElement | null;
    const proximoAlvo = event.relatedTarget as Node | null;

    if (elementoAtual?.contains(proximoAlvo)) {
      return;
    }

    setArrastandoArquivo(false);
  };

  // Recebe o arquivo arrastado para dentro da tela e reaproveita o fluxo normal de upload.
  const handleDrop = (event: DragEvent) => {
    event.preventDefault();
    setArrastandoArquivo(false);
    void enviarArquivoSelecionado(event.dataTransfer?.files?.[0] ?? null);
  };

  // Abre o seletor nativo de arquivos sem depender do clique na area de arraste.
  const handleAbrirSeletorArquivo = () => {
    inputArquivoRef.current?.click();
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

          <div class="contracts-form">
            <button
              class="contracts-button contracts-button--picker"
              type="button"
              disabled={carregandoUpload}
              onClick={handleAbrirSeletorArquivo}
            >
              {carregandoUpload ? "Enviando..." : "Upload do arquivo"}
            </button>

            <label class="contracts-field">
              <span>Arquivo de contratos</span>
              <div
                class={`contracts-dropzone${arrastandoArquivo ? " is-dragging" : ""}`}
                onDragOver={handleDragOver}
                onDragLeave={handleDragLeave}
                onDrop={handleDrop}
              >
                <div class="contracts-dropzone__content">
                  <strong>Arraste o arquivo para ca</strong>
                  <span>
                    {arquivo
                      ? `Arquivo selecionado: ${arquivo.name}`
                      : "Solte o arquivo nesta area para importar automaticamente."}
                  </span>
                </div>
              </div>
            </label>

            <input
              ref={inputArquivoRef}
              id="arquivo-contratos"
              class="contracts-file-input-hidden"
              type="file"
              onChange={(event) => void enviarArquivoSelecionado(event.currentTarget.files?.[0] ?? null)}
            />
          </div>

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
