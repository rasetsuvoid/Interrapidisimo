# ── Stage 1: Build ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Restore — copy only .csproj files first for layer caching
COPY src/Domain/Interrapidisimo.Domain.csproj             src/Domain/
COPY src/Application/Interrapidisimo.Application.csproj   src/Application/
COPY src/Infrastructure/Interrapidisimo.Infrastructure.csproj src/Infrastructure/
COPY src/API/Interrapidisimo.API.csproj                   src/API/
RUN dotnet restore src/API/Interrapidisimo.API.csproj

# Build & publish
COPY src/ src/
RUN dotnet publish src/API/Interrapidisimo.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Interrapidisimo.API.dll"]
