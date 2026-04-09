import { type FunctionalComponent } from "preact";
import { useRef, useState } from "preact/hooks";
import { uploadContratos } from "../services/contratosApi";

// A tela volta a ser um upload simples: o backend detecta o periodo pela Referencia.
// Se precisar restaurar o seletor de exercicio, este e o arquivo certo para recolocar o campo.
export const UploadHistoryPage: FunctionalComponent = () => {
  const inputArquivoRef = useRef<HTMLInputElement>(null);
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [mensagemUpload, setMensagemUpload] = useState("");
  const [erroUpload, setErroUpload] = useState("");
  const [carregandoUpload, setCarregandoUpload] = useState(false);
  const [arrastandoArquivo, setArrastandoArquivo] = useState(false);

  const atualizarArquivoSelecionado = (proximoArquivo: File | null) => {
    setArquivo(proximoArquivo);
    setErroUpload("");
  };

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

  const handleDragOver = (event: DragEvent) => {
    event.preventDefault();
    setArrastandoArquivo(true);
  };

  const handleDragLeave = (event: DragEvent) => {
    const elementoAtual = event.currentTarget as HTMLElement | null;
    const proximoAlvo = event.relatedTarget as Node | null;

    if (elementoAtual?.contains(proximoAlvo)) {
      return;
    }

    setArrastandoArquivo(false);
  };

  const handleDrop = (event: DragEvent) => {
    event.preventDefault();
    setArrastandoArquivo(false);
    void enviarArquivoSelecionado(event.dataTransfer?.files?.[0] ?? null);
  };

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
              {carregandoUpload ? "Enviando..." : "Selecione o arquivo"}
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
              onChange={(event) => {
                void enviarArquivoSelecionado(event.currentTarget.files?.[0] ?? null);
              }}
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
