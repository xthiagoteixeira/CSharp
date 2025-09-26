# Dockerfile para Api_ContaCorrente
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5222

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Api_ContaCorrente.csproj", "./"]
RUN dotnet restore "Api_ContaCorrente.csproj"
COPY . .
RUN dotnet build "Api_ContaCorrente.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api_ContaCorrente.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configurar a aplicação para escutar em todas as interfaces
ENV ASPNETCORE_URLS=http://+:5222
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Api_ContaCorrente.dll"]