FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /PublicSite

# Copy everything
COPY ../ ./

# Build and publish a release
WORKDIR /PublicSite/Server
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /PublicSite
COPY --from=build PublicSite/PublicSite/Server/out .
ENTRYPOINT ["dotnet", "PublicSite.Server.dll"]
