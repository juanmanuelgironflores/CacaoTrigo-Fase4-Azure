FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CacaoTrigo.csproj", "./"]
RUN dotnet restore "CacaoTrigo.csproj"
COPY . .
RUN dotnet publish "CacaoTrigo.csproj" --configuration Release --output /app/publish --no-restore && rm -f /app/publish/appsettings*.json

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CacaoTrigo.dll"]
