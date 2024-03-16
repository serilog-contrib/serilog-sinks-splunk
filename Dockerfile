FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . / 
WORKDIR /sample/Sample
RUN dotnet restore
RUN dotnet publish -c Release -o out -f net6.0

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /sample/Sample
COPY --from=build /sample/Sample/out ./
ENTRYPOINT ["dotnet", "Sample.dll"]