FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
EXPOSE 5000
EXPOSE 443

COPY LA_LIGA_REKREATIVO/ .
RUN ls

RUN dotnet restore Server/LA_LIGA_REKREATIVO.Server.csproj


RUN dotnet publish Server/LA_LIGA_REKREATIVO.Server.csproj -c Release -o out



FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
 
# expose ports
EXPOSE 8080
EXPOSE 8443
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "LA_LIGA_REKREATIVO.Server.dll","--urls", "http://+:8080"]