FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY ../../ ./

WORKDIR /source/AdminSite/InterfurCreations.AdminSite
RUN dotnet publish InterfurCreations.AdminSite.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /source/AdminSite/InterfurCreations.AdminSite/out/ ./

ENTRYPOINT ["dotnet", "InterfurCreations.AdminSite.dll"]