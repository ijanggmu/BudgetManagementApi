# ----------------- BASE IMAGE -----------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Install dependencies required by SkiaSharp and fonts
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    fontconfig \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081

# ----------------- BUILD IMAGE -----------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the entire repo to ensure all projects exist for restore
COPY . .

# Restore dependencies for the whole solution
RUN dotnet restore "BeemaEdge.sln"

# Build the Web API project
WORKDIR "/src/src/Web"
RUN dotnet build "BeemaEdgeApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ----------------- PUBLISH IMAGE -----------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/Web"
RUN dotnet publish "BeemaEdgeApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ----------------- FINAL IMAGE -----------------
FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# ----------------- ADD FONTS -----------------
RUN mkdir -p /usr/share/fonts/truetype/nirmala
COPY --from=build /src/src/Web/Assets/Fonts/Nirmala.ttf /usr/share/fonts/truetype/nirmala/Nirmala.ttf
RUN chmod 644 /usr/share/fonts/truetype/nirmala/Nirmala.ttf \
    && fc-cache -f -v

# ----------------- SECURITY -----------------
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

# ----------------- ENTRYPOINT -----------------
ENTRYPOINT ["dotnet", "BeemaEdgeApi.dll"]
