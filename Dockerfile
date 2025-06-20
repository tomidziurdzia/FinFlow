# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos csproj(s) y restauramos dependencias primero (cache)
COPY ["FinFlow.csproj", "./"]
RUN dotnet restore "FinFlow.csproj"

# Copiamos el resto del código y publicamos
COPY . .
RUN dotnet publish "FinFlow.csproj" -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Exponemos el puerto que uses (por defecto 80)
EXPOSE 80

ENTRYPOINT ["dotnet", "FinFlow.dll"]
