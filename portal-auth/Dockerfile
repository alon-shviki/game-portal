# ── Stage 1: build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY PortalAuth.csproj .
RUN dotnet restore PortalAuth.csproj
COPY . .
RUN dotnet publish PortalAuth.csproj -c Release -o /publish --no-restore

# ── Stage 2: runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS serve
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "PortalAuth.dll"]
