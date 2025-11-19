#base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
LABEL service="SFC.Identity.Api" author="Kryvoruk Andrii" email="krivorukandrey@gmail.com"
USER root
WORKDIR /app

#restore & build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV HUSKY=False
WORKDIR /source
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["src/API/SFC.Identity.Api/SFC.Identity.Api.csproj", "src/API/SFC.Identity.Api/"]
COPY ["src/Client/SFC.Identity.Client/SFC.Identity.Client.esproj", "src/Client/SFC.Identity.Client/"]
COPY ["src/Core/SFC.Identity.Application/SFC.Identity.Application.csproj", "src/Core/SFC.Identity.Application/"]
COPY ["src/Core/SFC.Identity.Domain/SFC.Identity.Domain.csproj", "src/Core/SFC.Identity.Domain/"]
COPY ["src/Infrastructure/SFC.Identity.Infrastructure.Persistence/SFC.Identity.Infrastructure.Persistence.csproj", "src/Infrastructure/SFC.Identity.Infrastructure.Persistence/"]
COPY ["src/Infrastructure/SFC.Identity.Infrastructure/SFC.Identity.Infrastructure.csproj", "src/Infrastructure/SFC.Identity.Infrastructure/"]
COPY ["src/Contracts/SFC.Identity.Contracts/SFC.Identity.Contracts.csproj", "src/Contracts/SFC.Identity.Contracts/"]
COPY ["src/Contracts/SFC.Identity.Messages/SFC.Identity.Messages.csproj", "src/Contracts/SFC.Identity.Messages/"]
RUN dotnet restore "./src/API/SFC.Identity.Api/SFC.Identity.Api.csproj" --property:HUSKY=$HUSKY
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_lts.x | bash - && \
    apt-get install -y nodejs
COPY [".", "."]
WORKDIR "/source/src/API/SFC.Identity.Api"
RUN dotnet build "./SFC.Identity.Api.csproj" --configuration $BUILD_CONFIGURATION --property:HUSKY=$HUSKY --output /app/build

#publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SFC.Identity.Api.csproj" --configuration $BUILD_CONFIGURATION --property:HUSKY=$HUSKY --property:UseAppHost=false --output /app/publish

#final
FROM base AS final
ARG HTTP_PORT=8080
ARG HTTPS_PORT=8081
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE $HTTP_PORT
EXPOSE $HTTPS_PORT
ENTRYPOINT ["bash", "entrypoint.sh"]