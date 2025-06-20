# ---------------------------------------------------
# 1) Build
# ---------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos la solución y los .csproj para cachear restore
COPY ["FinFlow.sln", "./"]
COPY ["src/FinFlow.Application.Contracts/FinFlow.Application.Contracts.csproj", "src/FinFlow.Application.Contracts/"]
COPY ["src/FinFlow.Application.Implementation/FinFlow.Application.Implementation.csproj", "src/FinFlow.Application.Implementation/"]
COPY ["src/FinFlow.Domain/FinFlow.Domain.csproj", "src/FinFlow.Domain/"]
COPY ["src/FinFlow.Infrastructure/FinFlow.Infrastructure.csproj", "src/FinFlow.Infrastructure/"]
COPY ["src/FinFlow.WebApi/FinFlow.WebApi.csproj", "src/FinFlow.WebApi/"]

# Restauramos a nivel de solución
RUN dotnet restore "FinFlow.sln"

# Copiamos todo el código y publicamos sólo el WebApi
COPY . .
WORKDIR "/src/src/FinFlow.WebApi"
RUN dotnet publish "FinFlow.WebApi.csproj" -c Release -o /app/publish

# ---------------------------------------------------
# 2) Runtime
# ---------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Traemos los binarios publicados
COPY --from=build /app/publish .

# Exponemos el puerto 80
EXPOSE 80

# Arrancamos la API
ENTRYPOINT ["dotnet", "FinFlow.WebApi.dll"]
