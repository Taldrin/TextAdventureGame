FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY ../../../ ./

WORKDIR /source/FurventureSite/FurventureSite/FurventureSite
RUN dotnet publish FurventureSite.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /source/FurventureSite/FurventureSite/FurventureSite/out/ ./

ENTRYPOINT ["dotnet", "FurventureSite.dll"]