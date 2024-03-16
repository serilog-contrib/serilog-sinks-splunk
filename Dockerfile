FROM microsoft/dotnet:8.0-sdk AS build
ADD . / 
WORKDIR /sample/Sample
RUN dotnet restore
RUN dotnet publish -c Release -o out -f net8.0

FROM microsoft/dotnet:8.0-runtime AS runtime
WORKDIR /sample/Sample
COPY --from=build /sample/Sample/out ./
ENTRYPOINT ["dotnet", "Sample.dll"]