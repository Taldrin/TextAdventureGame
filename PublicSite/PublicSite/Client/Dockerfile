FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY ../../../ ./

WORKDIR /source/PublicSite/PublicSite/Client
RUN dotnet publish PublicSite.Client.csproj -c Release -o out

FROM nginx:alpine
WORKDIR /app
COPY --from=build /source/PublicSite/PublicSite/Client/out/wwwroot /usr/share/nginx/html
COPY --from=build /source/PublicSite/PublicSite/Client/nginx.conf /etc/nginx/nginx.conf
EXPOSE 5007