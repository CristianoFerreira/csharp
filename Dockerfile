FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY csharp.csproj .
RUN dotnet restore csharp.csproj

COPY . .
RUN dotnet publish csharp.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 5001
ENTRYPOINT ["dotnet", "csharp.dll"]
