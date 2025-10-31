
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Install dependencies required by SkiaSharp
RUN apt-get update && apt-get install -y libfontconfig1

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Web/CustomerPortalApi.csproj", "src/Web/"]
RUN dotnet restore "./src/Web/./CustomerPortalApi.csproj"
COPY . .
WORKDIR "/src/src/Web"
RUN dotnet build "./CustomerPortalApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CustomerPortalApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install dependencies required by questpdf font
RUN mkdir -p /usr/share/fonts/truetype/nirmala
COPY src/Web/Assets/Fonts/Nirmala.ttf /usr/share/fonts/truetype/nirmala/
RUN chmod 644 /usr/share/fonts/truetype/nirmala/Nirmala.ttf
RUN apt-get update && apt-get install -y fontconfig
RUN fc-cache -f -v

ENTRYPOINT ["dotnet", "CustomerPortalApi.dll"]
