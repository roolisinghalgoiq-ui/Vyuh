FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and restore dependencies
COPY src/VYUH.Engine.sln ./src/
COPY src/Shared/VYUH.Shared.Contracts/VYUH.Shared.Contracts.csproj ./src/Shared/VYUH.Shared.Contracts/
COPY src/Services/Gateway/VYUH.Gateway.Api/VYUH.Gateway.Api.csproj ./src/Services/Gateway/VYUH.Gateway.Api/
COPY src/Services/Gateway/VYUH.Gateway.Application/VYUH.Gateway.Application.csproj ./src/Services/Gateway/VYUH.Gateway.Application/
COPY src/Services/Gateway/VYUH.Gateway.Domain/VYUH.Gateway.Domain.csproj ./src/Services/Gateway/VYUH.Gateway.Domain/
COPY src/Services/Gateway/VYUH.Gateway.Infrastructure/VYUH.Gateway.Infrastructure.csproj ./src/Services/Gateway/VYUH.Gateway.Infrastructure/

COPY src/Services/Ingestion/VYUH.Ingestion.Api/VYUH.Ingestion.Api.csproj ./src/Services/Ingestion/VYUH.Ingestion.Api/
COPY src/Services/Ingestion/VYUH.Ingestion.Application/VYUH.Ingestion.Application.csproj ./src/Services/Ingestion/VYUH.Ingestion.Application/
COPY src/Services/Ingestion/VYUH.Ingestion.Domain/VYUH.Ingestion.Domain.csproj ./src/Services/Ingestion/VYUH.Ingestion.Domain/
COPY src/Services/Ingestion/VYUH.Ingestion.Infrastructure/VYUH.Ingestion.Infrastructure.csproj ./src/Services/Ingestion/VYUH.Ingestion.Infrastructure/

COPY src/Services/Optimizer/VYUH.Optimizer.Api/VYUH.Optimizer.Api.csproj ./src/Services/Optimizer/VYUH.Optimizer.Api/
COPY src/Services/Optimizer/VYUH.Optimizer.Application/VYUH.Optimizer.Application.csproj ./src/Services/Optimizer/VYUH.Optimizer.Application/
COPY src/Services/Optimizer/VYUH.Optimizer.Domain/VYUH.Optimizer.Domain.csproj ./src/Services/Optimizer/VYUH.Optimizer.Domain/
COPY src/Services/Optimizer/VYUH.Optimizer.Infrastructure/VYUH.Optimizer.Infrastructure.csproj ./src/Services/Optimizer/VYUH.Optimizer.Infrastructure/

COPY src/Services/Risk/VYUH.Risk.Api/VYUH.Risk.Api.csproj ./src/Services/Risk/VYUH.Risk.Api/
COPY src/Services/Risk/VYUH.Risk.Application/VYUH.Risk.Application.csproj ./src/Services/Risk/VYUH.Risk.Application/
COPY src/Services/Risk/VYUH.Risk.Domain/VYUH.Risk.Domain.csproj ./src/Services/Risk/VYUH.Risk.Domain/
COPY src/Services/Risk/VYUH.Risk.Infrastructure/VYUH.Risk.Infrastructure.csproj ./src/Services/Risk/VYUH.Risk.Infrastructure/

RUN dotnet restore ./src/Services/Risk/VYUH.Risk.Api/VYUH.Risk.Api.csproj

# Copy everything and build
COPY . .
RUN dotnet publish ./src/Services/Risk/VYUH.Risk.Api/VYUH.Risk.Api.csproj -c Release -o /out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "VYUH.Risk.Api.dll"]
