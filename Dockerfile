FROM microsoft/dotnet-framework:4.7.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY KeepassPinentry/*.csproj ./KeepassPinentry/
RUN dotnet restore

# copy everything else and build app
COPY KeepassPinentry/ ./KeepassPinentry/
COPY Keepass/ ./Keepass/
WORKDIR /app/KeepassPinentry
RUN dotnet build

#  Creates on container:
#  C:\app\KeepassPinentry\bin\Debug\KeepassPinentry.dll