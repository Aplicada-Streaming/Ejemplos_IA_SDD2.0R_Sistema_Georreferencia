# syntax=docker/dockerfile:1
# Imagen de base de datos del monolito GeoVial: SQL Server 2022.
# Parte de la imagen oficial de Microsoft (guia-publicacion-image-docker §1). El esquema y el seed
# se aplican por migracion EF Core al arrancar el backend (scripts/db-create), no se hornean datos en
# la imagen (minimizacion de datos personales, ADR-14). La SA password y ACCEPT_EULA se inyectan por
# variables de entorno en el deploy, desde secretos (entornos-deploy §4); nunca en la imagen.
FROM mcr.microsoft.com/mssql/server:2022-latest
EXPOSE 1433
