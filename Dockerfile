FROM mcr.microsoft.com/dotnet/framework/sdk:4.7.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY KeepassPinentry/*.csproj ./KeepassPinentry/
RUN dotnet restore

# copy everything else and build app
COPY KeepassPinentry/ ./KeepassPinentry/
COPY Keepass/ ./Keepass/
WORKDIR /app/KeepassPinentry
RUN dotnet --info
RUN dotnet build

#  Creates on container:
#  C:\app\KeepassPinentry\bin\Debug\KeepassPinentry.dll
