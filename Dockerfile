# ========================================
# STAGE 1: Build (Compilación)
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivo de solución y todos los .csproj
COPY solution.sln ./
COPY webEscuela.Api/webEscuela.Api.csproj ./webEscuela.Api/
COPY webEscuela.Application/webEscuela.Application.csproj ./webEscuela.Application/
COPY webEscuela.Domain/webEscuela.Domain.csproj ./webEscuela.Domain/
COPY webEscuela.Infrastructure/webEscuela.Infrastructure.csproj ./webEscuela.Infrastructure/

# Restaurar dependencias (se cachea si no cambian los .csproj)
RUN dotnet restore solution.sln

# Copiar todo el código fuente
COPY webEscuela.Api/ ./webEscuela.Api/
COPY webEscuela.Application/ ./webEscuela.Application/
COPY webEscuela.Domain/ ./webEscuela.Domain/
COPY webEscuela.Infrastructure/ ./webEscuela.Infrastructure/

# Compilar el proyecto en modo Release
WORKDIR /src/webEscuela.Api
RUN dotnet build -c Release -o /app/build

# ========================================
# STAGE 2: Publish (Publicación)
# ========================================
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ========================================
# STAGE 3: Runtime (Imagen final ligera)
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Exponer puerto 8080 (puerto por defecto de .NET en contenedores)
EXPOSE 8080

# Copiar los archivos publicados desde el stage anterior
COPY --from=publish /app/publish .

# Punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "webEscuela.Api.dll"]