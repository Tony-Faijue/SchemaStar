#--- Stage 1 Build Image ---
# .NET SDK base image to build and run the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Set the working directory
WORKDIR /src

# Copy the dependency definitions to the docker container
COPY SchemaStar/SchemaStar.csproj SchemaStar/

# Downloads the dependencies for the NuGet packages
RUN dotnet restore SchemaStar/SchemaStar.csproj

# Copies the source files for the project
# Docker Cache : Does not re-run a command unless a change occurs for those files
# Changes to the source code will not re-run commands for dependencies unless dependency files change
COPY SchemaStar/ SchemaStar/

# Compile and Package the project
# -c build in Release configuration
# -o puts the application into /app/publish directory
# --no-restore stops running the restore command again which is caused by the publish command
RUN dotnet publish SchemaStar/SchemaStar.csproj -c Release -o /app/publish --no-restore

#--- Stage 2 Runtime Image ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SchemaStar.dll"]
