param(
    [string]$version = "0.0.0",
    [string]$output = "dist"
)

# Test the app.
dotnet test ./src/IAmGhost.sln
# Publish the app to the output.
dotnet publish ./src/IAmGhost/IAmGhost.csproj -c release -o "$PSScriptRoot/$output"