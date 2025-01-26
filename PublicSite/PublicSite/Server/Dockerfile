FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY ../../../ ./

WORKDIR /source/PublicSite/PublicSite/Server
RUN dotnet publish PublicSite.Server.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /source/PublicSite/PublicSite/Server/out/ ./

ENTRYPOINT ["dotnet", "PublicSite.Server.dll"]