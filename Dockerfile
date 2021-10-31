#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY PQM-WebApp/PQM-WebApp/PQM-WebApp.csproj PQM-WebApp/
COPY PQM-WebApp/PQM-WebApp.Service/PQM-WebApp.Service.csproj PQM-WebApp.Service/
COPY PQM-WebApp/PQM-WebApp.Data/PQM-WebApp.Data.csproj PQM-WebApp.Data/
RUN dotnet restore "PQM-WebApp/PQM-WebApp.csproj"
COPY . .
WORKDIR "/src/PQM-WebApp"
RUN dotnet build "PQM-WebApp/PQM-WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PQM-WebApp/PQM-WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PQM-WebApp.dll"]
