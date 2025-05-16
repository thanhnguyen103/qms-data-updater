# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY src/QMS-Data-Updater.Functions/QMS-Data-Updater.Functions.csproj ./QMS-Data-Updater.Functions/
RUN dotnet restore ./QMS-Data-Updater.Functions/QMS-Data-Updater.Functions.csproj

# Copy the rest of the source code
COPY . .

# Publish the application
RUN dotnet publish ./src/QMS-Data-Updater.Functions/QMS-Data-Updater.Functions.csproj -c Release -o /app/publish

# Use the official .NET runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (optional, adjust if needed)
EXPOSE 80

# Set the entrypoint
ENTRYPOINT ["dotnet", "QMS-Data-Updater.Functions.dll"]