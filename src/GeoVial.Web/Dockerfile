# syntax=docker/dockerfile:1
# Imagen front del monolito GeoVial (GeoVial.Web, Blazor Interactive Server).
# Build multi-stage: SDK .NET 10 para compilar/publicar, runtime ASP.NET minimo para ejecutar.
# Contexto de build: la raiz del repo (comparte Directory.Build.props con el resto).
# Equivalente a STAGE-12 (guia-publicacion-image-docker §2). El front consume la API por ApiBaseUrl.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY src/ src/
RUN dotnet restore src/GeoVial.Web/GeoVial.Web.csproj
RUN dotnet publish src/GeoVial.Web/GeoVial.Web.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
USER app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENTRYPOINT ["dotnet", "GeoVial.Web.dll"]
