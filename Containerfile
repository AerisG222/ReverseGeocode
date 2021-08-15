# build app
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine-amd64 as build

WORKDIR /src

COPY ReverseGeocode.sln .
COPY src/. ./src/

RUN dotnet restore
RUN dotnet publish -o /app -c Release


# build runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine-amd64

WORKDIR /reverse-geocode

COPY _run.sh .
COPY --from=build /app .

ENTRYPOINT [ "_run.sh" ]
