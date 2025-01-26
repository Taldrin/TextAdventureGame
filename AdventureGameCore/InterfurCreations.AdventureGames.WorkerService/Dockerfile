FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /AdventureGameCore

# Copy everything
COPY . ./

# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /AdventureGameCore
COPY --from=build /AdventureGameCore/out ./
ENTRYPOINT ["dotnet", "InterfurCreations.AdventureGames.WorkerService.dll"]
