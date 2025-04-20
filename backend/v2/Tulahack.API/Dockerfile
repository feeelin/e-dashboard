FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
ENV ASPNETCORE_URLS=http://+:80
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Tulahack.API/Tulahack.API.csproj", "Tulahack.API/"]
RUN dotnet restore "Tulahack.API/Tulahack.API.csproj"
COPY . .
WORKDIR "/src/Tulahack.API"
RUN dotnet build "./Tulahack.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tulahack.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tulahack.API.dll"]
