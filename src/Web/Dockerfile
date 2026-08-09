# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    fontconfig \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Web/BeemaEdgeApi.csproj", "src/Web/"]
COPY ["src/Business/Business.AdminPortalApi/Business.AdminPortalApi/Business.AdminPortalApi.csproj", "src/Business/Business.AdminPortalApi/Business.AdminPortalApi/"]
COPY ["src/Data/Data/Data.csproj", "src/Data/Data/"]
COPY ["src/Infrastructure/Infrastructure.Common/Infrastructure.Common.csproj", "src/Infrastructure/Infrastructure.Common/"]
COPY ["src/Models/Models/Models.csproj", "src/Models/Models/"]
COPY ["src/SharedKernel/SharedKernel/SharedKernel.csproj", "src/SharedKernel/SharedKernel/"]
COPY ["src/Business/Business.Common/Business.Common.csproj", "src/Business/Business.Common/"]
COPY ["src/Infrastructure/Infrastructure.CoreApi/Infrastructure.CoreApi/Infrastructure.CoreApi.csproj", "src/Infrastructure/Infrastructure.CoreApi/Infrastructure.CoreApi/"]
COPY ["src/Business/Business.CustomerPortalApi/Business.BeemaEdgeApi.csproj", "src/Business/Business.CustomerPortalApi/"]
RUN dotnet restore "./src/Web/BeemaEdgeApi.csproj"
COPY . .
WORKDIR "/src/src/Web"
RUN dotnet build "./BeemaEdgeApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BeemaEdgeApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Add fonts
RUN mkdir -p /usr/share/fonts/truetype/nirmala
COPY --from=build /src/src/Web/Assets/Fonts/Nirmala.ttf /usr/share/fonts/truetype/nirmala/Nirmala.ttf
RUN chmod 644 /usr/share/fonts/truetype/nirmala/Nirmala.ttf && fc-cache -f -v

# Security: non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "BeemaEdgeApi.dll"]
