# ──────────────────────────────────────────────────────────────
# Stage 1 – build & publish
# ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution manifest and every project file first so that
# Docker can cache the restore layer independently of source changes.
COPY OnlineShopping.slnx ./
COPY src/Domain/Domain.csproj                               src/Domain/
COPY src/Application/Application.csproj                     src/Application/
COPY src/Infrastructure/Infrastructure.csproj               src/Infrastructure/
COPY src/ConsoleApp/ConsoleApp.csproj                       src/ConsoleApp/
COPY tests/Application.Tests/Application.Tests.csproj       tests/Application.Tests/
COPY tests/Infrastructure.Tests/Infrastructure.Tests.csproj tests/Infrastructure.Tests/

RUN dotnet restore OnlineShopping.slnx

# Copy the remaining source code and publish the console app.
COPY . .

RUN dotnet publish src/ConsoleApp/ConsoleApp.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# ──────────────────────────────────────────────────────────────
# Stage 2 – minimal runtime image
# ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Use Production environment so only appsettings.json is loaded;
# the real connection string is injected at runtime via the
# ConnectionStrings__DefaultConnection environment variable.
ENV DOTNET_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ConsoleApp.dll"]
