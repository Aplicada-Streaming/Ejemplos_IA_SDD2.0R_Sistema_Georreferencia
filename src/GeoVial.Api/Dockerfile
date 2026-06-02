# syntax=docker/dockerfile:1
# Imagen backend del monolito GeoVial (GeoVial.Api + Application + Infrastructure + FileHosting).
# Build multi-stage: SDK .NET 10 para compilar/publicar, runtime ASP.NET minimo para ejecutar.
# Contexto de build: la raiz del repo (los proyectos se referencian entre si y comparten
# Directory.Build.props). Equivalente a STAGE-12 (guia-publicacion-image-docker §2).

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Directory.Build.props aplica a todos los proyectos; se copia antes del restore.
COPY Directory.Build.props ./
COPY src/ src/
RUN dotnet restore src/GeoVial.Api/GeoVial.Api.csproj
RUN dotnet publish src/GeoVial.Api/GeoVial.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
# Ejecutar como usuario no-root (la imagen aspnet provee el usuario 'app').
USER app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENTRYPOINT ["dotnet", "GeoVial.Api.dll"]
