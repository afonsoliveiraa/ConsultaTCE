# Sintetiza frontend e backend numa unica imagem final para publicacao.
FROM node:22-alpine AS frontend-build
WORKDIR /src

# Instala as dependencias do frontend primeiro para aproveitar cache de build.
COPY Frontend/package.json Frontend/package-lock.json ./Frontend/
RUN cd Frontend && npm ci

# Copia o frontend e gera os arquivos estaticos em ConsultaTCE/wwwroot.
COPY Frontend ./Frontend
ARG VITE_API_BASE_URL=/api
ARG VITE_APP_SECRET=ChaveLocal123!
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_APP_SECRET=$VITE_APP_SECRET
RUN cd Frontend && npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dotnet-build
WORKDIR /src

# Restaura os projetos .NET antes de copiar o codigo inteiro para melhorar cache.
COPY global.json ./
COPY ConsultaTCE.sln ./
COPY Application/Application.csproj ./Application/
COPY ConsultaTCE/ConsultaTCE.csproj ./ConsultaTCE/
COPY DbMigrator/DbMigrator.csproj ./DbMigrator/
COPY Domain/Domain.csproj ./Domain/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/
RUN dotnet restore ConsultaTCE.sln
RUN dotnet restore DbMigrator/DbMigrator.csproj

# Copia o codigo fonte e injeta o frontend compilado na pasta publica da API.
COPY Application ./Application
COPY ConsultaTCE ./ConsultaTCE
COPY DbMigrator ./DbMigrator
COPY Domain ./Domain
COPY Infrastructure ./Infrastructure
COPY --from=frontend-build /src/ConsultaTCE/wwwroot ./ConsultaTCE/wwwroot

# Publica a API principal e o migrador como artefatos independentes.
RUN dotnet publish ConsultaTCE/ConsultaTCE.csproj -c Release -o /out/app --no-restore
RUN dotnet publish DbMigrator/DbMigrator.csproj -c Release -o /out/migrator --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS app
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=dotnet-build /out/app ./
ENTRYPOINT ["dotnet", "ConsultaTCE.dll"]

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS migrator
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=dotnet-build /out/migrator ./
ENTRYPOINT ["dotnet", "DbMigrator.dll"]
