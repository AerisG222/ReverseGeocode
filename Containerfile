# build app
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine-amd64 AS build

WORKDIR /src

COPY ReverseGeocode.sln .
COPY src/. ./src/

RUN dotnet restore
RUN dotnet publish src/ReverseGeocode/ReverseGeocode.csproj -o /app -c Release -r linux-musl-x64 --self-contained false


# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine-amd64

WORKDIR /reverse-geocode

COPY --from=build /app .

ENTRYPOINT [ "/reverse-geocode/ReverseGeocode" ]
