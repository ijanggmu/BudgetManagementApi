
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Install dependencies required by SkiaSharp and fonts
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    fontconfig \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["BeemaEdge.sln", "."]
COPY ["src/Web/BeemaEdgeApi.csproj", "src/Web/"]
COPY ["src/Data/Data/Data.csproj", "src/Data/Data/"]
COPY ["src/Business/Business.Common/Business.Common.csproj", "src/Business/Business.Common/"]
COPY ["src/Models/Models/Models.csproj", "src/Models/Models/"]
COPY ["src/SharedKernel/SharedKernel/SharedKernel.csproj", "src/SharedKernel/SharedKernel/"]
COPY ["src/Infrastructure/Infrastructure.Common/Infrastructure.Common.csproj", "src/Infrastructure/Infrastructure.Common/"]

# Restore dependencies
RUN dotnet restore "BeemaEdge.sln"

# Copy everything else
COPY . .

# Build the project
WORKDIR "/src/src/Web"
RUN dotnet build "BeemaEdgeApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BeemaEdgeApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Install fonts for QuestPDF
RUN mkdir -p /usr/share/fonts/truetype/nirmala
COPY src/Web/Assets/Fonts/Nirmala.ttf /usr/share/fonts/truetype/nirmala/ 2>/dev/null || true
RUN if [ -f /usr/share/fonts/truetype/nirmala/Nirmala.ttf ]; then \
        chmod 644 /usr/share/fonts/truetype/nirmala/Nirmala.ttf && \
        fc-cache -f -v; \
    fi

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

ENTRYPOINT ["dotnet", "BeemaEdgeApi.dll"]
