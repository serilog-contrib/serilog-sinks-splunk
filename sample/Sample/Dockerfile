#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Create this "restore-solution" section by running ./Create-DockerfileSolutionRestore.ps1, to optimize build cache reuse
COPY ["sample/Sample/Sample.csproj", "sample/Sample/"]
COPY ["src/Serilog.Sinks.Splunk/Serilog.Sinks.Splunk.csproj", "src/Serilog.Sinks.Splunk/"]
COPY ["src/Serilog.Sinks.TCP/Serilog.Sinks.Splunk.TCP.csproj", "src/Serilog.Sinks.TCP/"]
COPY ["src/Serilog.Sinks.UDP/Serilog.Sinks.Splunk.UDP.csproj", "src/Serilog.Sinks.UDP/"]
COPY ["test/Serilog.Sinks.Splunk.Tests/Serilog.Sinks.Splunk.Tests.csproj", "test/Serilog.Sinks.Splunk.Tests/"]
COPY ["docker-compose.dcproj", "./"]
COPY ["nuget.config", "./"]
COPY ["serilog-sinks-splunk.sln", "./"]
RUN dotnet restore "serilog-sinks-splunk.sln"

COPY . .
WORKDIR "/src/sample/Sample/"
RUN dotnet build "./Sample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.dll"]