FROM microsoft/dotnet:2.1-sdk AS build
ADD . / 
WORKDIR /sample/Sample
RUN dotnet restore
RUN dotnet publish -c Release -o out -f netcoreapp2.0

FROM microsoft/dotnet:2.1-runtime AS runtime
WORKDIR /sample/Sample
COPY --from=build /sample/Sample/out ./
ENTRYPOINT ["dotnet", "Sample.dll"]