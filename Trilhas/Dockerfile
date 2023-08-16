#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.prodest.es.gov.br/microsoft/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM registry.prodest.es.gov.br/microsoft/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Trilhas/Trilhas.csproj", "Trilhas/"]
RUN dotnet restore "Trilhas/Trilhas.csproj"
COPY . .
WORKDIR "/src/Trilhas"
RUN dotnet build "Trilhas.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trilhas.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trilhas.dll"]