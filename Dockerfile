# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AI.TaskManagement.API/AI.TaskManagement.API.csproj", "AI.TaskManagement.API/"]
COPY ["AI.TaskManagement.Application/AI.TaskManagement.Application.csproj", "AI.TaskManagement.Application/"]
COPY ["AI.TaskManagement.Domain/AI.TaskManagement.Domain.csproj", "AI.TaskManagement.Domain/"]
COPY ["AI.TaskManagement.Infrastructure/AI.TaskManagement.Infrastructure.csproj", "AI.TaskManagement.Infrastructure/"]
COPY ["AI.TaskManagement.Shared/AI.TaskManagement.Shared.csproj", "AI.TaskManagement.Shared/"]

RUN dotnet restore "AI.TaskManagement.API/AI.TaskManagement.API.csproj"

COPY . .
WORKDIR "/src/AI.TaskManagement.API"
RUN dotnet build "AI.TaskManagement.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AI.TaskManagement.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5001
ENV ASPNETCORE_URLS=https://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "AI.TaskManagement.API.dll"]
