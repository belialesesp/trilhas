# Base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Trilhas.csproj", "."]
RUN dotnet restore "Trilhas.csproj"

# Copy the rest of the code and build
COPY . .
RUN dotnet build "Trilhas.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "Trilhas.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Trilhas.dll"]