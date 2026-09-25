FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY csharp.csproj .
RUN dotnet restore csharp.csproj

COPY . .
RUN dotnet publish csharp.csproj -c Release -o /app --no-restore

# Gera o contrato OpenAPI a partir do assembly recém-compilado, sem subir a
# aplicação (Swashbuckle CLI). Fica junto do publish em /app e segue pra
# imagem final; o pipeline da plataforma extrai esse arquivo dali para
# publicar a API no catálogo do Backstage.
RUN dotnet tool restore \
    && dotnet tool run swagger tofile --output /app/openapi.json /app/csharp.dll v1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 5001
ENTRYPOINT ["dotnet", "csharp.dll"]
