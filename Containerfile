# build app
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

WORKDIR /src

COPY ReverseGeocode.sln .
COPY src/. ./src/

RUN dotnet restore
RUN dotnet publish -o /app -c Release


# build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1

WORKDIR /reverse-geocode

COPY --from=build /app .

ENTRYPOINT [ "/reverse-geocode/ReverseGeocode" ]
