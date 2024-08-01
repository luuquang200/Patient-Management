# Use the official .NET image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy the app
COPY --from=build-env /app/out .

# Expose the port
EXPOSE 8080

ENTRYPOINT ["dotnet", "PatientManagementApp.dll"]