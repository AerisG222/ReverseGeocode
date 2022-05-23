# build app
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine-amd64 AS build

WORKDIR /src

COPY ReverseGeocode.sln .
COPY src/. ./src/

RUN dotnet restore
RUN dotnet publish -o /app -c Release -r linux-musl-x64 --self-contained false


# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine-amd64

WORKDIR /reverse-geocode

COPY --from=build /app .

ENTRYPOINT [ "/reverse-geocode/ReverseGeocode" ]
