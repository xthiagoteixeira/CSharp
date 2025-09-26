# Dockerfile para Api_Transferencia
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5037

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Api_Transferencia.csproj", "./"]
RUN dotnet restore "Api_Transferencia.csproj"
COPY . .
RUN dotnet build "Api_Transferencia.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api_Transferencia.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configurar a aplicação para escutar em todas as interfaces
ENV ASPNETCORE_URLS=http://+:5037
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Api_Transferencia.dll"]