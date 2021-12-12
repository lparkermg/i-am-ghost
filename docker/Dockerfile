# Base Image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as dependencies
WORKDIR /build

# Restore Dependencies from each of the csproj file.
COPY ./src/IAmGhost/*.csproj ./IAmGhost/
COPY ./src/IAmGhost.Tests/*.csproj ./IAmGhost.Tests/
COPY ./src/*.sln ./
RUN dotnet restore

# Test and Build
FROM dependencies as build
COPY ./src/ ./
RUN dotnet test "./IAmGhost.sln" --logger "console;verbosity=normal" --no-restore
RUN dotnet publish "./IAmGhost/IAmGhost.csproj" -c release -o "./out" --no-restore

# Final image

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as final
WORKDIR /app
COPY --from=build /build/out/ ./
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "IAmGhost.dll"]