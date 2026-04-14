FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GerenciamentoProducaoo.csproj", "./"]
RUN dotnet restore "GerenciamentoProducaoo.csproj"

COPY . .
RUN dotnet publish "GerenciamentoProducaoo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet GerenciamentoProducaoo.dll"]
