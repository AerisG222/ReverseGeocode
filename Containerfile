# build app
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble-amd64 AS build
WORKDIR /reverse-geocode

COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY ReverseGeocode.slnx .
COPY global.json .
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
# console app, so the aspnet shared framework is dead weight.  the non-extra chiseled image is
# also enough: InvariantGlobalization drops the ICU requirement, and NodaTime carries its own tzdb.
FROM mcr.microsoft.com/dotnet/runtime:10.0-noble-chiseled-amd64
WORKDIR /reverse-geocode

COPY --from=build /build .

ENTRYPOINT [ "/reverse-geocode/ReverseGeocode" ]
