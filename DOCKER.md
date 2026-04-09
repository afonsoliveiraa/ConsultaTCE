# Docker

## Estrutura

- `db`: PostgreSQL com volume persistente
- `db-migrator`: aplica as migrations antes da API subir
- `app`: ASP.NET Core servindo API e frontend publicado em `wwwroot`

## Preparacao

1. Copie `.env.docker.example` para `.env`
2. Ajuste senha do banco, porta publica e origem publica da aplicacao

## Subida local com Docker

```powershell
docker compose up --build -d
```

## Logs

```powershell
docker compose logs -f app
docker compose logs -f migrator
docker compose logs -f db
```

## Parada

```powershell
docker compose down
```

## Publicacao

- O container da aplicacao expoe HTTP interno na porta `8080`
- Em producao, termine HTTPS no proxy reverso ou balanceador
- A origem publica deve refletir a URL final da aplicacao em `APP_PUBLIC_ORIGIN`
- Se a chave `X-App-Secret` continuar sendo usada, `FRONTEND_SECRET` precisa ser igual no build do frontend e no backend
