# build app
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine-amd64 AS build

WORKDIR /src

COPY ReverseGeocode.sln .
COPY src/. ./src/

RUN dotnet restore
RUN dotnet publish -o /app -c Release


# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine-amd64

WORKDIR /reverse-geocode

COPY _run.sh /usr/local/bin
COPY --from=build /app .

ENTRYPOINT [ "_run.sh" ]
