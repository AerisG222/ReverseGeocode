# build app
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble-amd64 AS build
WORKDIR /reverse-geocode

COPY ReverseGeocode.slnx .
COPY nuget.config .
COPY src/ReverseGeocode/ReverseGeocode.csproj src/ReverseGeocode/
RUN dotnet restore \
    --runtime linux-x64 \
    src/ReverseGeocode/ReverseGeocode.csproj

COPY src/. src/
RUN dotnet publish \
    --no-restore \
    --no-self-contained \
    --configuration Release \
    --runtime linux-x64 \
    --output /build \
    src/ReverseGeocode/ReverseGeocode.csproj


# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra-amd64
WORKDIR /reverse-geocode

COPY --from=build /build .

ENTRYPOINT [ "/reverse-geocode/ReverseGeocode" ]
